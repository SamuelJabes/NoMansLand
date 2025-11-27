using UnityEngine;

public class WeaponOrbit2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;          // arraste o Transform do player
    public Camera cam;                // arraste a c�mera (se vazio usa Camera.main)

    [Header("Mobile Auto-Aim (Opcional)")]
    [Tooltip("Sistema de auto-aim para mobile. Se vazio, busca automaticamente.")]
    public AutoAimSystem autoAim;

    [Header("�rbita")]
    [Min(0.01f)] public float radius = 1.0f;     // raio da �rbita
    [Range(0.01f, 50f)] public float followLerp = 25f;  // suavidade de posi��o (maior = mais �grudado�)

    [Header("Rota��o da arma")]
    public bool faceMouse = true;                 // true: arma aponta pro mouse (ou auto-aim em mobile)
    [Range(0.01f, 50f)] public float rotateLerp = 25f;  // suavidade da rota��o
    public float angleOffsetDeg = 0f;             // ajuste fino se o sprite est� �de lado�

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // Tenta encontrar o auto-aim automaticamente
        if (autoAim == null)
        {
            autoAim = FindObjectOfType<AutoAimSystem>();
        }
    }

    void LateUpdate()
    {
        if (!player || !cam) return;

        // Obt�m dire��o de acordo com a plataforma (Auto-aim ou Mouse)
        Vector2 dir = GetAimDirection();
        if (dir.sqrMagnitude < 0.00001f) return;  // evita NaN
        dir.Normalize();

        // ===== 1) posi��o alvo na �rbita =====
        Vector3 targetPos = player.position + (Vector3)(dir * radius);

        // suaviza a transi��o (lerp exponencial)
        float tFollow = 1f - Mathf.Exp(-followLerp * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, tFollow);

        // ===== 2) rota��o da arma (opcional) =====
        if (faceMouse)
        {
            // �ngulo em graus (0� no +X, cresce CCW)
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffsetDeg;
            float current = transform.eulerAngles.z;
            float target = ang;

            // interpola em Z para suavizar
            float tRot = 1f - Mathf.Exp(-rotateLerp * Time.deltaTime);
            float newZ = Mathf.LerpAngle(current, target, tRot);
            transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
    }

    /// <summary>
    /// Obt�m dire��o de mira de acordo com a plataforma
    /// </summary>
    Vector2 GetAimDirection()
    {
        // Prioridade 1: Auto-Aim (se estiver mobile e tiver alvo)
        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsMobile)
        {
            if (autoAim != null && autoAim.HasTarget)
            {
                return autoAim.GetAimDirection();
            }
            // Se mobile mas sem alvo, aponta para frente (dire��o do movimento)
            // Isso evita que a arma fique parada
            return Vector2.right; // ou pegar dire��o do movimento do player
        }

        // Prioridade 2: Mouse (PC)
        float zDist = Mathf.Abs(cam.transform.position.z - player.position.z);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist));
        mouseWorld.z = player.position.z;

        Vector2 dir = (mouseWorld - player.position);
        return dir;
    }
}
