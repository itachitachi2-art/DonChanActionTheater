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

        private static readonly HashSet<string> TargetNavObjectClasses =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "quest",
                "rally",
                "go_to_trader",
                "return_to_trader",
                "quest_switch"
            };

        internal static QuietQuestDistanceRuntime Instance { get; private set; }
        internal static bool Hidden => Instance != null && Instance.hidden;

        private QuietQuestDistanceConfig config = new QuietQuestDistanceConfig();
        private string configPath;
        private KeyCode toggleKey = KeyCode.F8;
        private bool hidden;
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

            Debug.Log("[QuietQuestDistance] Quest distance labels: " + (hidden ? "hidden" : "visible")
                + "; toggle key: " + toggleKey);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                SetHidden(!hidden);
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

        internal static bool ShouldHide(NavObject navObject)
        {
            if (!Hidden || navObject == null || navObject.NavObjectClass == null) return false;

            NavObjectScreenSettings settings = navObject.CurrentScreenSettings;
            if (settings == null || settings.ShowTextType != NavObjectScreenSettings.ShowTextTypes.Distance)
                return false;

            return TargetNavObjectClasses.Contains(navObject.NavObjectClass.NavObjectClassName);
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

            notice = hidden ? "QUEST DISTANCE: HIDDEN" : "QUEST DISTANCE: VISIBLE";
            noticeUntil = Time.unscaledTime + 1.5f;
            Debug.Log("[QuietQuestDistance] " + notice);
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
                "\\"" + Regex.Escape(key) + "\\"\\s*:\\s*\\"([^\\"]+)\\"",
                RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : fallback;
        }

        private static bool ReadBool(string json, string key, bool fallback)
        {
            Match match = Regex.Match(
                json ?? "",
                "\\"" + Regex.Escape(key) + "\\"\\s*:\\s*(true|false)",
                RegexOptions.IgnoreCase);
            bool value;
            return match.Success && bool.TryParse(match.Groups[1].Value, out value) ? value : fallback;
        }
    }
}
