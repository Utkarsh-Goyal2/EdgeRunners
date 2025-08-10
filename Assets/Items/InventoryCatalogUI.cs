using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Database database;
    public Transform contentParent; // ScrollView > Viewport > Content
    public GameObject itemButtonPrefab; // has Button, Icon(Image), Name(Text), optional LockBadge
    public Image detailIcon;
    public TMP_Text detailName, detailDamage, detailSpeed, detailMaxDistance;
    public TMP_Text detailHpIncrease, detailDamageReduction;
    public GameObject detailsLockBadge;
    public Button equipButton;

    
void Start()
    {
        BuildList();
       
    }

    void BuildList()
    {
        foreach (Transform c in contentParent) Destroy(c.gameObject);

        foreach (var item in database.allItems)
        {
            if (item == null) continue;

            var go = Instantiate(itemButtonPrefab, contentParent);

            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            var name = go.transform.Find("Name")?.GetComponent<TMP_Text>();
            if (icon) icon.sprite = item.icon;
            if (name) name.text = item.itemName;

            bool unlocked = ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel);

            // Disable interaction if locked
            var btn = go.GetComponent<Button>();
            if (btn) btn.interactable = unlocked;

            // Optional visuals
            var cg = go.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = unlocked ? 1f : 0.5f;

            var lockBadge = go.transform.Find("LockBadge")?.gameObject;
            if (lockBadge) lockBadge.SetActive(!unlocked);

            if (btn && unlocked)
            {
                Items captured = item;
                btn.onClick.AddListener(() => ShowDetails(captured));
            }
        }
    }

    void ShowDetails(Items item)
    {
        if (detailIcon) detailIcon.sprite = item.icon;
        if (detailName) detailName.text = item.itemName;

        // Clear fields
        if (detailDamage) detailDamage.text = "";
        if (detailSpeed) detailSpeed.text = "";
        if (detailMaxDistance) detailMaxDistance.text = "";
        if (detailHpIncrease) detailHpIncrease.text = "";
        if (detailDamageReduction) detailDamageReduction.text = "";

        // Populate based on type
        if (item.itemType == Items.ItemType.Gun)
        {
            if (detailDamage) detailDamage.text = $"Damage: {item.bulletDamage:0.##}";
            if (detailSpeed) detailSpeed.text = $"Speed: {item.bulletSpeed:0.##}";
            if (detailMaxDistance) detailMaxDistance.text = $"Max Distance: {item.bulletMaxDistance:0.##}";
        }
        else if (item.itemType == Items.ItemType.Armor)
        {
            if (detailHpIncrease) detailHpIncrease.text = $"HP +{item.hpIncreaseFlat:0.##}";
            if (detailDamageReduction) detailDamageReduction.text = $"Damage Reduction: {(item.damageReductionPct * 100f):0.#}%";
        }

        if (detailsLockBadge) detailsLockBadge.SetActive(!ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel));

        if (equipButton)
        {
            bool canEquip = item.itemType == Items.ItemType.Gun && ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel);
            equipButton.gameObject.SetActive(true);
            equipButton.interactable = canEquip;
        }
    }
}