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
        // se encostou em inimigo, o EnemyHealth já vai destruir a bala,
        // então aqui não faz nada
        if (other.GetComponentInParent<EnemyHealth>() != null)
            return;

        // parede / props / qualquer coisa -> some
        Despawn();
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
