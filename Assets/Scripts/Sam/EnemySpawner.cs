using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public ObjectPool enemyPool;      // pool dos zumbis
    public Transform[] spawnPoints;   // pontos de spawn (podem estar fora do navmesh)
    public float spawnInterval = 2f;

    [Tooltip("Raio de busca para achar o ponto mais próximo no NavMesh.")]
    public float navmeshSearchRadius = 3f;

    [Tooltip("Alvo dos zumbis (normalmente o player).")]
    public Transform target;

    void Start()
    {
        // fallback: se não arrastar o player no Inspector, tenta achar por Tag
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
        if (enemyPool == null || spawnPoints.Length == 0) return;

        GameObject enemy = enemyPool.RequestObjectFromPool();
        if (enemy == null) return;

        var agent = enemy.GetComponent<NavMeshAgent>();
        var enemyAI = enemy.GetComponent<Enemy>();

        // spawn aleatório
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 desiredPos = spawnPoint.position;

        // tenta encaixar no navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(desiredPos, out hit, navmeshSearchRadius, NavMesh.AllAreas))
        {
            Vector3 navmeshPos = hit.position;

            enemy.transform.position = navmeshPos;

            if (agent != null)
            {
                agent.Warp(navmeshPos);
            }

            // seta o alvo pro inimigo (player)
            if (enemyAI != null && target != null)
            {
                enemyAI.SetTarget(target);
            }
        }
        else
        {
            Debug.LogWarning("SpawnPoint fora do NavMesh e não achei posição próxima.");
            enemyPool.ReturnObjectToPool(enemy);
        }
    }
}
