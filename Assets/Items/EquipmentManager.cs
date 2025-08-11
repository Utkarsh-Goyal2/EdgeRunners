using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipped Weapon Slots")]
    public Image gun1;
    public Image gun2;

    [Header("Equipped Armor Images")]
    public Image equippedHelmet;
    public Image equippedChestplate;
    public Image equippedLeggings;
    public Image equippedBoots;

    [Header("Database Reference")]
    public Database database;

    [Header("Player Reference")]
    public PlayerController playerController; // Assign in Inspector

    void Start()
    {
        LoadEquippedItems();
    }

    public void EquipItem(Items item)
    {
        if (item.itemType == Items.ItemType.Gun)
        {
            // For guns, we'll now wait for slot selection instead of auto-assigning
            // This method will be called from InventoryUI with slot selection
            EquipWeapon(item, 1); // Default to slot 1, but this will be overridden
        }
        else if (item.itemType == Items.ItemType.Armor)
        {
            EquipArmor(item);
        }
    }

    // New method to equip weapon to specific slot
    public void EquipWeaponToSlot(Items weapon, int slot)
    {
        EquipmentService.EquipGun(weapon.itemId, slot);
        UpdateWeaponUI();

        // Notify player controller to recalculate if needed
        if (playerController != null)
        {
            // If the equipped weapon is the currently active one, update it
            if (EquipmentService.GetActiveGunSlot() == slot)
            {
                // Force player to re-setup the gun
                playerController.OnWeaponEquipmentChanged();
            }
        }

        Debug.Log($"Equipped weapon: {weapon.itemName} to slot {slot}");
    }

    private void EquipWeapon(Items weapon, int slot = 1)
    {
        EquipWeaponToSlot(weapon, slot);
    }

    private void EquipArmor(Items armor)
    {
        string armorType = armor.armorSubtype.ToString();
        EquipmentService.EquipArmor(armor.itemId, armorType);
        UpdateArmorUI();

        // Notify player controller to recalculate armor bonuses
        if (playerController != null)
        {
            playerController.OnArmorEquipmentChanged();
        }

        Debug.Log($"Equipped {armor.armorSubtype}: {armor.itemName}");
    }

    private void LoadEquippedItems()
    {
        UpdateWeaponUI();
        UpdateArmorUI();
    }

    private void UpdateWeaponUI()
    {
        // Update gun1
        string gun1Id = EquipmentService.GetEquippedGun(1);
        UpdateGunSlot(gun1, gun1Id);

        // Update gun2
        string gun2Id = EquipmentService.GetEquippedGun(2);
        UpdateGunSlot(gun2, gun2Id);
    }

    private void UpdateGunSlot(Image gunImage, string gunId)
    {
        if (string.IsNullOrEmpty(gunId))
        {
            SetImageActive(gunImage, false);
            return;
        }

        Items gunItem = database.GetItemById(gunId);
        if (gunItem != null)
        {
            gunImage.sprite = gunItem.icon;
            SetImageActive(gunImage, true);
        }
        else
        {
            SetImageActive(gunImage, false);
        }
    }

    private void UpdateArmorUI()
    {
        UpdateArmorSlot(equippedHelmet, "Helmet");
        UpdateArmorSlot(equippedChestplate, "Chestplate");
        UpdateArmorSlot(equippedLeggings, "Leggings");
        UpdateArmorSlot(equippedBoots, "Boots");
    }

    private void UpdateArmorSlot(Image armorImage, string armorType)
    {
        string armorId = EquipmentService.GetEquippedArmor(armorType);
        if (string.IsNullOrEmpty(armorId))
        {
            SetImageActive(armorImage, false);
            return;
        }

        Items armorItem = database.GetItemById(armorId);
        if (armorItem != null)
        {
            armorImage.sprite = armorItem.icon;
            SetImageActive(armorImage, true);
        }
        else
        {
            SetImageActive(armorImage, false);
        }
    }

    private void SetImageActive(Image image, bool active)
    {
        if (image != null)
        {
            image.gameObject.SetActive(active);
        }
    }
}
