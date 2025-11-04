using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRotation : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Velocidade linear do player (unidades por segundo).")]
    public float moveSpeed = 6f;

    [Header("Mira/Rota��o")]
    [Tooltip("Se verdadeiro, o player sempre gira para o cursor do mouse.")]
    public bool faceMouse = true;

    [Tooltip("Se falso, o player gira para a dire��o do movimento.")]
    public bool rotateOnlyWhenMoving = true;

    Rigidbody2D rb;
    Vector2 inputMove;
    Vector2 aimDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Recomenda��es f�sicas para um controlador de top-down:
        // rb.gravityScale = 0; // garanta isso no Inspector
        // rb.freezeRotation = false; // deixe liberar rota��o para o MoveRotation funcionar
        // rb.interpolation = RigidbodyInterpolation2D.Interpolate; // suaviza movimento
    }

    void Update()
    {
        // Leitura do input (WASD / Setas). Use GetAxisRaw para resposta imediata.
        inputMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inputMove.sqrMagnitude > 1f) inputMove.Normalize();

        // Dire��o de mira
        if (faceMouse)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = transform.position.z;
            aimDir = ((Vector2)(mouseWorld - transform.position)).normalized;
        }
        else
        {
            aimDir = inputMove;
        }
    }

    void FixedUpdate()
    {
        // Movimento f�sico
        rb.linearVelocity = inputMove * moveSpeed;

        // Condi��o para rotacionar
        bool shouldRotate = faceMouse || (!rotateOnlyWhenMoving) || (rb.linearVelocity.sqrMagnitude > 0.0001f);

        if (shouldRotate && aimDir.sqrMagnitude > 0.000001f)
        {
            // Atan2 retorna �ngulo em rela��o ao +X; subtra�mos 90� para alinhar "para cima" (+Y) como frente.
            float targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
            rb.MoveRotation(targetAngle);
        }
    }
}
