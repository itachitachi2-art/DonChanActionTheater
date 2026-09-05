using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DonChanActionTheater
{
    internal static class JsonConfig
    {
        internal static float Float(string json, string key, float fallback)
        {
            Match match = Regex.Match(json ?? "", "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*(-?[0-9]+(?:\\.[0-9]+)?)", RegexOptions.IgnoreCase);
            float value;
            return match.Success && float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                ? value : fallback;
        }

        internal static bool Bool(string json, string key, bool fallback)
        {
            Match match = Regex.Match(json ?? "", "\\\"" + Regex.Escape(key) + "\\\"\\s*:\\s*(true|false)", RegexOptions.IgnoreCase);
            bool value;
            return match.Success && bool.TryParse(match.Groups[1].Value, out value) ? value : fallback;
        }

        internal static void Apply(string json, AnimationSettings settings)
        {
            settings.fps = Float(json, "fps", settings.fps);
            settings.loop = Bool(json, "loop", settings.loop);
            settings.holdLastFrame = Float(json, "holdLastFrame", settings.holdLastFrame);
            settings.weight = Float(json, "weight", settings.weight);
            settings.displaySeconds = Float(json, "displaySeconds", settings.displaySeconds);
            settings.repeatCount = Float(json, "repeatCount", settings.repeatCount);
        }

        internal static void Apply(string json, TheaterConfig config)
        {
            config.enabled = Bool(json, "enabled", config.enabled);
            config.width = Float(json, "width", config.width);
            config.height = Float(json, "height", config.height);
            config.scale = Float(json, "scale", config.scale);
            config.centerX = Float(json, "centerX", config.centerX);
            config.centerY = Float(json, "centerY", config.centerY);
            config.opacity = Float(json, "opacity", config.opacity);
            config.sequenceLockSeconds = Float(json, "sequenceLockSeconds", config.sequenceLockSeconds);
        }
    }
}
