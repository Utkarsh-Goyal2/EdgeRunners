using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Game/Database")]
public class Database : ScriptableObject
{
    public List<Items> allItems = new List<Items>();

    
public Items GetById(string id)
    {
        return allItems.Find(i => i != null && i.itemId == id);
    }
}