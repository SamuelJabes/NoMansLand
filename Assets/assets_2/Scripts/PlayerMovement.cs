using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configura��es de movimento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 4.5f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 input;
    private Vector2 lastMove = Vector2.down;

    void Update()
    {
        // Coleta o input (WASD ou setas)
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
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
        rb.linearVelocity = input * currentSpeed; // ou rb.velocity em vers�es antigas
    }
}
