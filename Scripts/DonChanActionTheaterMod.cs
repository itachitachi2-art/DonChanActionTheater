using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace DonChanActionTheater
{
    public sealed class DonChanActionTheaterMod : IModApi
    {
        public void InitMod(Mod mod)
        {
            ActionTheaterRuntime.Create(mod.Path);
            new Harmony("itachi.donchanactiontheater").PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("[DonChanActionTheater] 0.1.0 prototype loaded");
        }
    }
}
