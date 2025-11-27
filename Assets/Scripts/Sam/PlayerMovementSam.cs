using UnityEngine;

public class PlayerMovementSam : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 6f;
    public bool useAcceleration = false;
    public float acceleration = 20f;

    [Header("Mobile Input (Opcional)")]
    [Tooltip("Referência ao joystick mobile. Se vazio, busca automaticamente.")]
    public MobileJoystick mobileJoystick;

    private Rigidbody2D rb;
    private Vector2 inputMove;
    private Vector2 targetVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tenta encontrar o joystick mobile automaticamente
        if (mobileJoystick == null)
        {
            mobileJoystick = FindObjectOfType<MobileJoystick>();
        }
    }

    void Update()
    {
        // Obtém input de acordo com a plataforma
        inputMove = GetInputDirection();
        
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

    /// <summary>
    /// Obtém direção de input de acordo com a plataforma (PC ou Mobile)
    /// </summary>
    Vector2 GetInputDirection()
    {
        // Prioridade 1: Mobile Joystick (se estiver sendo usado)
        if (mobileJoystick != null && mobileJoystick.IsPressed)
        {
            return mobileJoystick.Direction;
        }

        // Prioridade 2: Teclado (WASD/Setas) - sempre disponível
        Vector2 keyboardInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        return keyboardInput;
    }
}
