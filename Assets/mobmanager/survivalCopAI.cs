using UnityEngine;

public class survivalCopAI : MonoBehaviour, IDamageable, ISurvivalMob
{
    [Header("AI Settings")]
    public Transform player;
    public float sightRange;
    public float shootingRange;
    public float moveSpeed = 5f;
    public float HP = 100;
    public survivalmobs mobManager;
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 1f;
    public float bulletSpeed;
    public float bulletDamage;
    public float maxBulletDistance;
    [Header("Detection")]
    public LayerMask obstacleLayer;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextFireTime;
    private bool isFacingRight = false;
    private bool isDying = false;
    private readonly string INSIGHT_PARAM = "inSight";
    private readonly string INRANGE_PARAM = "inRange";
    private readonly string ISFACINGRIGHT_PARAM = "isFacingRight";

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            //Debug.Log("CopAI: Player transform is null, attempting to find player by tag");
            player = GameObject.FindGameObjectWithTag("Player").transform;
            //Debug.Log("CopAI: Player found: " + (player != null ? player.name : "null"));
        }

        rb.freezeRotation = true;
    }
    void ISurvivalMob.SetManager(survivalmobs manager)
    {
        mobManager = manager;
        //Debug.Log("CopAI: Mob manager set.");
    }

    void Update()
    {
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        //Debug.Log("CopAI: Distance to player = " + distanceToPlayer);

        bool playerInSight = distanceToPlayer <= sightRange && HasLineOfSight2D();
        //Debug.Log("CopAI: Player in sight = " + playerInSight);

        bool playerInRange = distanceToPlayer <= shootingRange && HasLineOfSight2D();
        //Debug.Log("CopAI: Player in shooting range = " + playerInRange);

        animator.SetBool(INSIGHT_PARAM, playerInSight);
        animator.SetBool(INRANGE_PARAM, playerInRange);
        animator.SetBool(ISFACINGRIGHT_PARAM, isFacingRight);

        if (playerInSight)
        {
            //Debug.Log("CopAI: Player detected, engaging");
            if (playerInRange)
            {
                //Debug.Log("CopAI: Player in range, stopping and shooting");
                rb.linearVelocity = Vector2.zero;
                LookAtPlayer2D();
                Shoot2D();
            }
            else
            {
                //Debug.Log("CopAI: Player in sight but out of range, moving towards player");
                MoveToPlayer2D();
            }
        }
        else
        {
            //Debug.Log("CopAI: Player not in sight, idle");
            rb.linearVelocity= Vector2.zero;
        }
    }

    bool HasLineOfSight2D()
    {
        //Debug.Log("CopAI: Checking line of sight...");

        Vector2 directionToPlayer = ((Vector2)(player.position - transform.position)).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
        {
            //Debug.Log("CopAI: Raycast hit: " + hit.collider.name);
            return hit.transform == player;
        }
        else
        {
            //Debug.Log("CopAI: Raycast found no obstacles");
            return true;
        }
    }


    void MoveToPlayer2D()
    {
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);
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
