using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    public Database database;
    public EquipmentManager equipmentManager;

    [Header("Item List")]
    public Transform contentParent;
    public GameObject itemButtonPrefab;

    [Header("Details Panels")]
    public GameObject weaponDetailsPanel;
    public GameObject armorDetailsPanel;
    public GameObject detailsLockBadge;

    [Header("Equipment Buttons")]
    public Button equipButton; // For armor only
    public Button slot1Button; // Separate button for gun slot 1
    public Button slot2Button; // Separate button for gun slot 2

    [Header("Weapon Details UI - TMP Only")]
    public TMP_Text damage;
    public TMP_Text speed;
    public TMP_Text range;
    public TMP_Text cooldown;

    [Header("Armor Details UI - TMP Only")]
    public TMP_Text hpIncrease;
    public TMP_Text damageReduction;

    private Items currentSelectedItem;

    void Start()
    {
        BuildList();

        // Initially hide both detail panels
        if (weaponDetailsPanel) weaponDetailsPanel.SetActive(false);
        if (armorDetailsPanel) armorDetailsPanel.SetActive(false);

        // Initially hide all equipment buttons
        if (equipButton) equipButton.gameObject.SetActive(false);
        if (slot1Button) slot1Button.gameObject.SetActive(false);
        if (slot2Button) slot2Button.gameObject.SetActive(false);

        // Set up slot button listeners
        if (slot1Button) slot1Button.onClick.AddListener(() => EquipToSlot(1));
        if (slot2Button) slot2Button.onClick.AddListener(() => EquipToSlot(2));
    }

    void BuildList()
    {
        // Clear existing items
        foreach (Transform c in contentParent) Destroy(c.gameObject);

        foreach (var item in database.allItems)
        {
            if (item == null) continue;

            var go = Instantiate(itemButtonPrefab, contentParent);

            // Set up item display
            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            var name = go.transform.Find("Name")?.GetComponent<TMP_Text>();
            if (icon) icon.sprite = item.icon;
            if (name) name.text = item.itemName;

            bool unlocked = ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel);

            // Disable interaction if locked
            var btn = go.GetComponent<Button>();
            if (btn) btn.interactable = unlocked;

            // Optional visuals for locked items
            var cg = go.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = unlocked ? 1f : 0.5f;

            var lockBadge = go.transform.Find("LockBadge")?.gameObject;
            if (lockBadge) lockBadge.SetActive(!unlocked);

            // Add click listener for unlocked items
            if (btn && unlocked)
            {
                Items captured = item;
                btn.onClick.AddListener(() => ShowDetails(captured));
            }
        }
    }

    void ShowDetails(Items item)
    {
        currentSelectedItem = item;

        // Hide both panels first
        if (weaponDetailsPanel) weaponDetailsPanel.SetActive(false);
        if (armorDetailsPanel) armorDetailsPanel.SetActive(false);

        bool isUnlocked = ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel);

        if (item.itemType == Items.ItemType.Gun)
        {
            ShowWeaponDetails(item);
            SetupWeaponEquipButtons(isUnlocked);
        }
        else if (item.itemType == Items.ItemType.Armor)
        {
            ShowArmorDetails(item);
            SetupArmorEquipButton(isUnlocked);
        }

        // Show/hide lock badge
        if (detailsLockBadge) detailsLockBadge.SetActive(!isUnlocked);
    }

    void ShowWeaponDetails(Items weapon)
    {
        if (weaponDetailsPanel) weaponDetailsPanel.SetActive(true);

        // Set weapon stats in TMP components
        if (damage) damage.text = $"Damage: {weapon.bulletDamage:0.##}";
        if (speed) speed.text = $"Speed: {weapon.bulletSpeed:0.##}";
        if (range) range.text = $"Range: {weapon.bulletMaxDistance:0.##}";
        if (cooldown) cooldown.text = $"Cooldown: {weapon.gunCoolDown:0.##}s";
    }

    void ShowArmorDetails(Items armor)
    {
        if (armorDetailsPanel) armorDetailsPanel.SetActive(true);

        // Set armor stats in TMP components
        if (hpIncrease) hpIncrease.text = $"HP Increase: +{armor.hpIncreaseFlat:0.##}";
        if (damageReduction) damageReduction.text = $"Damage Reduction: {(armor.damageReductionPct * 100f):0.#}%";
    }

    void SetupWeaponEquipButtons(bool isUnlocked)
    {
        // Hide armor equip button for weapons
        if (equipButton) equipButton.gameObject.SetActive(false);

        // Show weapon slot buttons
        if (slot1Button)
        {
            slot1Button.gameObject.SetActive(true);
            slot1Button.interactable = isUnlocked;
        }

        if (slot2Button)
        {
            slot2Button.gameObject.SetActive(true);
            slot2Button.interactable = isUnlocked;
        }
    }

    void SetupArmorEquipButton(bool isUnlocked)
    {
        // Hide weapon slot buttons for armor
        if (slot1Button) slot1Button.gameObject.SetActive(false);
        if (slot2Button) slot2Button.gameObject.SetActive(false);

        // Show single equip button for armor
        if (equipButton)
        {
            equipButton.gameObject.SetActive(true);
            equipButton.interactable = isUnlocked;

            // Clear previous listeners and add new one for armor
            equipButton.onClick.RemoveAllListeners();
            if (isUnlocked)
            {
                equipButton.onClick.AddListener(() => EquipCurrentArmor());
            }
        }
    }

    void EquipToSlot(int slot)
    {
        if (currentSelectedItem != null && currentSelectedItem.itemType == Items.ItemType.Gun && equipmentManager != null)
        {
            equipmentManager.EquipWeaponToSlot(currentSelectedItem, slot);
            Debug.Log($"Equipped {currentSelectedItem.itemName} to slot {slot}");
        }
    }

    void EquipCurrentArmor()
    {
        if (currentSelectedItem != null && currentSelectedItem.itemType == Items.ItemType.Armor && equipmentManager != null)
        {
            equipmentManager.EquipItem(currentSelectedItem);
        }
    }
}
