using HarmonyLib;

namespace QuietQuestDistance
{
    [HarmonyPatch(typeof(XUiC_OnScreenIcons.OnScreenIcon), nameof(XUiC_OnScreenIcons.OnScreenIcon.Update))]
    internal static class QuestDistancePatch
    {
        [HarmonyPostfix]
        private static void Postfix(XUiC_OnScreenIcons.OnScreenIcon __instance)
        {
            if (__instance == null || __instance.Label == null) return;
            if (!QuietQuestDistanceRuntime.ShouldHide(__instance.NavObject)) return;

            // The original Update has already positioned the quest marker and
            // populated its distance. Clear only the label; preserve the sprite.
            __instance.Label.text = "";
        }
    }
}
