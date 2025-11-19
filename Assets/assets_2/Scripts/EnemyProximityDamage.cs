using UnityEngine;

public class EnemyProximityDamage : MonoBehaviour
{
    [Tooltip("Transform do jogador. Se vazio, procura pelo tag 'Player'.")]
    public Transform player;

    [Tooltip("Sistema de vida (HeartsHealthUI).")]
    public HeartsHealthUI heartsUI;

    [Tooltip("Distância máxima para causar dano.")]
    public float damageRange = 1.5f;

    [Tooltip("Dano em unidades por segundo (1 = meio coração).")]
    public float damageUnitsPerSecond = 1f;

    float damageTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
        }

        if (heartsUI == null)
            heartsUI = FindObjectOfType<HeartsHealthUI>();
    }

    void Update()
    {
        if (player == null || heartsUI == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= damageRange)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 1f / damageUnitsPerSecond)
            {
                heartsUI.TakeDamage(1); // meio coração
                damageTimer = 0f;
            }
        }
        else
        {
            damageTimer = 0f; // reseta se afastar
        }
    }
}
