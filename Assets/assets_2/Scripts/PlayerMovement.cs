using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de movimento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 4.5f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Mobile Input (Opcional)")]
    [Tooltip("Referência ao joystick mobile. Se vazio, busca automaticamente.")]
    public MobileJoystick mobileJoystick;

    private Vector2 input;
    private Vector2 lastMove = Vector2.down;

    void Awake()
    {
        // Tenta encontrar o joystick mobile automaticamente
        if (mobileJoystick == null)
        {
            mobileJoystick = FindObjectOfType<MobileJoystick>();
        }
    }

    void Update()
    {
        // Coleta o input (Mobile Joystick ou WASD)
        input = GetInputDirection();
        input = input.normalized;

        bool isRunning = Input.GetKey(runKey);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Atualiza par�metros do Animator
        animator.SetFloat("Xinput", input.x);
        animator.SetFloat("Yinput", input.y);
        animator.SetFloat("Speed", rb.linearVelocity.sqrMagnitude);
        animator.SetBool("IsRunning", isRunning);

        // Guarda a �ltima dire��o para o Idle direcional
        if (input.sqrMagnitude > 0.001f)
        {
            lastMove = input;
            animator.SetFloat("LastMoveX", lastMove.x);
            animator.SetFloat("LastMoveY", lastMove.y);
        }

        // Movimento real
        rb.linearVelocity = input * currentSpeed; // ou rb.velocity em versões antigas
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
