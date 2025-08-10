using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float maxDistance;
    public Vector3 startPosition;

    // Set these to match your project's actual layer numbers
    private int layerground = 6; // ground/obstacle layer
    private int layerplayer = 7; // player layer
    private int layermob = 8; // for example, mob/enemy layer
    private int layerzone = 11;
    public void Initialize(float bulletSpeed, float bulletDamage, float bulletMaxDistance, Vector3 position)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        maxDistance = bulletMaxDistance;
        startPosition = position; // Set the initial position
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {   
        if (other.isTrigger) return; // Ignore triggers

        int otherLayer = other.gameObject.layer;

        if (otherLayer == layerground || otherLayer == layerzone)
        {
            // Hit ground/obstacle: no damage
            Destroy(gameObject);
        }
        else if (otherLayer == layerplayer || otherLayer == layermob)
        {
            // Try to deal damage if valid target script exists
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            // Or, if your player/mob uses a different damage system, call it here
            Destroy(gameObject);
        }
        // Optionally, ignore all else: bullet passes through or implement extra rules
    }
}
