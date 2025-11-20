using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BossKnife : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 8f;
    public int damage = 10;
    public float lifeTime = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Despawn), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Start()
    {
        // Move a faca na direção "up" do objeto (considerando a rotação)
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora o Boss (verifica se tem a tag antes de comparar)
        if (other.gameObject.CompareTag("Boss"))
            return;

        // Se acertar o player
        if (other.CompareTag("Player"))
        {
            // Busca o sistema de corações (igual aos zombies)
            HeartsHealthUI heartsUI = FindObjectOfType<HeartsHealthUI>();
            if (heartsUI != null)
            {
                // Converte damage (10) para unidades de coração
                // 10 damage = 1 unidade = meio coração
                int heartUnits = Mathf.CeilToInt(damage / 10f);
                heartsUI.TakeDamage(heartUnits);
                Debug.Log($"Faca do Boss acertou! Dano: {heartUnits} unidade(s) de coração");
            }
            else
            {
                Debug.LogWarning("[BossKnife] HeartsHealthUI não encontrado na cena!");
            }
        }

        // Destrói a faca ao colidir com qualquer coisa (exceto Boss)
        Despawn();
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
