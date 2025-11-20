using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner2Types : MonoBehaviour
{
    [Header("Pools de Inimigos")]
    public ObjectPool smallZombiePool;     // zumbi pequeno
    public ObjectPool bigZombiePool;       // zumbi grande

    [Header("Spawn Points por Área")]
    public Transform[] area1Points;
    public Transform[] area2Points;
    public Transform[] area3Points;

    [Header("Desbloqueio de Áreas")]
    [Tooltip("Sala inicial liberada por padrão.")]
    public bool area1Unlocked = true;
    public bool area2Unlocked = false;
    public bool area3Unlocked = false;

    [Header("Área atual do jogador")]
    [Tooltip("1 = sala 1, 2 = sala 2, 3 = sala 3.")]
    public int currentArea = 1;

    [Header("Config de Spawn (por progresso)")]
    [Tooltip("Intervalo de spawn enquanto só a área 1 está liberada.")]
    public float spawnIntervalArea1 = 2f;

    [Tooltip("Intervalo de spawn quando a área 2 foi liberada.")]
    public float spawnIntervalArea2 = 1.5f;

    [Tooltip("Intervalo de spawn quando a área 3 foi liberada.")]
    public float spawnIntervalArea3 = 1.0f;

    [Header("Chance de Zumbi Grande")]
    [Range(0f, 1f)]
    public float bigZombieChance = 0.3f;

    [Header("Limite de Spawn")]
    [Tooltip("Máximo de zombies spawnados por ciclo (evita esgotar o pool).")]
    public int maxSpawnsPerCycle = 5;

    [Header("NavMesh")]
    [Tooltip("Raio para projetar o spawn no NavMesh.")]
    public float navmeshSearchRadius = 3f;

    [Header("Camera / Visibilidade")]
    [Tooltip("Margem além dos limites da câmera para considerar 'fora da tela'.")]
    public float offscreenMargin = 1f;

    [Header("Alvo dos zumbis (player)")]
    public Transform target;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnFromAllPoints();
            yield return new WaitForSeconds(GetCurrentSpawnInterval());
        }
    }

    /// <summary>
    /// Retorna o intervalo de spawn atual baseado nas áreas desbloqueadas.
    /// </summary>
    float GetCurrentSpawnInterval()
    {
        if (area3Unlocked) return spawnIntervalArea3;
        if (area2Unlocked) return spawnIntervalArea2;
        return spawnIntervalArea1;
    }

    /// <summary>
    /// Chamado pelas portas / triggers de sala quando uma nova área é liberada.
    /// </summary>
    public void UnlockArea(int areaIndex)
    {
        switch (areaIndex)
        {
            case 1: area1Unlocked = true; break;
            case 2: area2Unlocked = true; break;
            case 3: area3Unlocked = true; break;
        }

        Debug.Log($"[EnemySpawner2Types] Área {areaIndex} desbloqueada!");
    }

    /// <summary>
    /// Chamado por um trigger de área para indicar em qual sala o player está.
    /// </summary>
    public void SetCurrentArea(int areaIndex)
    {
        currentArea = Mathf.Clamp(areaIndex, 1, 3);
        Debug.Log($"[EnemySpawner2Types] Player agora está na área {currentArea}");
    }

    /// <summary>
    /// Spawna zombies em spawn points offscreen da área atual (limitado por maxSpawnsPerCycle).
    /// </summary>
    void SpawnFromAllPoints()
    {
        var offscreenPoints = GetOffscreenPointsFromCurrentArea();
        if (offscreenPoints.Count == 0)
            return;

        // Embaralha e limita quantidade
        Shuffle(offscreenPoints);
        int spawnCount = Mathf.Min(offscreenPoints.Count, maxSpawnsPerCycle);

        for (int idx = 0; idx < spawnCount; idx++)
        {
            var sp = offscreenPoints[idx];
            if (sp == null) continue;

            // Escolhe o pool (grande ou pequeno) pra ESTE spawn
            ObjectPool chosenPool = null;
            float r = Random.value;
            if (bigZombiePool != null && r < bigZombieChance)
                chosenPool = bigZombiePool;
            else
                chosenPool = smallZombiePool;

            if (chosenPool == null)
            {
                Debug.LogWarning("[EnemySpawner2Types] Nenhum pool válido configurado.");
                continue;
            }

            GameObject enemy = chosenPool.RequestObjectFromPool();
            if (enemy == null)
            {
                // pool acabou, cancela este spawn
                continue;
            }


            var agent = enemy.GetComponent<NavMeshAgent>();
            var enemyAI = enemy.GetComponent<Enemy>();

            Vector3 desiredPos = sp.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(desiredPos, out hit, navmeshSearchRadius, NavMesh.AllAreas))
            {
                Vector3 navmeshPos = hit.position;

                enemy.transform.position = navmeshPos;

                if (agent != null)
                {
                    agent.Warp(navmeshPos);
                }

                if (enemyAI != null && target != null)
                {
                    enemyAI.SetTarget(target);
                }
            }
            else
            {
                // Se não encontrar NavMesh perto, devolve pro pool
                chosenPool.ReturnObjectToPool(enemy);
            }
        }
    }

    List<Transform> GetOffscreenPointsFromCurrentArea()
    {
        var result = new List<Transform>();
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return result;

        Vector3 min = mainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 max = mainCam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float minX = min.x - offscreenMargin;
        float maxX = max.x + offscreenMargin;
        float minY = min.y - offscreenMargin;
        float maxY = max.y + offscreenMargin;

        Transform[] points = null;
        bool unlocked = false;

        switch (currentArea)
        {
            case 1:
                points = area1Points;
                unlocked = area1Unlocked;
                break;
            case 2:
                points = area2Points;
                unlocked = area2Unlocked;
                break;
            case 3:
                points = area3Points;
                unlocked = area3Unlocked;
                break;
        }

        if (!unlocked || points == null) return result;

        foreach (var sp in points)
        {
            if (sp == null) continue;

            Vector3 p = sp.position;
            bool off =
                p.x < minX ||
                p.x > maxX ||
                p.y < minY ||
                p.y > maxY;

            if (off)
                result.Add(sp);
        }

        return result;
    }

    /// <summary>
    /// Embaralha uma lista usando Fisher-Yates shuffle.
    /// </summary>
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
