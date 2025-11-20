using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SoundTrigger2D : MonoBehaviour
{
    [Header("Som a ser tocado")]
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Header("Comportamento")]
    [Tooltip("Se true, só toca uma vez e depois desliga o trigger.")]
    public bool playOnlyOnce = false;

    [Tooltip("Tempo mínimo entre ativações (para não spammar).")]
    public float minDelayBetweenPlays = 1.0f;

    [Header("Em qual AudioSource tocar? (opcional)")]
    [Tooltip("Se preencher, usa esse AudioSource. Se deixar vazio, usa PlayClipAtPoint.")]
    public AudioSource audioSourceOverride;

    private float lastPlayTime = -999f;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // garante que é trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Só reage ao Player
        if (!other.CompareTag("Player")) return;

        // Respeita cooldown
        if (Time.time < lastPlayTime + minDelayBetweenPlays) return;

        // Toca o som
        PlaySound();

        lastPlayTime = Time.time;

        if (playOnlyOnce)
        {
            // desativa o trigger pra nunca mais
            col.enabled = false;
        }
    }

    void PlaySound()
    {
        if (clip == null)
        {
            Debug.LogWarning($"[SoundTrigger2D] Sem AudioClip em {name}");
            return;
        }

        if (audioSourceOverride != null)
        {
            // Toca usando um AudioSource específico (ex: do AudioManager)
            float originalPitch = audioSourceOverride.pitch;
            audioSourceOverride.pitch = 1f;
            audioSourceOverride.PlayOneShot(clip, volume);
            audioSourceOverride.pitch = originalPitch;
        }
        else
        {
            // Toca no mundo 2D (sem posição relevante)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
        }
    }

    // Gizmo só pra ver o trigger na cena
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        var c = GetComponent<Collider2D>();
        if (c is BoxCollider2D box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
        else if (c is CircleCollider2D circle)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(circle.offset, circle.radius);
        }
    }
}
