using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public float defaultspeed = 5f;
    public float crouchingspeed = 2f;
    public float jumpForce = 7f;
    public float jumpcooldown = 1.5f;
    private float baseMaxHp = 100f; // Base HP without armor bonuses
    private float hp;

    [Header("UI Panels")]
    public GameObject youLostPanel;
    public Image hpFillImage;
    public GameObject youWonPanel;

    [Header("Gun Settings")]
    public Transform gunHolder;
    public Database database;
    public InGameInventoryUI inGameInventory;

    [Header("Shooting Settings")]
    public Transform bulletSpawnPoint;

    [Header("Gun Switching")]
    public float weaponSwitchCooldown = 2f;

    // Armor stats (calculated from equipped armor)
    private float totalHpBonus = 0f;
    private float totalDamageReduction = 0f;
    private float maxhp; // Dynamic max HP including armor bonuses

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private Animator animator;
    private Vector2 originalColliderSize;
    private Vector2 duckingColliderSize;
    private Vector2 originalOffset;
    private Vector2 crouchOffset;
    private float lastJumpTime = -Mathf.Infinity;
    private float lastShootTime = -Mathf.Infinity;
    private float lastWeaponSwitchTime = -Mathf.Infinity;
    private float moveInput;
    private float moveSpeed;
    private GameObject currentGunObject;
    private Items currentGunItem;
    private bool weaponSwitching = false;
    private float lastdamagetaken = 0;
    public bool isFacingLeft { get; private set; }
    private float healamount = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        originalColliderSize = capsuleCollider.size;
        duckingColliderSize = new Vector2(originalColliderSize.x, originalColliderSize.y / 3);
        originalOffset = capsuleCollider.offset;
        crouchOffset = new Vector2(originalOffset.x, originalOffset.y - (originalColliderSize.y / 4));

        youLostPanel.SetActive(false);
        youWonPanel.SetActive(false);
        moveSpeed = defaultspeed;

        // Calculate armor bonuses and set max HP
        CalculateArmorBonuses();
        hp = maxhp; // Start with full HP

        // Ensure player has gun1 equipped by default
        if (string.IsNullOrEmpty(EquipmentService.GetEquippedGun(1)))
        {
            foreach (var item in database.allItems)
            {
                if (item != null && item.itemType == Items.ItemType.Gun &&
                    ProgressService.IsItemUnlocked(item.itemId, item.requiredLevel))
                {
                    EquipmentService.EquipGun(item.itemId, 1);
                    break;
                }
            }
        }
        SetHPPercent(hp / maxhp);

        SetupCurrentGun();
    }

    void CalculateArmorBonuses()
    {
        totalHpBonus = 0f;
        totalDamageReduction = 0f;

        // Check all armor slots
        string[] armorTypes = { "Helmet", "Chestplate", "Leggings", "Boots" };

        foreach (string armorType in armorTypes)
        {
            string armorId = EquipmentService.GetEquippedArmor(armorType);
            if (!string.IsNullOrEmpty(armorId))
            {
                Items armorItem = database.GetItemById(armorId);
                if (armorItem != null && armorItem.itemType == Items.ItemType.Armor)
                {
                    totalHpBonus += armorItem.hpIncreaseFlat;
                    totalDamageReduction += armorItem.damageReductionPct;
                }
            }
        }

        // Clamp damage reduction to prevent negative damage or immunity
        totalDamageReduction = Mathf.Clamp01(totalDamageReduction);

        // Calculate new max HP
        maxhp = baseMaxHp + totalHpBonus;

        // If current HP is now above max HP (due to removing armor), clamp it
        if (hp > maxhp)
        {
            hp = maxhp;
        }

        // Update HP UI
        SetHPPercent(hp / maxhp);

        Debug.Log($"Armor Bonuses - HP Bonus: +{totalHpBonus}, Damage Reduction: {totalDamageReduction * 100f:0.1f}%, Max HP: {maxhp}");
    }

    // Call this method whenever armor equipment changes
    public void OnArmorEquipmentChanged()
    {
        CalculateArmorBonuses();
    }

    void Update()
    {
        if(Time.time - lastdamagetaken > 10f && hp < maxhp)
        {
            heal();
        }
        HandleMovement();
        HandleGunSwitching();
        HandleShooting();
    }

    void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            isFacingLeft = moveInput < 0;
            spriteRenderer.flipX = isFacingLeft;
        }

        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isFacingLeft", isFacingLeft);

        bool isDucking = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        animator.SetBool("duck", isDucking);

        if (isDucking)
        {
            capsuleCollider.size = duckingColliderSize;
            capsuleCollider.offset = crouchOffset;
            moveSpeed = crouchingspeed;
        }
        else
        {
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalOffset;
            moveSpeed = defaultspeed;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDucking && Time.time - lastJumpTime >= jumpcooldown)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("jump");
            lastJumpTime = Time.time;
        }
    }

    void HandleGunSwitching()
    {
        if (Time.time - lastWeaponSwitchTime < weaponSwitchCooldown)
        {
            weaponSwitching = true;
            return;
        }
        else if (weaponSwitching)
        {
            weaponSwitching = false;
            SetupCurrentGun();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToGun(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToGun(2);
        }
    }

    void SwitchToGun(int gunSlot)
    {
        if (EquipmentService.GetActiveGunSlot() == gunSlot) return;

        string gunId = EquipmentService.GetEquippedGun(gunSlot);
        if (string.IsNullOrEmpty(gunId)) return;

        EquipmentService.SetActiveGunSlot(gunSlot);
        lastWeaponSwitchTime = Time.time;
        weaponSwitching = true;

        if (currentGunObject != null)
        {
            Destroy(currentGunObject);
            currentGunObject = null;
            currentGunItem = null;
        }

        if (inGameInventory != null)
        {
            inGameInventory.UpdateInventoryDisplay();
        }

        Debug.Log($"Switching to gun slot {gunSlot}");
    }

    void SetupCurrentGun()
    {
        if (currentGunObject != null)
        {
            Destroy(currentGunObject);
            currentGunObject = null;
            currentGunItem = null;
        }

        string activeGunId = EquipmentService.GetActiveGunId();
        if (string.IsNullOrEmpty(activeGunId)) return;

        currentGunItem = database.GetItemById(activeGunId);
        if (currentGunItem == null) return;

        if (gunHolder != null && currentGunItem.bulletPrefab != null)
        {
            currentGunObject = new GameObject("EquippedGun");
            currentGunObject.transform.SetParent(gunHolder);
            currentGunObject.transform.localPosition = Vector3.zero;
            currentGunObject.transform.localRotation = Quaternion.identity;

            SpriteRenderer gunSprite = currentGunObject.AddComponent<SpriteRenderer>();
            gunSprite.sprite = currentGunItem.icon;
            gunSprite.sortingOrder = 1;

            float scale = currentGunItem.sizeMultiplier;
            currentGunObject.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void HandleShooting()
    {
        if (weaponSwitching) return;

        bool isDucking = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool mouseInput = Input.GetMouseButton(0);

        animator.SetBool("shooting", mouseInput && !weaponSwitching);

        if (currentGunItem == null || isDucking) return;

        float shootCooldown = currentGunItem.gunCoolDown;
        if (mouseInput && Time.time - lastShootTime >= shootCooldown)
        {
            if (currentGunItem.bulletPrefab != null && bulletSpawnPoint != null)
            {
                GameObject bullet = Instantiate(currentGunItem.bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(
                        currentGunItem.bulletSpeed,
                        currentGunItem.bulletDamage,
                        currentGunItem.bulletMaxDistance,
                        bulletSpawnPoint.position
                    );
                }
                lastShootTime = Time.time;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        // Apply damage reduction from armor
        float reducedDamage = amount * (1f - totalDamageReduction);
        lastdamagetaken = Time.time;
        hp -= reducedDamage;
        SetHPPercent(hp / maxhp);

        Debug.Log($"Took {amount} damage, reduced to {reducedDamage:0.1f} (Reduction: {totalDamageReduction * 100f:0.1f}%)");

        if (hp <= 0)
        {
            youLostPanel.SetActive(true);
            Destroy(gameObject);
        }
    }
    void heal()
    {
        lastdamagetaken = Time.time;
        hp += healamount;
        SetHPPercent(hp / maxhp);
    }
    void SetHPPercent(float percent)
    {
        hpFillImage.fillAmount = Mathf.Clamp01(percent);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // Add this method to handle weapon equipment changes
    public void OnWeaponEquipmentChanged()
    {
        SetupCurrentGun();
    }

    // Public getters for other scripts to access armor stats
    public float GetMaxHP() => maxhp;
    public float GetTotalHPBonus() => totalHpBonus;
    public float GetTotalDamageReduction() => totalDamageReduction;
}
