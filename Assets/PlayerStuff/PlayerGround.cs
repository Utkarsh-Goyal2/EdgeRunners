using UnityEngine;

public class PlayerGroundSnap : MonoBehaviour
{
    public float maxStepHeight = 0.2f;        // max vertical difference to snap (20px)
    public float checkDistance = 0.2f;        // how far ahead to check
    public LayerMask groundLayer;             // assign your Ground layer here

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (IsMovingHorizontally())
        {
            TrySnapToGround();
        }
    }

    bool IsMovingHorizontally()
    {
        return Mathf.Abs(rb.linearVelocity.x) > 0.01f;
    }

    void TrySnapToGround()
    {
        Vector2 origin = transform.position + new Vector3(rb.linearVelocity.x > 0 ? 0.25f : -0.25f, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, maxStepHeight + 0.05f, groundLayer);

        if (hit.collider != null)
        {
            float distance = transform.position.y - hit.point.y;
            if (distance > 0.01f && distance <= maxStepHeight)
            {
                // Snap player to platform
                Vector2 pos = rb.position;
                pos.y = hit.point.y + 0.01f; // small offset to prevent collision jitter
                rb.MovePosition(pos);
            }
        }
    }
}
