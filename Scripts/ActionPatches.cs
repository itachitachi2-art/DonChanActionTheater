using HarmonyLib;

namespace DonChanActionTheater
{
    // ItemActionEat is also used by medical consumables. Trigger only when the
    // private consume step completes, not when the use button is first pressed.
    [HarmonyPatch(typeof(ItemActionEat), "consume")]
    internal static class ConsumableCompletePatch
    {
        private static void Postfix(ItemActionData _actionData)
        {
            if (!ActionClassifier.IsLocal(_actionData) || ActionTheaterRuntime.Instance == null) return;
            string item = ActionClassifier.ItemName(_actionData);
            ActionTopic topic = ActionClassifier.ConsumableTopic(item);
            ActionTheaterRuntime.Instance.Trigger(topic, "Generic", item);
        }
    }

    // V3.2 keeps two independent melee action implementations. Some legacy or
    // modded items use ItemActionMelee, while current vanilla weapons primarily
    // use ItemActionDynamicMelee. Patch both entry points.
    [HarmonyPatch(typeof(ItemActionMelee), nameof(ItemActionMelee.ExecuteAction))]
    internal static class MeleeActionPatch
    {
        private static void Postfix(ItemActionData _actionData, bool _bReleased)
        {
            MeleeActionTrigger.TryTrigger(_actionData, _bReleased);
        }
    }

    [HarmonyPatch(typeof(ItemActionDynamicMelee), nameof(ItemActionDynamicMelee.ExecuteAction))]
    internal static class DynamicMeleeActionPatch
    {
        private static void Postfix(ItemActionData _actionData, bool _bReleased)
        {
            MeleeActionTrigger.TryTrigger(_actionData, _bReleased);
        }
    }

    internal static class MeleeActionTrigger
    {
        internal static void TryTrigger(ItemActionData actionData, bool released)
        {
            if (released || !ActionClassifier.IsLocal(actionData) || ActionTheaterRuntime.Instance == null) return;
            string item = ActionClassifier.ItemName(actionData);
            ActionTheaterRuntime.Instance.Trigger(ActionTopic.Melee, ActionClassifier.MeleeSubtype(item), item);
        }
    }

    // This callback occurs after an actual shot. It avoids animations for an
    // empty magazine or a click rejected by another mod.
    [HarmonyPatch(typeof(ItemActionRanged), "onHoldingEntityFired")]
    internal static class RangedFiredPatch
    {
        private static void Postfix(ItemActionData _actionData)
        {
            if (!ActionClassifier.IsLocal(_actionData) || ActionTheaterRuntime.Instance == null) return;
            string item = ActionClassifier.ItemName(_actionData);
            if (ActionClassifier.IsChainsaw(item))
            {
                ActionTheaterRuntime.Instance.Trigger(ActionTopic.Melee, "Chainsaw", item);
                return;
            }
            ActionTheaterRuntime.Instance.Trigger(ActionTopic.Ranged, ActionClassifier.RangedSubtype(item), item);
        }
    }

    [HarmonyPatch(typeof(XUiC_RecipeStack), "outputStack")]
    internal static class CraftCompletePatch
    {
        private static void Postfix(bool __result)
        {
            if (!__result || ActionTheaterRuntime.Instance == null) return;
            ActionTheaterRuntime.Instance.Trigger(ActionTopic.Craft, "Generic", "craft");
        }
    }

    [HarmonyPatch(typeof(EntityVehicle), nameof(EntityVehicle.EnterVehicle))]
    internal static class VehicleEnteredPatch
    {
        private static void Postfix(EntityVehicle __instance, EntityAlive _entity)
        {
            if (__instance == null || !(_entity is EntityPlayerLocal) || ActionTheaterRuntime.Instance == null) return;
            if (_entity.AttachedToEntity != __instance) return;
            string subtype = ActionClassifier.VehicleSubtype(__instance);
            ActionTheaterRuntime.Instance.Trigger(ActionTopic.Vehicle, subtype, __instance.GetType().Name);
        }
    }
}
