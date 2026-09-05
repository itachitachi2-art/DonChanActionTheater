namespace DonChanActionTheater
{
    internal enum ActionTopic
    {
        Food,
        Healing,
        Craft,
        Melee,
        Ranged,
        Vehicle
    }

    internal static class ActionClassifier
    {
        internal static string ItemName(ItemActionData data)
        {
            if (data == null || data.invData == null || data.invData.item == null) return "generic";
            return data.invData.item.GetItemName() ?? "generic";
        }

        internal static bool IsLocal(ItemActionData data)
        {
            return data != null && data.invData != null && data.invData.holdingEntity is EntityPlayerLocal;
        }

        internal static ActionTopic ConsumableTopic(string itemName)
        {
            string n = Normalize(itemName);
            if (ContainsAny(n, "bandage", "firstaid", "medical", "medkit", "painkiller", "antibiotic", "vitamin", "splint", "cast", "steroid", "aloe", "heal"))
                return ActionTopic.Healing;
            return ActionTopic.Food;
        }

        internal static string MeleeSubtype(string itemName)
        {
            string n = Normalize(itemName);
            if (ContainsAny(n, "chainsaw")) return "Chainsaw";
            if (ContainsAny(n, "knife", "machete", "blade")) return "Knife";
            if (ContainsAny(n, "spear", "javelin")) return "Spear";
            if (ContainsAny(n, "sledge")) return "Sledge";
            if (ContainsAny(n, "baton", "stun")) return "Baton";
            if (ContainsAny(n, "shovel", "spade")) return "Shovel";
            if (ContainsAny(n, "pickaxe")) return "Pickaxe";
            if (ContainsAny(n, "axe")) return "Axe";
            if (ContainsAny(n, "knuckle", "fist", "hand")) return "Fist";
            if (ContainsAny(n, "club", "bat")) return "Club";
            return "Generic";
        }

        internal static string RangedSubtype(string itemName)
        {
            string n = Normalize(itemName);
            if (ContainsAny(n, "desertvulture", "deserteagle", "deagle")) return "DesertVulture";
            if (ContainsAny(n, "magnum", "revolver")) return "Magnum";
            if (ContainsAny(n, "crossbow")) return "Crossbow";
            if (ContainsAny(n, "bow")) return "Bow";
            if (ContainsAny(n, "shotgun")) return "Shotgun";
            if (ContainsAny(n, "smg", "submachine")) return "SMG";
            if (ContainsAny(n, "rifle", "ak47", "m60", "machinegun")) return "Rifle";
            if (ContainsAny(n, "rocket", "launcher")) return "Launcher";
            if (ContainsAny(n, "pistol")) return "Pistol";
            return "Generic";
        }

        internal static bool IsChainsaw(string itemName)
        {
            return Normalize(itemName).Contains("chainsaw");
        }

        internal static string VehicleSubtype(EntityVehicle vehicle)
        {
            if (vehicle is EntityBicycle) return "Bicycle";
            if (vehicle is EntityMinibike) return "Minibike";
            if (vehicle is EntityMotorcycle) return "Motorcycle";
            if (vehicle is EntityVJeep) return "Jeep";
            if (vehicle is EntityVGyroCopter) return "Gyrocopter";
            return "Generic";
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value.Replace("_", "").Replace(" ", "").ToLowerInvariant();
        }

        private static bool ContainsAny(string value, params string[] words)
        {
            for (int i = 0; i < words.Length; i++) if (value.Contains(words[i])) return true;
            return false;
        }
    }
}
