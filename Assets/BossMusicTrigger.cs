using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossMusicTrigger : MonoBehaviour
{
    [Header("Música da Boss Fight")]
    public AudioClip bossMusic;

    [Tooltip("Se true, só funciona na primeira vez que o player entra.")]
    public bool onlyOnce = true;

    [Tooltip("Duração do fade entre a música atual e a do boss.")]
    public float fadeDuration = 1.5f;

    private bool triggered = false;

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // garante que seja trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (onlyOnce && triggered) return;

        triggered = true;

        if (RandomAmbientAudio.Instance != null)
        {
            RandomAmbientAudio.Instance.ChangeMusic(bossMusic, fadeDuration, loop: true);
        }
        else
        {
            Debug.LogWarning("[BossMusicTrigger] RandomAmbientAudio.Instance não encontrado na cena!");
        }

        // Se quiser, você pode opcionalmente desativar o gatilho
        // Destroy(gameObject);
        // ou col.enabled = false;
    }
}
