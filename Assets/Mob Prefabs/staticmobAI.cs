using UnityEngine;

public class staticmobAI : MonoBehaviour, IDamageable, IMob
{
    [Header("AI Settings")]
    public Transform player;
    public float shootingRange;
    public float HP = 100;
    public MobManager1 mobManager; // Reference to the mob manager for callbacks
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 1f; // Time in seconds between shots
    public float bulletSpeed;
    public float bulletDamage;
    public float maxBulletDistance;
    [Header("Detection")]
    public LayerMask obstacleLayer = 6;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasLineOfSight;
    private bool isFacingRight = false;
    // Animator parameter names
    private readonly string INRANGE_PARAM = "inrange";
    private readonly string ISFACINGRIGHT_PARAM = "isFacingRight";
    private float nextFireTime;
    private bool isDying = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
           player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null)
        {
            //Debug.LogWarning("StaticMobAI: No player assigned or found!");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        //Debug.Log("StaticMobAI: Distance to player = " + distanceToPlayer);

        bool playerInRange = distanceToPlayer <= shootingRange && HasLineOfSight2D();
        //Debug.Log("StaticMobAI: Player in shooting range and visible = " + playerInRange);

        animator.SetBool(INRANGE_PARAM, playerInRange);
        animator.SetBool(ISFACINGRIGHT_PARAM, isFacingRight);

        if (playerInRange)
        {
            //Debug.Log("StaticMobAI: Player in range - looking and shooting");
            LookAtPlayer2D();
            Shoot2D();
        }
    }

    void IMob.SetManager(MobManager1 manager)
    {
        mobManager = manager;
    }
    bool HasLineOfSight2D()
    {
        //Debug.Log("StaticMobAI: Checking line of sight...");

        Vector2 directionToPlayer = ((Vector2)(player.position - transform.position)).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
        {
            //Debug.Log("StaticMobAI: Raycast hit: " + hit.collider.name);
            hasLineOfSight = hit.transform == player;
        }
        else
        {
            //Debug.Log("StaticMobAI: Raycast found no obstacles");
            hasLineOfSight = true;
        }

        return hasLineOfSight;
    }

    void LookAtPlayer2D()
    {
        float directionX = player.position.x - transform.position.x;
        spriteRenderer.flipX = directionX > 0;
        isFacingRight = directionX > 0;
    }

    void Shoot2D()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            if (bulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(bulletSpeed, bulletDamage, maxBulletDistance, firePoint.position);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        //Debug.Log("StaticMobAI: Took " + damage + " damage, HP now: " + HP);
        if (HP <= 0)
        {
            if (isDying) return; // Prevent multiple calls            
            Die();

        }
    }
    void Die()
    {
        isDying = true;
        animator.SetTrigger("Die");
        //this.enabled = false;
        //GetComponent<Collider>().enabled = false;
        mobManager.MobDied(this.gameObject);


        //Debug.Log("StaticMobAI: Dying animation triggered");
        Destroy(gameObject, 0.6f); // Adjust delay as needed
    }

}
