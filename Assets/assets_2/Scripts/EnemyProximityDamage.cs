using UnityEngine;

public class EnemyProximityDamage : MonoBehaviour
{
    [Tooltip("Transform do jogador. Se vazio, procura pelo tag 'Player'.")]
    public Transform player;

    [Tooltip("Sistema de vida (HeartsHealthUI).")]
    public HeartsHealthUI heartsUI;

    [Tooltip("Distância máxima para causar dano.")]
    public float damageRange = 0.7f; // Reduzido para contato mais próximo

    [Tooltip("Dano em unidades por segundo (1 = meio coração).")]
    public float damageUnitsPerSecond = 1f;

    [Tooltip("Tempo entre cada hit de dano (em segundos). Menor = mais rápido.")]
    public float damageInterval = 0.5f; // Dano a cada 0.5s por padrão

    private float damageTimer;
    private bool hasDealtFirstDamage = false;
    private EnemyHealth enemyHealth; // Referência para verificar se está morto

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
        }

        if (heartsUI == null)
            heartsUI = FindObjectOfType<HeartsHealthUI>();
        // Busca o componente EnemyHealth no mesmo GameObject
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (player == null || heartsUI == null) return;
        
        // NÃO causa dano se o zombie estiver morto
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            return;
        }

        // NÃO causa dano se o zombie estiver morto
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= damageRange)
        {
            damageTimer += Time.deltaTime;

            // Primeiro dano é imediato ao entrar no range
            if (!hasDealtFirstDamage)
            {
                Debug.Log($"[DANO] {gameObject.name} causou dano no player! Distância: {dist:F2}");
                heartsUI.TakeDamage(1);
                hasDealtFirstDamage = true;
                damageTimer = 0f;
            }
            // Danos subsequentes seguem o intervalo configurado
            else if (damageTimer >= damageInterval)
            {
                Debug.Log($"[DANO] {gameObject.name} causou dano no player! Distância: {dist:F2}");
                heartsUI.TakeDamage(1); // meio coração
                damageTimer = 0f;
            }
        }
        else
        {
            damageTimer = 0f; // reseta se afastar
            hasDealtFirstDamage = false; // reseta flag para próximo contato
        }
    }
}
