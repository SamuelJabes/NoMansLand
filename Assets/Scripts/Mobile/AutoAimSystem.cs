using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sistema de auto-aim para mobile.
/// Detecta o inimigo mais próximo e fornece direção para mirar.
/// </summary>
public class AutoAimSystem : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Alcance máximo para detectar inimigos")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("Tags dos inimigos a serem detectados")]
    [SerializeField] private string[] enemyTags = { "Enemy", "Boss" };

    [Tooltip("Layers dos inimigos")]
    [SerializeField] private LayerMask enemyLayers = ~0;

    [Tooltip("Ângulo máximo de auto-aim (graus). 0 = 360° completo")]
    [Range(0f, 180f)]
    [SerializeField] private float maxAimAngle = 0f; // 0 = desabilitado (mira em qualquer direção)

    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = Color.red;

    [Header("Estado (Read-Only)")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float distanceToTarget;

    // Cache
    private Transform playerTransform;
    private List<Transform> potentialTargets = new List<Transform>();

    public Transform CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null;
    public Vector2 AimDirection => HasTarget ? (Vector2)(currentTarget.position - playerTransform.position).normalized : Vector2.zero;

    void Awake()
    {
        playerTransform = transform;
    }

    void Update()
    {
        UpdateTarget();
    }

    /// <summary>
    /// Atualiza o alvo atual procurando o inimigo mais próximo
    /// </summary>
    void UpdateTarget()
    {
        potentialTargets.Clear();
        currentTarget = null;
        distanceToTarget = float.MaxValue;

        // Busca todos inimigos por tag
        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                // Verifica se o inimigo está vivo (não está desativado)
                if (!enemy.activeInHierarchy) continue;

                // Verifica se tem EnemyHealth e se está morto
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null && enemyHealth.IsDead()) continue;

                // Verifica se tem BossHealth e se está morto
                BossHealth bossHealth = enemy.GetComponent<BossHealth>();
                if (bossHealth != null && bossHealth.IsDead()) continue;

                potentialTargets.Add(enemy.transform);
            }
        }

        // Se não encontrou nenhum inimigo, retorna
        if (potentialTargets.Count == 0) return;

        // Encontra o mais próximo dentro do alcance
        float closestDistance = detectionRange;
        Transform closestEnemy = null;

        foreach (Transform enemy in potentialTargets)
        {
            float distance = Vector2.Distance(playerTransform.position, enemy.position);

            // Verifica se está dentro do alcance
            if (distance > detectionRange) continue;

            // Verifica ângulo se configurado
            if (maxAimAngle > 0f)
            {
                Vector2 directionToEnemy = (enemy.position - playerTransform.position).normalized;
                float angleToEnemy = Vector2.Angle(playerTransform.up, directionToEnemy);
                
                if (angleToEnemy > maxAimAngle) continue;
            }

            // Verifica se é o mais próximo
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        currentTarget = closestEnemy;
        distanceToTarget = closestDistance;
    }

    /// <summary>
    /// Retorna a direção normalizada para o alvo atual
    /// </summary>
    public Vector2 GetAimDirection()
    {
        return AimDirection;
    }

    /// <summary>
    /// Retorna a posição do alvo atual
    /// </summary>
    public Vector3 GetTargetPosition()
    {
        return HasTarget ? currentTarget.position : playerTransform.position;
    }

    /// <summary>
    /// Força atualização do alvo (útil quando inimigo morre)
    /// </summary>
    public void ForceUpdateTarget()
    {
        UpdateTarget();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying) return;

        // Desenha alcance de detecção
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Desenha linha para o alvo atual
        if (HasTarget)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            
            // Desenha esfera no alvo
            Gizmos.DrawWireSphere(currentTarget.position, 0.5f);
        }
    }
#endif
}
