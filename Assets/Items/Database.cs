using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Game/Database")]
public class Database : ScriptableObject
{
    public List<Items> allItems = new List<Items>();


    // Add this method to your Database class
    public Items GetItemById(string itemId)
    {
        foreach (var item in allItems)
        {
            if (item != null && item.itemId == itemId)
                return item;
        }
        return null;
    }

}