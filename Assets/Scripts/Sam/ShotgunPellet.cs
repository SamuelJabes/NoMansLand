using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ShotgunPellet : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 8f;              // velocidade do pellet
    [Min(0.5f)] public float maxRange = 5f; // alcance curto

    [Header("Vida")]
    public float lifeTimeSafety = 1.2f;   // só por segurança

    [Header("Dano")]
    public int damage = 1;

    [Header("Colisão")]
    [Tooltip("Layers que podem receber dano (por ex.: Enemy, Walls). Se deixar 0, acerta tudo.")]
    public LayerMask hitMask = ~0;

    [Tooltip("Collider de quem disparou, para não tomar tiro na cara.")]
    public Collider2D ownerCollider;

    private Vector3 spawnPos;
    private float lifeTimer;

    void OnEnable()
    {
        spawnPos = transform.position;
        lifeTimer = 0f;
    }

    void Update()
    {
        // Move no +Y local (igual ao seu Bullet)
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
        // 1) Ignora quem atirou
        if (ownerCollider && other == ownerCollider) return;

        // 2) Ignora outros projéteis NA MESMA LAYER (não colide entre pellets)
        if (other.gameObject.layer == gameObject.layer) return;

        // 3) Se tiver hitMask configurado, filtra por layer
        if (hitMask != (LayerMask)0)
        {
            if (((1 << other.gameObject.layer) & hitMask) == 0)
                return;
        }

        // 4) Se for inimigo, dá dano + feedback (EnemyHealth)
        // procura o EnemyHealth no objeto ou em qualquer pai
        if (other.GetComponentInParent<EnemyHealth>() is EnemyHealth enemy)
        {
            enemy.TakeDamage(damage);
        }


        // 5) Qualquer coisa "relevante" que passou no hitMask → pellet some
        Destroy(gameObject);
    }
}
