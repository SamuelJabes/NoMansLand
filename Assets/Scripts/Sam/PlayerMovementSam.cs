using UnityEngine;

public class PlayerMovementSam : MonoBehaviour
{
    public float moveSpeed = 6f;
    public bool useAcceleration = false;
    public float acceleration = 20f;

    private Rigidbody2D rb;
    private Vector2 inputMove;
    private Vector2 targetVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputMove.sqrMagnitude > 1f) inputMove.Normalize();
        targetVelocity = inputMove * moveSpeed;
    }

    void FixedUpdate()
    {
        if (useAcceleration)
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        else
            rb.linearVelocity = targetVelocity;
    }
}
