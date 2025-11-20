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

    [Header("Ataque: Corrida")]
    [SerializeField] private float chargeSpeed = 15f;             // Velocidade da corrida
    [SerializeField] private float chargeDuration = 2.5f;         // Duração da corrida
    [SerializeField] private float chargePreparationTime = 0.3f;  // Tempo de preparação antes de correr
    [SerializeField] private bool predictionCharge = true;        // Prevê onde o player vai estar
    [SerializeField] private float predictionMultiplier = 0.5f;   // Quanto prevê o movimento

    [Header("Movimento Normal")]
    [SerializeField] private float normalSpeed = 3f;        // Velocidade normal de perseguição
    [SerializeField] private float stoppingDistance = 5f;   // Distância mínima do player

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isPreparingCharge = false;
    private Vector2 chargeDirection;
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
        // ou, dependendo da sua versão:
        // rb.freezeRotation = true;
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
        // Atualiza posição/rotação do firePoint em torno do boss
        UpdateFirePointOrbit();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se está correndo no dash
        if (isCharging)
        {
            float checkDistance = 1f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, chargeDirection, checkDistance, ~LayerMask.GetMask("Player"));

            if (hit.collider != null)
            {
                // Pare se bater em algo
                Debug.Log("Raycast detectou obstáculo: " + hit.collider.name);
                StopCharge();
                return;
            }

            rb.linearVelocity = chargeDirection * chargeSpeed;
            return;
        }

        // Se está em qualquer ataque (facas ou pré/pós dash) -> fica parado
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

    /// <summary>
    /// Zera completamente o movimento do boss.
    /// </summary>
    private void StopCompletely()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Atualiza posição e rotação do firePoint em torno do boss,
    /// sempre apontando para o player.
    /// </summary>
    void UpdateFirePointOrbit()
    {
        if (player == null || firePoint == null) return;

        Vector2 dir = (player.position - transform.position);
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        // POSIÇÃO: em uma órbita na direção do player
        Vector2 orbitPos = (Vector2)transform.position + dir * firePointOrbitRadius;
        firePoint.position = orbitPos;

        // ROTAÇÃO: firePoint "olha" pro player (up aponta pra dir)
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
        StopCompletely(); // garante boss parado enquanto lança facas

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
        isAttacking = true;
        Debug.Log("Boss: Preparando corrida!");

        StopCompletely();

        // Calcula a direção para o player COM PREDIÇÃO
        Vector2 targetPosition = player.position;

        if (predictionCharge)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 playerVelocity = playerRb.linearVelocity;
                targetPosition = (Vector2)player.position + (playerVelocity * predictionMultiplier);
                Debug.Log("Boss: Prevendo posição do player!");
            }
        }

        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;

        // Feedback visual: vermelho durante preparação
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

        Debug.Log("Boss: Correndo!");
        isCharging = true;
        isAttacking = false; // agora está "só" correndo

        float chargeTimer = 0f;
        while (chargeTimer < chargeDuration && isCharging)
        {
            chargeTimer += Time.deltaTime;
            yield return null;
        }

        if (isCharging)
        {
            isCharging = false;
            StopCompletely();
            Debug.Log("Boss: Corrida finalizada por tempo!");
        }

        // pequena pausa depois do dash
        yield return new WaitForSeconds(0.5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            Health2D playerHealth = collision.gameObject.GetComponent<Health2D>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(20f);
                Debug.Log("Boss acertou o player na corrida!");
            }
            StopCharge();
        }
        else if (isCharging)
        {
            Debug.Log("Boss colidiu com: " + collision.gameObject.name);
            StopCharge();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isCharging)
        {
            StopCharge();
        }
    }

    void StopCharge()
    {
        if (!isCharging) return;

        isCharging = false;
        StopCompletely();

        Debug.Log("Boss parou a corrida por colisão!");
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

        // Gizmo da órbita do firePoint
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, firePointOrbitRadius);
    }
}
