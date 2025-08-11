using UnityEngine;
using UnityEngine.UI;

public class InGameInventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image gun1Slot;
    public Image gun2Slot;

    [Header("Database")]
    public Database database;


    void Start()
    {
        UpdateInventoryDisplay();
    }


    public void UpdateInventoryDisplay()
    {
        // Update gun slots
        UpdateGunSlot(gun1Slot, EquipmentService.GetEquippedGun(1));
        UpdateGunSlot(gun2Slot, EquipmentService.GetEquippedGun(2));
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

    private void SetImageActive(Image image, bool active)
    {
        if (image != null)
        {
            image.gameObject.SetActive(active);
        }
    }
}
