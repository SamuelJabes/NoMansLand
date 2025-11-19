using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BulletShotgun2D : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 8f;                     // velocidade inicial do pellet
    [Tooltip("Desaceleração por segundo (0 = sem drag).")]
    [Range(0f, 10f)] public float drag = 1.5f;

    [Header("Alcance")]
    [Min(0.5f)] public float maxRange = 5f;      // alcance curto de shotgun
    [Tooltip("Tempo máximo de vida como segurança.")]
    public float lifeTimeSafety = 1.2f;

    [Header("Dano")]
    public int damage = 1;

    [Header("Colisão")]
    [Tooltip("Layers com as quais o pellet colide (Inimigos, Paredes, etc).")]
    public LayerMask collisionMask = ~0;         // tudo, por padrão
    [Tooltip("Opcional: ignorar colisão com quem disparou.")]
    public Collider2D ownerCollider;

    private Vector3 _spawnPos;
    private float _currentSpeed;
    private float _lifeTimer;

    void OnEnable()
    {
        _spawnPos = transform.position;
        _currentSpeed = speed;
        _lifeTimer = 0f;

        if (ownerCollider)
        {
            var myCol = GetComponent<Collider2D>();
            if (myCol) Physics2D.IgnoreCollision(myCol, ownerCollider, true);
        }
    }

    void OnDisable()
    {
        if (ownerCollider)
        {
            var myCol = GetComponent<Collider2D>();
            if (myCol) Physics2D.IgnoreCollision(myCol, ownerCollider, false);
        }
    }

    void Update()
    {
        // move no +Y local (igual ao seu Bullet original)
        transform.Translate(Vector3.up * _currentSpeed * Time.deltaTime, Space.Self);

        // drag simples
        if (drag > 0f)
        {
            float k = Mathf.Clamp01(drag * Time.deltaTime);
            _currentSpeed = Mathf.Max(0f, _currentSpeed * (1f - k));
        }

        // checa alcance
        float dist = Vector3.Distance(_spawnPos, transform.position);
        if (dist >= maxRange)
        {
            Destroy(gameObject);
            return;
        }

        // lifeTime de segurança
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= lifeTimeSafety)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // respeita collisionMask
        if (((1 << other.gameObject.layer) & collisionMask) == 0)
            return;

        // ignora o dono
        if (ownerCollider && other == ownerCollider) return;

        if (other.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
