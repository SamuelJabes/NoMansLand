using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossArenaTrigger : MonoBehaviour
{
    [Header("Parede invisível que bloqueia o retorno")]
    [Tooltip("Collider da parede invisível entre Área 2 e Área 3 (normalmente um BoxCollider2D, NÃO trigger).")]
    public Collider2D backWallCollider;

    [Header("Colisor que define a Área 3 (boss arena)")]
    [Tooltip("Um Collider2D (geralmente BoxCollider2D) que cobre TODA a arena do boss.")]
    public Collider2D bossAreaCollider;

    [Header("Comportamento")]
    [Tooltip("Se true, esse trigger só funciona uma vez.")]
    public bool onlyOnce = true;

    private bool triggered = false;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // garante que seja trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (onlyOnce && triggered) return;

        triggered = true;

        // 1) Ativa a parede invisível
        if (backWallCollider != null)
        {
            backWallCollider.enabled = true;
            Debug.Log("[BossArenaTrigger] Parede invisível ativada.");
        }
        else
        {
            Debug.LogWarning("[BossArenaTrigger] backWallCollider não atribuído.");
        }

        // 2) Limpa todos os zumbis que NÃO estiverem dentro da Área 3
        if (bossAreaCollider != null)
        {
            EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();

            int cleaned = 0;
            foreach (var e in enemies)
            {
                if (!e.isActiveAndEnabled) continue;

                // Pega posição do transform (pivô do inimigo)
                Vector2 pos = e.transform.position;

                // Se NÃO está dentro do colisor da boss arena → some sem score
                if (!bossAreaCollider.OverlapPoint(pos))
                {
                    e.ForceDespawnWithoutScore();
                    cleaned++;
                }
            }

            Debug.Log($"[BossArenaTrigger] Limpou {cleaned} inimigos fora da boss arena.");
        }
        else
        {
            Debug.LogWarning("[BossArenaTrigger] bossAreaCollider não atribuído.");
        }

        // 3) Opcional: desativa o próprio trigger
        if (onlyOnce)
        {
            col.enabled = false;
        }
    }
}
