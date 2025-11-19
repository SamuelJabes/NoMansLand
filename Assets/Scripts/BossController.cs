using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject knifePrefab; // Prefab da faca
    [SerializeField] private Transform firePoint; // Ponto de onde as facas saem

    [Header("Configuração de Ataques")]
    [SerializeField] private float timeBetweenAttacks = 3f; // Tempo entre cada ataque
    [SerializeField] private float detectionRange = 15f; // Distância para começar a atacar

    [Header("Ataque: Lançar Facas")]
    [SerializeField] private int knivesPerAttack = 3; // Quantas facas lançar por ataque
    [SerializeField] private float timeBetweenKnives = 0.3f; // Tempo entre cada faca
    [SerializeField] private float knifeSpeed = 8f;

    [Header("Ataque: Corrida")]
    [SerializeField] private float chargeSpeed = 15f; // Velocidade da corrida (aumentado!)
    [SerializeField] private float chargeDuration = 2.5f; // Duração da corrida (aumentado!)
    [SerializeField] private float chargePreparationTime = 0.3f; // Tempo de preparação antes de correr (reduzido!)
    [SerializeField] private bool predictionCharge = true; // Prevê onde o player vai estar
    [SerializeField] private float predictionMultiplier = 0.5f; // Quanto prevê o movimento

    [Header("Movimento Normal")]
    [SerializeField] private float normalSpeed = 3f; // Velocidade normal de perseguição
    [SerializeField] private float stoppingDistance = 5f; // Distância mínima do player

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
    }

    void Start()
    {
        if (player == null)
        {
            // Tenta encontrar o player automaticamente pela tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Inicia o ciclo de ataques
        StartCoroutine(AttackRoutine());
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se está carregando (correndo), verifica parede e move
        if (isCharging)
        {
            // Raycast múltiplo para detectar paredes à frente de forma mais confiável
            float checkDistance = 1f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, chargeDirection, checkDistance, ~LayerMask.GetMask("Player"));
            
            if (hit.collider != null)
            {
                Debug.Log("Raycast detectou obstáculo: " + hit.collider.name);
                StopCharge();
                return;
            }

            // Usa velocity direta para movimento mais preciso
            rb.linearVelocity = chargeDirection * chargeSpeed;
            return;
        }

        // Se está atacando (lançando facas), fica parado
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movimento normal: persegue o player mas mantém distância mínima
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * normalSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);

            if (player == null) continue;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Só ataca se o player estiver dentro do alcance
            if (distanceToPlayer <= detectionRange)
            {
                // 75% de chance de lançar facas, 25% de chance de correr
                int randomValue = Random.Range(0, 100);

                if (randomValue < 75) // 75% de chance
                {
                    yield return StartCoroutine(ThrowKnivesAttack());
                }
                else // 25% de chance
                {
                    yield return StartCoroutine(ChargeAttack());
                }
            }
        }
    }

    IEnumerator ThrowKnivesAttack()
    {
        isAttacking = true;
        Debug.Log("Boss: Lançando facas!");

        for (int i = 0; i < knivesPerAttack; i++)
        {
            // Aponta para o player
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // Instancia a faca
            if (knifePrefab != null)
            {
                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
                GameObject knife = Instantiate(knifePrefab, spawnPos, Quaternion.Euler(0, 0, angle));
                
                // Configura a velocidade da faca
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

        // Preparação: fica parado por um momento
        rb.linearVelocity = Vector2.zero;
        
        // Calcula a direção para o player COM PREDIÇÃO
        Vector2 targetPosition = player.position;
        
        if (predictionCharge)
        {
            // Tenta prever onde o player vai estar baseado no movimento dele
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 playerVelocity = playerRb.linearVelocity;
                targetPosition = (Vector2)player.position + (playerVelocity * predictionMultiplier);
                Debug.Log("Boss: Prevendo posição do player!");
            }
        }
        
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;

        // Feedback visual: Boss fica vermelho brilhante durante preparação
        isPreparingCharge = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f); // Vermelho brilhante
        }
        
        yield return new WaitForSeconds(chargePreparationTime);

        // Volta a cor normal
        isPreparingCharge = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Inicia a corrida
        Debug.Log("Boss: Correndo!");
        isCharging = true;
        isAttacking = false;

        // Corre por um tempo determinado OU até colidir
        float chargeTimer = 0f;
        while (chargeTimer < chargeDuration && isCharging)
        {
            chargeTimer += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Para a corrida (se ainda estiver correndo)
        if (isCharging)
        {
            isCharging = false;
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Boss: Corrida finalizada por tempo!");
        }

        // Pequena pausa após a corrida
        yield return new WaitForSeconds(0.5f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Se colidir durante a corrida, causa dano ao player
        if (isCharging && collision.gameObject.CompareTag("Player"))
        {
            Health2D playerHealth = collision.gameObject.GetComponent<Health2D>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(20f); // Dano da corrida
                Debug.Log("Boss acertou o player na corrida!");
            }
            // Para a corrida após acertar o player
            StopCharge();
        }
        // Se colidir com qualquer outra coisa durante corrida (paredes), para
        else if (isCharging)
        {
            Debug.Log("Boss colidiu com: " + collision.gameObject.name);
            StopCharge();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Garante que pare se ainda estiver colidindo durante a corrida
        if (isCharging)
        {
            StopCharge();
        }
    }

    void StopCharge()
    {
        if (!isCharging) return; // Já parou
        
        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Boss parou a corrida por colisão!");
    }

    // Visualização do alcance no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Desenha a direção do dash
        if (isCharging || isPreparingCharge)
        {
            Gizmos.color = isPreparingCharge ? Color.yellow : Color.red;
            Vector3 lineEnd = transform.position + (Vector3)(chargeDirection * 5f);
            Gizmos.DrawLine(transform.position, lineEnd);
            Gizmos.DrawWireSphere(lineEnd, 0.3f);
        }
    }
}
