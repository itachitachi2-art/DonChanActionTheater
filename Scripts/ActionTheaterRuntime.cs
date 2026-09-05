using System;
using System.IO;
using UnityEngine;

namespace DonChanActionTheater
{
    [Serializable]
    internal sealed class TheaterConfig
    {
        public bool enabled = true;
        public float width = 300f;
        public float height = 300f;
        public float scale = 1.5f;
        public float centerX = 0.92f;
        public float centerY = 0.42f;
        public float opacity = 1f;
        public float sequenceLockSeconds = 0.70f;
    }

    internal sealed class ActionTheaterRuntime : MonoBehaviour
    {
        internal static ActionTheaterRuntime Instance { get; private set; }

        private AnimationLibrary library;
        private TheaterConfig config = new TheaterConfig();
        private string configPath;
        private AnimationVariant current;
        private AnimationVariant lastFood, lastHealing, lastCraft, lastMelee, lastRanged, lastVehicle;
        private float startedAt;
        private float visibleUntil;
        private float pulseUntil;
        private float sequenceUntil;
        private string sequenceKey = "";
        private int currentPriority;
        private bool layoutDirty;
        private float saveLayoutAt;

        private const string ScalePreference = "DonChanActionTheater.Scale";
        private const string CenterXPreference = "DonChanActionTheater.CenterX";
        private const string CenterYPreference = "DonChanActionTheater.CenterY";
        private const string EnabledPreference = "DonChanActionTheater.Enabled";

        internal static void Create(string modPath)
        {
            if (Instance != null) return;
            GameObject go = new GameObject("DonChanActionTheater.Runtime");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<ActionTheaterRuntime>();
            Instance.Initialize(modPath);
        }

        private void Initialize(string modPath)
        {
            configPath = Path.Combine(modPath, "Config", "theater.json");
            Reload();
        }

        internal void Trigger(ActionTopic topic, string subtype, string itemName)
        {
            if (!config.enabled || library == null) return;
            int priority = Priority(topic);
            float now = Time.unscaledTime;
            if (current != null && now < visibleUntil && priority < currentPriority) return;

            string key = topic + "/" + subtype + "/" + (itemName ?? "");
            // Held buttons and rapid clicks can report the same action many times.
            // Do not restart or extend a sequence which is still visibly playing.
            if (current != null && now < visibleUntil && string.Equals(sequenceKey, key, StringComparison.Ordinal))
                return;

            bool continuing = now < sequenceUntil && string.Equals(sequenceKey, key, StringComparison.Ordinal);
            AnimationVariant selected = continuing && current != null
                ? current
                : library.Choose(topic.ToString(), subtype, Previous(topic));
            if (selected == null) return;

            current = selected;
            if (!continuing) Remember(topic, selected);
            startedAt = now;
            pulseUntil = now + 0.11f;
            visibleUntil = now + selected.NaturalDuration;
            sequenceUntil = now + Math.Max(0.1f, config.sequenceLockSeconds);
            sequenceKey = key;
            currentPriority = priority;
        }

        private void Update()
        {
            if (layoutDirty && Time.unscaledTime >= saveLayoutAt)
            {
                PlayerPrefs.Save();
                layoutDirty = false;
            }
        }

        private void OnGUI()
        {
            if (XUi.InGameMenuOpen)
            {
                DrawEscSettingsPanel();
                return;
            }
            if (!config.enabled || current == null || Time.unscaledTime >= visibleUntil) return;
            float elapsed = Math.Max(0f, Time.unscaledTime - startedAt);
            int raw = Mathf.FloorToInt(elapsed * Math.Max(1f, current.Settings.fps));
            int repeatCount = Math.Max(1, Mathf.RoundToInt(current.Settings.repeatCount));
            int index = current.Settings.loop || raw < current.FramePaths.Length * repeatCount
                ? raw % current.FramePaths.Length
                : Math.Min(raw, current.FramePaths.Length - 1);
            Texture2D frame = library.GetFrame(current, index);
            if (frame == null) return;

            float pulse = Time.unscaledTime < pulseUntil ? 1.045f : 1f;
            float width = config.width * config.scale * pulse;
            float height = config.height * config.scale * pulse;
            Rect rect = PositionedRect(width, height);
            Color old = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(config.opacity));
            GUI.DrawTexture(rect, frame, ScaleMode.ScaleToFit, true);
            GUI.color = old;
        }

        private Rect PositionedRect(float width, float height)
        {
            float x = Mathf.Clamp(Screen.width * config.centerX - width * 0.5f, 0f, Math.Max(0f, Screen.width - width));
            float y = Mathf.Clamp(Screen.height * config.centerY - height * 0.5f, 0f, Math.Max(0f, Screen.height - height));
            return new Rect(x, y, width, height);
        }

        private void DrawEscSettingsPanel()
        {
            float menuScale = Mathf.Clamp(Screen.height / 1440f, 1f, 1.5f);
            Matrix4x4 oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(menuScale, menuScale, 1f));

            float width = 430f;
            float height = 245f;
            float x = Screen.width / menuScale - width - 30f;
            float y = Screen.height / menuScale - height - 30f;
            GUI.Box(new Rect(x, y, width, height), "DON-CHAN ACTION THEATER");

            GUI.Label(new Rect(x + 24f, y + 34f, 125f, 24f), "DISPLAY SIZE");
            float newScale = GUI.HorizontalSlider(new Rect(x + 145f, y + 41f, 205f, 20f), config.scale, 0.5f, 3f);
            GUI.Label(new Rect(x + 360f, y + 34f, 55f, 24f), Mathf.RoundToInt(newScale * 100f) + "%");

            GUI.Label(new Rect(x + 24f, y + 76f, 125f, 24f), "POSITION X");
            float newCenterX = GUI.HorizontalSlider(new Rect(x + 145f, y + 83f, 205f, 20f), config.centerX, 0.05f, 0.95f);
            GUI.Label(new Rect(x + 360f, y + 76f, 55f, 24f), Mathf.RoundToInt(newCenterX * 100f) + "%");

            GUI.Label(new Rect(x + 24f, y + 118f, 125f, 24f), "POSITION Y");
            float newCenterY = GUI.HorizontalSlider(new Rect(x + 145f, y + 125f, 205f, 20f), config.centerY, 0.05f, 0.95f);
            GUI.Label(new Rect(x + 360f, y + 118f, 55f, 24f), Mathf.RoundToInt(newCenterY * 100f) + "%");

            if (!Mathf.Approximately(newScale, config.scale) || !Mathf.Approximately(newCenterX, config.centerX)
                || !Mathf.Approximately(newCenterY, config.centerY))
            {
                config.scale = newScale;
                config.centerX = newCenterX;
                config.centerY = newCenterY;
                StoreLayout();
            }

            if (GUI.Button(new Rect(x + 24f, y + 168f, 180f, 42f), config.enabled ? "THEATER: ON" : "THEATER: OFF"))
            {
                config.enabled = !config.enabled;
                PlayerPrefs.SetInt(EnabledPreference, config.enabled ? 1 : 0);
                layoutDirty = true;
                saveLayoutAt = Time.unscaledTime + 0.35f;
            }
            if (GUI.Button(new Rect(x + 226f, y + 168f, 180f, 42f), "RESET LAYOUT"))
            {
                config.scale = 1.5f;
                config.centerX = 0.92f;
                config.centerY = 0.42f;
                StoreLayout();
            }
            GUI.matrix = oldMatrix;

            Color oldColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.35f);
            GUI.Box(PositionedRect(config.width * config.scale, config.height * config.scale), "ACTION THEATER PREVIEW");
            GUI.color = oldColor;
        }

        private void StoreLayout()
        {
            PlayerPrefs.SetFloat(ScalePreference, config.scale);
            PlayerPrefs.SetFloat(CenterXPreference, config.centerX);
            PlayerPrefs.SetFloat(CenterYPreference, config.centerY);
            layoutDirty = true;
            saveLayoutAt = Time.unscaledTime + 0.35f;
        }

        internal string RunCommand(string action)
        {
            string cmd = (action ?? "").Trim().ToLowerInvariant();
            if (cmd == "on") { SetEnabled(true); return "Don-chan Action Theater: ON"; }
            if (cmd == "off") { SetEnabled(false); current = null; return "Don-chan Action Theater: OFF"; }
            if (cmd == "reload") { int count = Reload(); return "Don-chan Action Theater: reloaded " + count + " animation folders."; }
            if (cmd == "test food") { Trigger(ActionTopic.Food, "Generic", "test"); return "Food animation test."; }
            if (cmd == "test healing") { Trigger(ActionTopic.Healing, "Generic", "test"); return "Healing animation test."; }
            if (cmd == "test craft") { Trigger(ActionTopic.Craft, "Generic", "test"); return "Craft animation test."; }
            if (cmd == "test melee") { Trigger(ActionTopic.Melee, "Generic", "test"); return "Melee animation test."; }
            if (cmd.StartsWith("test melee "))
            {
                string subtype = Title(cmd.Substring("test melee ".Length));
                Trigger(ActionTopic.Melee, subtype, "test-" + subtype);
                return "Melee/" + subtype + " animation test.";
            }
            if (cmd == "test ranged") { Trigger(ActionTopic.Ranged, "Pistol", "test"); return "Ranged/Pistol animation test."; }
            if (cmd.StartsWith("test ranged "))
            {
                string subtype = Title(cmd.Substring("test ranged ".Length));
                Trigger(ActionTopic.Ranged, subtype, "test-" + subtype);
                return "Ranged/" + subtype + " animation test.";
            }
            if (cmd.StartsWith("test vehicle "))
            {
                string subtype = VehicleTitle(cmd.Substring("test vehicle ".Length));
                Trigger(ActionTopic.Vehicle, subtype, "test-" + subtype);
                return "Vehicle/" + subtype + " animation test.";
            }
            return "dat on | off | reload | test food|healing|craft|melee [type]|ranged [type]|vehicle [type]";
        }

        private static string Title(string value)
        {
            if (string.IsNullOrEmpty(value)) return "Generic";
            if (string.Equals(value, "smg", StringComparison.OrdinalIgnoreCase)) return "SMG";
            if (string.Equals(value.Replace(" ", ""), "desertvulture", StringComparison.OrdinalIgnoreCase)) return "DesertVulture";
            return char.ToUpperInvariant(value[0]) + value.Substring(1).ToLowerInvariant();
        }

        private void SetEnabled(bool enabled)
        {
            config.enabled = enabled;
            PlayerPrefs.SetInt(EnabledPreference, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        private static string VehicleTitle(string value)
        {
            string normalized = (value ?? "").Replace("_", "").Replace(" ", "").ToLowerInvariant();
            if (normalized == "bike" || normalized == "motorbike") return "Motorcycle";
            if (normalized == "minibike") return "Minibike";
            if (normalized == "4x4" || normalized == "car" || normalized == "truck") return "Jeep";
            if (normalized == "gyro" || normalized == "gyrocopter") return "Gyrocopter";
            if (normalized == "bicycle" || normalized == "cycle") return "Bicycle";
            return Title(normalized);
        }

        private int Reload()
        {
            try
            {
                config = new TheaterConfig();
                if (File.Exists(configPath)) JsonConfig.Apply(File.ReadAllText(configPath), config);
                if (PlayerPrefs.HasKey(ScalePreference)) config.scale = PlayerPrefs.GetFloat(ScalePreference);
                if (PlayerPrefs.HasKey(CenterXPreference)) config.centerX = PlayerPrefs.GetFloat(CenterXPreference);
                if (PlayerPrefs.HasKey(CenterYPreference)) config.centerY = PlayerPrefs.GetFloat(CenterYPreference);
                if (PlayerPrefs.HasKey(EnabledPreference)) config.enabled = PlayerPrefs.GetInt(EnabledPreference) != 0;
            }
            catch (Exception e) { Debug.LogWarning("[DonChanActionTheater] Config error: " + e.Message); }
            if (library == null) library = new AnimationLibrary(Directory.GetParent(Directory.GetParent(configPath).FullName).FullName);
            return library.Reload();
        }

        private static int Priority(ActionTopic topic)
        {
            switch (topic)
            {
                case ActionTopic.Healing: return 5;
                case ActionTopic.Food: return 4;
                case ActionTopic.Craft: return 3;
                case ActionTopic.Melee: return 2;
                case ActionTopic.Vehicle: return 6;
                default: return 1;
            }
        }

        private AnimationVariant Previous(ActionTopic topic)
        {
            switch (topic)
            {
                case ActionTopic.Food: return lastFood;
                case ActionTopic.Healing: return lastHealing;
                case ActionTopic.Craft: return lastCraft;
                case ActionTopic.Melee: return lastMelee;
                case ActionTopic.Vehicle: return lastVehicle;
                default: return lastRanged;
            }
        }

        private void Remember(ActionTopic topic, AnimationVariant value)
        {
            switch (topic)
            {
                case ActionTopic.Food: lastFood = value; break;
                case ActionTopic.Healing: lastHealing = value; break;
                case ActionTopic.Craft: lastCraft = value; break;
                case ActionTopic.Melee: lastMelee = value; break;
                case ActionTopic.Vehicle: lastVehicle = value; break;
                default: lastRanged = value; break;
            }
        }
    }
}
