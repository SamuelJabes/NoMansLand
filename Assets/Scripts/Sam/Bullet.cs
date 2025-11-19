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
        // ignora o player, se quiser (deixa só se seu player tiver Tag "Player")
        if (other.CompareTag("Player"))
            return;

        //printar o nome do objeto
        //Debug.Log(other.name);

        // procura EnemyHealth no objeto ou em QUALQUER PAI (ex: Hitbox filho do Zumbi)
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        Debug.Log(enemy.name);
        if (enemy != null)
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
