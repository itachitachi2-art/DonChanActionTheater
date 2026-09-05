using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DonChanActionTheater
{
    [Serializable]
    internal sealed class AnimationSettings
    {
        public float fps = 10f;
        public bool loop = false;
        public float holdLastFrame = 0.18f;
        public float weight = 1f;
        public float displaySeconds = 0f;
        public float repeatCount = 1f;
    }

    internal sealed class AnimationVariant
    {
        internal string Topic;
        internal string Subtype;
        internal string Name;
        internal string[] FramePaths;
        internal Texture2D[] Frames;
        internal AnimationSettings Settings;

        internal float NaturalDuration
        {
            get
            {
                if (Settings.displaySeconds > 0f) return Settings.displaySeconds;
                return Math.Max(0.12f, FramePaths.Length * Math.Max(1f, Settings.repeatCount)
                    / Math.Max(1f, Settings.fps) + Settings.holdLastFrame);
            }
        }
    }

    internal sealed class AnimationLibrary
    {
        private readonly string root;
        private readonly List<AnimationVariant> variants = new List<AnimationVariant>();
        private static MethodInfo loadImageMethod;

        internal AnimationLibrary(string modPath)
        {
            root = Path.Combine(modPath, "Resources", "Animations");
        }

        internal int Reload()
        {
            Unload();
            if (!Directory.Exists(root)) return 0;

            foreach (string directory in Directory.GetDirectories(root, "*", SearchOption.AllDirectories))
            {
                string[] frames = Directory.GetFiles(directory, "*.png", SearchOption.TopDirectoryOnly);
                if (frames.Length == 0) continue;
                Array.Sort(frames, StringComparer.OrdinalIgnoreCase);

                string relative = directory.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string[] parts = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (parts.Length < 2) continue;

                AnimationSettings settings = new AnimationSettings();
                string jsonPath = Path.Combine(directory, "animation.json");
                if (File.Exists(jsonPath))
                {
                    try { JsonConfig.Apply(File.ReadAllText(jsonPath), settings); }
                    catch (Exception e) { Debug.LogWarning("[DonChanActionTheater] Invalid animation.json: " + jsonPath + " / " + e.Message); }
                }

                variants.Add(new AnimationVariant
                {
                    Topic = parts[0],
                    Subtype = parts.Length >= 3 ? parts[1] : "Generic",
                    Name = parts[parts.Length - 1],
                    FramePaths = frames,
                    Settings = settings
                });
            }
            Debug.Log("[DonChanActionTheater] animations loaded: " + variants.Count);
            return variants.Count;
        }

        internal AnimationVariant Choose(string topic, string subtype, AnimationVariant avoid)
        {
            List<AnimationVariant> candidates = Find(topic, subtype);
            if (candidates.Count == 0 && !string.Equals(subtype, "Generic", StringComparison.OrdinalIgnoreCase))
                candidates = Find(topic, "Generic");
            if (candidates.Count == 0) return null;

            float total = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates.Count > 1 && candidates[i] == avoid) continue;
                total += Math.Max(0.01f, candidates[i].Settings.weight);
            }
            float roll = UnityEngine.Random.Range(0f, total);
            for (int i = 0; i < candidates.Count; i++)
            {
                AnimationVariant candidate = candidates[i];
                if (candidates.Count > 1 && candidate == avoid) continue;
                roll -= Math.Max(0.01f, candidate.Settings.weight);
                if (roll <= 0f) return candidate;
            }
            return candidates[0];
        }

        internal Texture2D GetFrame(AnimationVariant variant, int index)
        {
            if (variant == null || variant.FramePaths.Length == 0) return null;
            if (variant.Frames == null) variant.Frames = new Texture2D[variant.FramePaths.Length];
            index = Mathf.Clamp(index, 0, variant.FramePaths.Length - 1);
            if (variant.Frames[index] != null) return variant.Frames[index];
            try
            {
                byte[] bytes = File.ReadAllBytes(variant.FramePaths[index]);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!LoadPng(texture, bytes))
                {
                    UnityEngine.Object.Destroy(texture);
                    return null;
                }
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                variant.Frames[index] = texture;
                return texture;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[DonChanActionTheater] Could not load frame: " + e.Message);
                return null;
            }
        }

        private List<AnimationVariant> Find(string topic, string subtype)
        {
            return variants.FindAll(v => string.Equals(v.Topic, topic, StringComparison.OrdinalIgnoreCase)
                && string.Equals(v.Subtype, subtype, StringComparison.OrdinalIgnoreCase));
        }

        private static bool LoadPng(Texture2D texture, byte[] bytes)
        {
            if (loadImageMethod == null)
            {
                Type imageConversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                if (imageConversion != null)
                    loadImageMethod = imageConversion.GetMethod("LoadImage", BindingFlags.Public | BindingFlags.Static, null,
                        new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) }, null);
            }
            if (loadImageMethod == null) return false;
            object result = loadImageMethod.Invoke(null, new object[] { texture, bytes, false });
            return result is bool && (bool)result;
        }

        private void Unload()
        {
            for (int i = 0; i < variants.Count; i++)
            {
                Texture2D[] frames = variants[i].Frames;
                if (frames == null) continue;
                for (int j = 0; j < frames.Length; j++) if (frames[j] != null) UnityEngine.Object.Destroy(frames[j]);
            }
            variants.Clear();
        }
    }
}
