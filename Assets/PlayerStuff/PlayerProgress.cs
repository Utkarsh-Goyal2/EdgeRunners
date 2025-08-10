using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerProgress
{
    public int clearedLevels = 0;
    public List<string> unlockedItemIds = new List<string>();
    public string equippedGunId = null; // For storing equipped gun
}

public static class ProgressService
{
    private const string Key = "PLAYER_PROGRESS_V3";
    private static PlayerProgress _cache;

    private static PlayerProgress Load()
    {
        if (_cache != null) return _cache;

        if (PlayerPrefs.HasKey(Key))
        {
            try
            {
                var json = PlayerPrefs.GetString(Key);
                _cache = JsonUtility.FromJson<PlayerProgress>(json);
            }
            catch
            {
                _cache = new PlayerProgress();
                Save();
            }
        }
        else
        {
            _cache = new PlayerProgress();
            Save();
        }
        return _cache;
    }

    public static void Save()
    {
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(Load()));
        PlayerPrefs.Save();
    }

    public static int ClearedLevels => Load().clearedLevels;

    public static string EquippedGunId => Load().equippedGunId;

    public static void EquipGun(string itemId)
    {
        var p = Load();
        p.equippedGunId = itemId;
        Save();
    }

    public static bool IsItemUnlocked(string itemId, int requiredLevel)
    {
        var p = Load();
        return p.unlockedItemIds.Contains(itemId) || p.clearedLevels >= requiredLevel;
    }

    public static void IncrementClearedLevelsAndAutoUnlock(Database db)
    {
        var p = Load();
        string currentScene = SceneManager.GetActiveScene().name;
        int level = 0;
        if (currentScene.StartsWith("level"))
        {
            string numberPart = currentScene.Substring(5);
            if (int.TryParse(numberPart, out int levelNum))
                level = levelNum;
        }
        p.clearedLevels = (p.clearedLevels > level) ? p.clearedLevels : level;
        AutoUnlock(db);
        Save();
    }

    public static void AutoUnlock(Database db)
    {
        var p = Load();
        foreach (var item in db.allItems)
        {
            if (item == null) continue;
            if (p.clearedLevels >= item.requiredLevel && !p.unlockedItemIds.Contains(item.itemId))
            {
                p.unlockedItemIds.Add(item.itemId);
            }
        }
    }
}
