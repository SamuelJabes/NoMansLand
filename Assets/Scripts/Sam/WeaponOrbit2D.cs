using UnityEngine;

public class WeaponOrbit2D : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;          // arraste o Transform do player
    public Camera cam;                // arraste a câmera (se vazio usa Camera.main)

    [Header("Órbita")]
    [Min(0.01f)] public float radius = 1.0f;     // raio da órbita
    [Range(0.01f, 50f)] public float followLerp = 25f;  // suavidade de posição (maior = mais “grudado”)

    [Header("Rotação da arma")]
    public bool faceMouse = true;                 // true: arma aponta pro mouse
    [Range(0.01f, 50f)] public float rotateLerp = 25f;  // suavidade da rotação
    public float angleOffsetDeg = 0f;             // ajuste fino se o sprite está “de lado”

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void LateUpdate()
    {
        if (!player || !cam) return;

        // ===== 1) pegar mouse em mundo (2D) =====
        // funciona com câmera ortográfica ou perspectiva:
        float zDist = Mathf.Abs(cam.transform.position.z - player.position.z);
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist));
        // manter no plano 2D do player
        mouseWorld.z = player.position.z;

        // ===== 2) direção player->mouse (XY) =====
        Vector2 dir = (mouseWorld - player.position);
        if (dir.sqrMagnitude < 0.00001f) return;  // se mouse “em cima” do player, evita NaN
        dir.Normalize();

        // ===== 3) posição alvo na órbita =====
        Vector3 targetPos = player.position + (Vector3)(dir * radius);

        // suaviza a transição (lerp exponencial)
        float tFollow = 1f - Mathf.Exp(-followLerp * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, tFollow);

        // ===== 4) rotação da arma (opcional) =====
        if (faceMouse)
        {
            // ângulo em graus (0° no +X, cresce CCW). Para apontar “para o mouse”:
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffsetDeg;
            float current = transform.eulerAngles.z;
            float target = ang;

            // interpola em Z para suavizar
            float tRot = 1f - Mathf.Exp(-rotateLerp * Time.deltaTime);
            float newZ = Mathf.LerpAngle(current, target, tRot);
            transform.rotation = Quaternion.Euler(0f, 0f, newZ);
        }
    }
}
