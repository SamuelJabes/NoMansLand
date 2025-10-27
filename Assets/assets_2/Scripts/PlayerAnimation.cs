using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 lastMoveDirection = Vector2.down;

    void Update()
    {
        Vector2 movement = new Vector2(rb.linearVelocityX, rb.linearVelocityY);


        if (movement.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = movement.normalized;
        }

        // Atualiza parâmetros do Animator
        animator.SetFloat("Xinput", lastMoveDirection.x);
        animator.SetFloat("Yinput", lastMoveDirection.y);
        animator.SetFloat("Speed", movement.magnitude);
    }
}
