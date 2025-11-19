using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject prefab;     // Prefab que será instanciado
    public int poolSize = 10;     // Tamanho inicial do pool

    private Queue<GameObject> pool;

    void Awake()
    {
        CreatePool();
    }

    // Cria os objetos e adiciona ao pool
    private void CreatePool()
    {
        pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);

            // garante escala e hierarquia limpas
            obj.transform.SetParent(transform, worldPositionStays: false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            SetupPooledObject(obj);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // Configurações específicas para objetos do pool (ex: EnemyHealth.pool = this)
    private void SetupPooledObject(GameObject obj)
    {
        var enemyHealth = obj.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.pool = this;
        }
    }

    // ========= AQUI: SÓ UMA VEZ =========
    public GameObject RequestObjectFromPool()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
            obj.transform.SetParent(transform, worldPositionStays: false);
            obj.transform.localScale = Vector3.one;
            SetupPooledObject(obj);
        }

        return obj;
    }
    // ====================================

    // Retorna um objeto ao pool
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
