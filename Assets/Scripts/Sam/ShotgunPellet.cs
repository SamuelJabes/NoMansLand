using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ShotgunPellet : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 8f;                  // velocidade do pellet
    [Min(0.5f)] public float maxRange = 5f;   // alcance curto

    [Header("Vida")]
    public float lifeTimeSafety = 1.2f;       // segurança extra

    [Header("Dano")]
    public int damage = 1;                    // usado pelo EnemyHealth

    private Vector3 spawnPos;
    private float lifeTimer;

    void OnEnable()
    {
        spawnPos = transform.position;
        lifeTimer = 0f;
    }

    void Update()
    {
        // Move no +Y local (igual à Bullet)
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

        // Alcance
        float dist = Vector3.Distance(spawnPos, transform.position);
        if (dist >= maxRange)
        {
            Destroy(gameObject);
            return;
        }

        // Tempo de segurança
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTimeSafety)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se acertou um inimigo
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Verifica se acertou o boss
        BossHealth boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destrói ao colidir com paredes/obstáculos (opcional)
        // Se quiser que atravesse inimigos mas pare em paredes, adicione layer check aqui
    }
}
