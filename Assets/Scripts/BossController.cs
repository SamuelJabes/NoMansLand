using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject knifePrefab; // Prefab da faca
    [SerializeField] private Transform firePoint;    // Ponto de onde as facas saem

    [Header("Órbita do FirePoint")]
    [SerializeField] private float firePointOrbitRadius = 1.2f; // raio da órbita em torno do boss

    [Header("Configuração de Ataques")]
    [SerializeField] private float timeBetweenAttacks = 3f; // Tempo entre cada ataque
    [SerializeField] private float detectionRange = 15f;    // Distância para começar a atacar

    [Header("Ataque: Lançar Facas")]
    [SerializeField] private int knivesPerAttack = 3;        // Quantas facas lançar por ataque
    [SerializeField] private float timeBetweenKnives = 0.3f; // Tempo entre cada faca
    [SerializeField] private float knifeSpeed = 8f;

    [Header("Ataque: Corrida (Dash)")]
    [SerializeField] private float chargeSpeed = 14f;             // Velocidade da corrida
    [SerializeField] private float chargeDuration = 1.0f;         // TEMPO MÁXIMO de dash (só segurança)
    [SerializeField] private float chargePreparationTime = 0.35f; // Tempo "carregando" (vermelho)
    [SerializeField] private bool predictionCharge = true;        // Prevê onde o player vai estar
    [SerializeField] private float predictionMultiplier = 0.25f;  // Quanto prevê o movimento

    [Header("Distâncias do Dash")]
    [Tooltip("Distância mínima para o boss decidir dar dash. Não dasha se estiver colado.")]
    [SerializeField] private float minChargeDistance = 3f;

    [Tooltip("Distância máxima que ele tenta percorrer com o dash.")]
    [SerializeField] private float maxChargeDistance = 8f;

    [Tooltip("Quanto ele tenta passar um pouco além da posição alvo (para atravessar o player).")]
    [SerializeField] private float chargeOvershoot = 1.0f;

    [Header("Movimento Normal")]
    [SerializeField] private float normalSpeed = 3f;      // Velocidade normal de perseguição
    [SerializeField] private float stoppingDistance = 5f; // Distância mínima do player

    [Header("Dano por contato no Dash")]
    [SerializeField] private int contactDamageUnits = 2;
    // ex: 2 unidades = 1 coração, se 1 unidade = meio coração

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isPreparingCharge = false;
    private Vector2 chargeDirection;
    private float currentDashDistance;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private enum AttackType { ThrowKnives, Charge }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Não deixar o boss girar em Z
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        UpdateFirePointOrbit();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se está correndo no dash → movimento só do dash
        if (isCharging)
        {
            rb.linearVelocity = chargeDirection * chargeSpeed;
            return;
        }

        // Se está em ataque (lançando facas ou preparando dash) → fica parado
        if (isAttacking)
        {
            StopCompletely();
            return;
        }

        // Movimento normal: persegue o player mantendo distância mínima
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * normalSpeed;
        }
        else
        {
            StopCompletely();
        }
    }

    private void StopCompletely()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void UpdateFirePointOrbit()
    {
        if (player == null || firePoint == null) return;

        Vector2 dir = (player.position - transform.position);
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        Vector2 orbitPos = (Vector2)transform.position + dir * firePointOrbitRadius;
        firePoint.position = orbitPos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);

            if (player == null) continue;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                int randomValue = Random.Range(0, 100);

                if (randomValue < 75)
                {
                    yield return StartCoroutine(ThrowKnivesAttack());
                }
                else
                {
                    yield return StartCoroutine(ChargeAttack());
                }
            }
        }
    }

    IEnumerator ThrowKnivesAttack()
    {
        isAttacking = true;
        StopCompletely();

        Debug.Log("Boss: Lançando facas!");

        for (int i = 0; i < knivesPerAttack; i++)
        {
            if (knifePrefab != null && firePoint != null)
            {
                GameObject knife = Instantiate(knifePrefab, firePoint.position, firePoint.rotation);

                BossKnife knifeScript = knife.GetComponent<BossKnife>();
                if (knifeScript != null)
                {
                    knifeScript.SetSpeed(knifeSpeed);
                }
            }

            yield return new WaitForSeconds(timeBetweenKnives);
        }

        isAttacking = false;
    }

    IEnumerator ChargeAttack()
    {
        if (player == null) yield break;

        float initialDistance = Vector2.Distance(transform.position, player.position);

        // Se está muito perto, não compensa dar dash → volta pro ciclo
        if (initialDistance < minChargeDistance)
        {
            yield break;
        }

        isAttacking = true;
        Debug.Log("Boss: Preparando corrida!");

        StopCompletely();

        // --- Fase de preparação (fica vermelho, "carregando") ---
        isPreparingCharge = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f);
        }

        yield return new WaitForSeconds(chargePreparationTime);

        isPreparingCharge = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // --- Calcula direção e distância do dash DEPOIS da preparação ---
        Vector2 bossPos = transform.position;
        Vector2 targetPosition = player.position;

        if (predictionCharge)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 playerVelocity = playerRb.linearVelocity;
                targetPosition = (Vector2)player.position + (playerVelocity * predictionMultiplier);
            }
        }

        Vector2 dir = (targetPosition - bossPos);

        // Se por algum motivo ele acabou vindo muito perto, cancela
        float distanceNow = dir.magnitude;
        if (distanceNow < 0.5f)
        {
            isAttacking = false;
            yield break;
        }

        dir.Normalize();
        chargeDirection = dir;

        // Distância alvo do dash (clampada)
        float desiredDistance = distanceNow + chargeOvershoot;
        currentDashDistance = Mathf.Clamp(desiredDistance, minChargeDistance, maxChargeDistance);

        Debug.Log($"Boss: Correndo! Distância alvo ~ {currentDashDistance:F2}");

        // --- Fase de dash ---
        isCharging = true;
        isAttacking = false; // agora ele está "só dashing"

        Vector2 startPos = transform.position;
        float elapsed = 0f;

        while (isCharging && elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;

            float traveled = Vector2.Distance(startPos, transform.position);
            if (traveled >= currentDashDistance)
            {
                // alcançou a distância planejada
                break;
            }

            yield return null;
        }

        // Se ainda estava dashing, força parar
        if (isCharging)
        {
            StopCharge();
            Debug.Log("Boss: Corrida finalizada (distância/tempo).");
        }

        // Pequena pausa pós dash
        yield return new WaitForSeconds(0.5f);
    }

    // ================================
    // DANO POR CONTATO (DASH DO BOSS)
    // ================================
    void DealContactDamage(GameObject other)
    {
        if (!isCharging) return;               // só causa dano durante o dash
        if (!other.CompareTag("Player")) return;

        HeartsHealthUI heartsUI = FindObjectOfType<HeartsHealthUI>();
        if (heartsUI != null)
        {
            heartsUI.TakeDamage(contactDamageUnits);
            Debug.Log($"Boss acertou o player no dash! Dano: {contactDamageUnits} unidade(s) de coração");
        }
        else
        {
            Debug.LogWarning("[BossController] HeartsHealthUI não encontrado na cena!");
        }

        StopCharge();
    }

    // Se o boss usar colliders NÃO-trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging)
        {
            DealContactDamage(collision.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isCharging)
        {
            DealContactDamage(collision.gameObject);
        }
    }

    // Caso haja hitboxes em trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            DealContactDamage(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isCharging)
        {
            DealContactDamage(other.gameObject);
        }
    }

    void StopCharge()
    {
        if (!isCharging) return;

        isCharging = false;
        StopCompletely();

        Debug.Log("Boss parou a corrida.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (isCharging || isPreparingCharge)
        {
            Gizmos.color = isPreparingCharge ? Color.yellow : Color.red;
            Vector3 lineEnd = transform.position + (Vector3)(chargeDirection * 5f);
            Gizmos.DrawLine(transform.position, lineEnd);
            Gizmos.DrawWireSphere(lineEnd, 0.3f);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, firePointOrbitRadius);
    }
}
