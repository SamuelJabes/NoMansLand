using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;
    public float lifeTime = 2.5f;

    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Despawn), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage);
        }
        Despawn();
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
