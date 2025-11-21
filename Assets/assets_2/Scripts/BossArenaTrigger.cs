using UnityEngine;
using System.Collections.Generic;

public class BossArenaTrigger : MonoBehaviour
{
    [Header("Referências de Arena")]
    [Tooltip("Parede invisível que impede voltar para a área 2 (BoxCollider2D, NÃO trigger).")]
    public Collider2D backWallCollider;

    [Tooltip("Collider 2D que representa TODA a área da arena do boss (deve ser trigger).")]
    public Collider2D bossAreaCollider;

    [Tooltip("Se marcado, só dispara uma vez.")]
    public bool onlyOnce = true;

    private bool triggered = false;

    [Header("Spawn do Boss")]
    [Tooltip("Prefab do boss (se quiser instanciar quando entrar). Opcional se usar bossInstance.")]
    public GameObject bossPrefab;

    [Tooltip("Boss já na cena, mas DESATIVADO no início. Opcional se usar bossPrefab.")]
    public GameObject bossInstance;

    [Tooltip("Ponto onde o boss deve nascer / ser colocado.")]
    public Transform bossSpawnPoint;

    private bool bossSpawned = false;

    [Header("Opções de Limpeza de Inimigos")]
    [Tooltip("Se marcado, inimigos fora da bossAreaCollider serão despawnados ao entrar.")]
    public bool despawnEnemiesOutsideArena = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (onlyOnce && triggered) return;
        triggered = true;

        Debug.Log("[BossArenaTrigger] Player entrou na arena do boss.");

        // 1) Ativa parede invisível para impedir retorno
        if (backWallCollider != null)
        {
            backWallCollider.enabled = true;
            Debug.Log("[BossArenaTrigger] Parede invisível ativada.");
        }

        // 2) Limpa zumbis fora da arena (voltam pro pool)
        if (despawnEnemiesOutsideArena && bossAreaCollider != null)
        {
            DespawnEnemiesOutsideArena();
        }

        // 3) Spawna / ativa o boss
        SpawnBossIfNeeded();
    }

    private void DespawnEnemiesOutsideArena()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        int countDespawned = 0;

        foreach (var e in enemies)
        {
            if (e == null || !e.isActiveAndEnabled) continue;

            Vector2 pos = e.transform.position;

            // Se NÃO está dentro da área do boss → some
            if (!bossAreaCollider.OverlapPoint(pos))
            {
                e.ForceDespawnWithoutScore(); // método que você já tem no EnemyHealth
                countDespawned++;
            }
        }

        Debug.Log($"[BossArenaTrigger] Despawnados {countDespawned} inimigos fora da arena do boss.");
    }

    private void SpawnBossIfNeeded()
    {
        if (bossSpawned) return;

        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;
        Quaternion spawnRot = Quaternion.identity;

        if (bossInstance != null)
        {
            // Caso 1: boss já existe na cena, só ativar
            bossInstance.transform.position = spawnPos;
            bossInstance.transform.rotation = spawnRot;
            bossInstance.SetActive(true);

            Debug.Log("[BossArenaTrigger] BossInstance ativado.");
        }
        else if (bossPrefab != null)
        {
            // Caso 2: instanciar o boss a partir de um prefab
            bossInstance = Instantiate(bossPrefab, spawnPos, spawnRot);
            Debug.Log("[BossArenaTrigger] BossPrefab instanciado.");
        }
        else
        {
            Debug.LogWarning("[BossArenaTrigger] Nenhum bossPrefab ou bossInstance definido!");
        }

        bossSpawned = true;
    }
}
