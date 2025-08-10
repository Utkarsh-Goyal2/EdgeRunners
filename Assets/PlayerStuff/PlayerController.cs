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
    public float maxhp = 100f;
    public float hp; // Player health
    
    [Header("UI Panels")]
    public GameObject youLostPanel;
    public Image hpFillImage;
    public GameObject youWonPanel;

    [Header("Gun Settings")]
    public Transform gunHolder;           // Assign in Inspector
    public GameObject gunPrefab;          // Assign in Inspector

    [Header("Shooting Settings")]
    public Transform bulletSpawnPoint;  // Assign in Inspector
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

    private float moveInput;
    private float moveSpeed;
    guninfo gunInfo;
    public bool isFacingLeft { get; private set; } // Exposed as read-only

    void Start()
    {
        
        hp = maxhp;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        originalColliderSize = capsuleCollider.size;
        duckingColliderSize = new Vector2(originalColliderSize.x, originalColliderSize.y / 2);

        originalOffset = capsuleCollider.offset;
        crouchOffset = new Vector2(originalOffset.x, originalOffset.y - (originalColliderSize.y / 4));

        youLostPanel.SetActive(false);
        youWonPanel.SetActive(false);

        moveSpeed = defaultspeed;
        SetUpGun();

    }
    void SetUpGun()
    {
        gunInfo = gunPrefab != null ? gunPrefab.GetComponent<guninfo>() : null;
        // Instantiate gun if needed (existing logic)
        if (gunHolder != null && gunPrefab != null)
        {
            GameObject gun = Instantiate(gunPrefab, gunHolder.position, gunHolder.rotation, gunHolder);
            gun.transform.localScale = gun.transform.localScale * (gunInfo != null ? gunInfo.size_multiplier : 1f);
        }
    }
    void Update()
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

        bool mouseinput = Input.GetMouseButton(0);
        animator.SetBool("shooting", mouseinput);
        float shootCooldown = gunInfo != null ? gunInfo.gunCoolDown : 0.5f;
        if (mouseinput && !isDucking && Time.time - lastShootTime >= shootCooldown)
        {
            GameObject bulletPrefab = gunInfo.bulletPrefab;
            float bulletSpeed = gunInfo.bulletSpeed;
            float bulletDamage = gunInfo.bulletDamage;
            float bulletMaxDistance = gunInfo.bulletMaxDistance;
            if (bulletPrefab != null && bulletSpawnPoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(bulletSpeed, bulletDamage, bulletMaxDistance, bulletSpawnPoint.position);
                }
            }
            lastShootTime = Time.time;
        }
    }

    public void TakeDamage(float amount)
    {
        //animator.SetTrigger("hurt");
        hp -= amount;
        SetHPPercent(hp / maxhp);
        if (hp <= 0)
        {
            youLostPanel.SetActive(true);
            Destroy(gameObject);

        }
    }
    void SetHPPercent(float percent)
    {
        hpFillImage.fillAmount = Mathf.Clamp01(percent);
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}
