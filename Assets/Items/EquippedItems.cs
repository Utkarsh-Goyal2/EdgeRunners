using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorSlot
{
    public string armorType;
    public string itemId;

    public ArmorSlot(string type, string id)
    {
        armorType = type;
        itemId = id;
    }
}

[System.Serializable]
public class EquippedItems
{
    public string equippedGun1Id = null;
    public string equippedGun2Id = null;
    public ArmorSlot[] equippedArmor = new ArmorSlot[]
    {
        new ArmorSlot("Helmet", null),
        new ArmorSlot("Chestplate", null),
        new ArmorSlot("Leggings", null),
        new ArmorSlot("Boots", null)
    };

    // Currently active gun (1 or 2)
    public int activeGunSlot = 1;
}

public static class EquipmentService
{
    private const string Key = "EQUIPPED_ITEMS_V1";
    private static EquippedItems _cache;

    private static EquippedItems Load()
    {
        if (_cache != null) return _cache;

        if (PlayerPrefs.HasKey(Key))
        {
            try
            {
                var json = PlayerPrefs.GetString(Key);
                _cache = JsonUtility.FromJson<EquippedItems>(json);

                // Initialize armor array if null (backwards compatibility)
                if (_cache.equippedArmor == null || _cache.equippedArmor.Length == 0)
                {
                    _cache.equippedArmor = new ArmorSlot[]
                    {
                        new ArmorSlot("Helmet", null),
                        new ArmorSlot("Chestplate", null),
                        new ArmorSlot("Leggings", null),
                        new ArmorSlot("Boots", null)
                    };
                }
            }
            catch
            {
                _cache = new EquippedItems();
                Save();
            }
        }
        else
        {
            _cache = new EquippedItems();
            Save();
        }

        return _cache;
    }

    public static void Save()
    {
        var json = JsonUtility.ToJson(Load());
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();

        Debug.Log($"Equipment saved: {json}"); // Debug line to verify saving
    }

    // Gun equipment methods (unchanged)
    public static void EquipGun(string itemId, int slot)
    {
        var equipment = Load();
        if (slot == 1)
            equipment.equippedGun1Id = itemId;
        else if (slot == 2)
            equipment.equippedGun2Id = itemId;
        Save();
    }

    public static string GetEquippedGun(int slot)
    {
        var equipment = Load();
        return slot == 1 ? equipment.equippedGun1Id : equipment.equippedGun2Id;
    }

    public static void SetActiveGunSlot(int slot)
    {
        var equipment = Load();
        equipment.activeGunSlot = slot;
        Save();
    }

    public static int GetActiveGunSlot() => Load().activeGunSlot;

    public static string GetActiveGunId()
    {
        var equipment = Load();
        return equipment.activeGunSlot == 1 ? equipment.equippedGun1Id : equipment.equippedGun2Id;
    }

    // Updated armor equipment methods
    public static void EquipArmor(string itemId, string armorType)
    {
        var equipment = Load();

        // Find the armor slot and update it
        for (int i = 0; i < equipment.equippedArmor.Length; i++)
        {
            if (equipment.equippedArmor[i].armorType == armorType)
            {
                equipment.equippedArmor[i].itemId = itemId;
                break;
            }
        }

        Save();
        Debug.Log($"Equipped armor: {armorType} = {itemId}"); // Debug line
    }

    public static string GetEquippedArmor(string armorType)
    {
        var equipment = Load();

        // Find the armor slot and return its itemId
        for (int i = 0; i < equipment.equippedArmor.Length; i++)
        {
            if (equipment.equippedArmor[i].armorType == armorType)
            {
                return equipment.equippedArmor[i].itemId;
            }
        }

        return null;
    }

    // Clear cache (useful for testing)
    public static void ClearCache()
    {
        _cache = null;
    }

    // Reset all equipment (useful for testing)
    public static void ResetAllEquipment()
    {
        PlayerPrefs.DeleteKey(Key);
        _cache = null;
        Debug.Log("All equipment data reset!");
    }
}
