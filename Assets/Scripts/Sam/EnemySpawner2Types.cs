using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner2Types : MonoBehaviour
{
    [Header("Pools")]
    public ObjectPool smallZombiePool;     // pool do zumbi pequeno
    public ObjectPool bigZombiePool;       // pool do zumbi grande

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Spawn Config")]
    public float spawnInterval = 2f;
    [Range(0f, 1f)]
    public float bigZombieChance = 0.3f;   // 30% de chance de vir um grande

    [Tooltip("Raio de busca para projetar no NavMesh.")]
    public float navmeshSearchRadius = 3f;

    [Header("Camera / Visibilidade")]
    [Tooltip("Margem além dos limites da câmera para considerar 'fora da tela'.")]
    public float offscreenMargin = 1f;

    [Header("Alvo dos zumbis")]
    public Transform target; // normalmente o player

    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        // fallback para achar player por Tag, se não arrastar no Inspector
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
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOne()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        // 1) filtra apenas spawnPoints fora da câmera
        var offscreenPoints = GetOffscreenSpawnPoints();
        if (offscreenPoints.Count == 0)
        {
            // nenhum ponto fora da tela agora → não spawna neste ciclo
            return;
        }

        // 2) escolhe pool (grande ou pequeno)
        ObjectPool chosenPool = null;
        float r = Random.value;
        if (bigZombiePool != null && r < bigZombieChance)
            chosenPool = bigZombiePool;
        else
            chosenPool = smallZombiePool;

        if (chosenPool == null)
        {
            Debug.LogWarning("[EnemySpawner2Types] Nenhum pool válido configurado.");
            return;
        }

        // 3) pega inimigo do pool
        GameObject enemy = chosenPool.RequestObjectFromPool();
        if (enemy == null) return;

        var agent = enemy.GetComponent<NavMeshAgent>();
        var enemyAI = enemy.GetComponent<Enemy>();

        // 4) escolhe um spawnPoint fora da câmera
        Transform sp = offscreenPoints[Random.Range(0, offscreenPoints.Count)];
        Vector3 desiredPos = sp.position;

        // 5) projeta para o NavMesh
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
            // não achou NavMesh perto → devolve pro pool
            Debug.LogWarning("[EnemySpawner2Types] SpawnPoint fora do NavMesh, devolvendo inimigo ao pool.");
            chosenPool.ReturnObjectToPool(enemy);
        }
    }

    // Retorna só os spawnPoints que estão fora da visão da câmera
    List<Transform> GetOffscreenSpawnPoints()
    {
        var result = new List<Transform>();
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return result;

        // bounds da câmera em world space
        Vector3 min = mainCam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 max = mainCam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float minX = min.x - offscreenMargin;
        float maxX = max.x + offscreenMargin;
        float minY = min.y - offscreenMargin;
        float maxY = max.y + offscreenMargin;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform sp = spawnPoints[i];
            if (sp == null) continue;

            Vector3 p = sp.position;

            // está fora da tela se está fora do retângulo expandido
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
}
