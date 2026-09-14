using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace QuietQuestDistance
{
    [Serializable]
    internal sealed class QuietQuestDistanceConfig
    {
        public string toggleKey = "F8";
        public bool hiddenByDefault = true;
    }

    internal sealed class QuietQuestDistanceRuntime : MonoBehaviour
    {
        private const string HiddenPreference = "QuietQuestDistance.Hidden";

        private static readonly string[] TargetNavObjectClasses =
        {
            "quest",
            "rally",
            "go_to_trader",
            "return_to_trader",
            "quest_switch"
        };

        internal static QuietQuestDistanceRuntime Instance { get; private set; }

        private readonly Dictionary<NavObjectScreenSettings, NavObjectScreenSettings.ShowTextTypes>
            originalTextTypes =
                new Dictionary<NavObjectScreenSettings, NavObjectScreenSettings.ShowTextTypes>();

        private QuietQuestDistanceConfig config = new QuietQuestDistanceConfig();
        private string configPath;
        private KeyCode toggleKey = KeyCode.F8;
        private bool hidden;
        private float nextApplyAt;
        private string notice = "";
        private float noticeUntil;

        internal static void Create(string modPath)
        {
            if (Instance != null) return;

            GameObject gameObject = new GameObject("QuietQuestDistance.Runtime");
            DontDestroyOnLoad(gameObject);
            Instance = gameObject.AddComponent<QuietQuestDistanceRuntime>();
            Instance.Initialize(modPath);
        }

        private void Initialize(string modPath)
        {
            configPath = Path.Combine(modPath, "Config", "quiet-quest-distance.json");
            ReloadConfig();

            hidden = PlayerPrefs.HasKey(HiddenPreference)
                ? PlayerPrefs.GetInt(HiddenPreference) != 0
                : config.hiddenByDefault;

            ApplyDesiredState();
            Debug.Log("[QuietQuestDistance] Quest distance labels: " + (hidden ? "hidden" : "visible")
                + "; toggle key: " + toggleKey);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                SetHidden(!hidden);
            }

            // Nav object classes can be rebuilt while loading a world or reloading
            // XML. Reapply occasionally so the selected state survives that cycle.
            if (Time.unscaledTime >= nextApplyAt)
            {
                ApplyDesiredState();
                nextApplyAt = Time.unscaledTime + 0.5f;
            }
        }

        private void OnGUI()
        {
            if (Time.unscaledTime >= noticeUntil || string.IsNullOrEmpty(notice)) return;

            const float width = 330f;
            const float height = 42f;
            Rect rect = new Rect((Screen.width - width) * 0.5f, 70f, width, height);
            GUI.Box(rect, notice);
        }

        private void OnDestroy()
        {
            RestoreOriginalSettings();
            if (Instance == this) Instance = null;
        }

        internal string RunCommand(string action)
        {
            string command = (action ?? "").Trim().ToLowerInvariant();
            switch (command)
            {
                case "hide":
                case "off":
                    SetHidden(true);
                    return Status();
                case "show":
                case "on":
                    SetHidden(false);
                    return Status();
                case "toggle":
                    SetHidden(!hidden);
                    return Status();
                case "reload":
                    ReloadConfig();
                    ApplyDesiredState();
                    return Status() + "; config reloaded";
                case "status":
                case "":
                    return Status();
                default:
                    return "qqd hide | show | toggle | status | reload";
            }
        }

        private string Status()
        {
            return "Quiet Quest Distance: labels " + (hidden ? "HIDDEN" : "VISIBLE")
                + "; key " + toggleKey;
        }

        private void SetHidden(bool value)
        {
            hidden = value;
            PlayerPrefs.SetInt(HiddenPreference, hidden ? 1 : 0);
            PlayerPrefs.Save();
            ApplyDesiredState();

            notice = hidden ? "QUEST DISTANCE: HIDDEN" : "QUEST DISTANCE: VISIBLE";
            noticeUntil = Time.unscaledTime + 1.5f;
            Debug.Log("[QuietQuestDistance] " + notice);
        }

        private void ApplyDesiredState()
        {
            foreach (string className in TargetNavObjectClasses)
            {
                NavObjectClass navClass = NavObjectClass.GetNavObjectClass(className);
                if (navClass == null) continue;

                ApplyToSettings(navClass.OnScreenSettings);
                ApplyToSettings(navClass.InactiveOnScreenSettings);
            }

            if (!hidden) originalTextTypes.Clear();
        }

        private void ApplyToSettings(NavObjectScreenSettings settings)
        {
            if (settings == null) return;

            if (hidden)
            {
                if (!originalTextTypes.ContainsKey(settings))
                    originalTextTypes.Add(settings, settings.ShowTextType);

                settings.ShowTextType = NavObjectScreenSettings.ShowTextTypes.None;
                return;
            }

            NavObjectScreenSettings.ShowTextTypes original;
            if (originalTextTypes.TryGetValue(settings, out original))
                settings.ShowTextType = original;
        }

        private void RestoreOriginalSettings()
        {
            foreach (KeyValuePair<NavObjectScreenSettings, NavObjectScreenSettings.ShowTextTypes> pair
                in originalTextTypes)
            {
                if (pair.Key != null) pair.Key.ShowTextType = pair.Value;
            }
            originalTextTypes.Clear();
        }

        private void ReloadConfig()
        {
            config = new QuietQuestDistanceConfig();

            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    config.toggleKey = ReadString(json, "toggleKey", config.toggleKey);
                    config.hiddenByDefault = ReadBool(json, "hiddenByDefault", config.hiddenByDefault);
                }

                KeyCode parsed;
                if (!Enum.TryParse(config.toggleKey, true, out parsed))
                {
                    Debug.LogWarning("[QuietQuestDistance] Invalid toggleKey '" + config.toggleKey
                        + "'; using F8.");
                    parsed = KeyCode.F8;
                }

                toggleKey = parsed;
            }
            catch (Exception exception)
            {
                toggleKey = KeyCode.F8;
                Debug.LogWarning("[QuietQuestDistance] Config error; using defaults: " + exception.Message);
            }
        }

        private static string ReadString(string json, string key, string fallback)
        {
            Match match = Regex.Match(
                json ?? "",
                "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*\\\"([^\\\"]+)\\\"",
                RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : fallback;
        }

        private static bool ReadBool(string json, string key, bool fallback)
        {
            Match match = Regex.Match(
                json ?? "",
                "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*(true|false)",
                RegexOptions.IgnoreCase);
            bool value;
            return match.Success && bool.TryParse(match.Groups[1].Value, out value) ? value : fallback;
        }
    }
}
