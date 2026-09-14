using UnityEngine;

namespace QuietQuestDistance
{
    public sealed class QuietQuestDistanceMod : IModApi
    {
        public void InitMod(Mod mod)
        {
            QuietQuestDistanceRuntime.Create(mod.Path);
            Debug.Log("[QuietQuestDistance] 0.1.0 loaded");
        }
    }
}
