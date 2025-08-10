using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class Items : ScriptableObject
{
    [Header("Display")]
    public string itemId; // unique key
    public string itemName;
    public Sprite icon;


    [Header("Type")]
    public ItemType itemType = ItemType.Gun;
    public enum ItemType { Gun, Armor, Other }

    [Header("Gun Stats (only if Gun)")]
    public GameObject bulletPrefab;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletMaxDistance;
    public float gunCoolDown;
    public float sizeMultiplier = 1f;
    [Header("Armor Stats (only if Armor)")]
    public float hpIncreaseFlat;        // e.g., +25 HP
    [Range(0, 1f)]
    public float damageReductionPct;    // e.g., 0.2 means 20% reduction
    [Header("Progression")]
    public int requiredLevel = 0;

    // Computed; not serialized in Inspector
    public bool IsUnlocked => ProgressService.IsItemUnlocked(itemId, requiredLevel);
}