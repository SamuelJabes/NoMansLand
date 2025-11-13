using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootPistol : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform muzzle;              // opcional
    [Min(0.01f)] public float fireRate = 0.35f; // tempo mínimo entre cliques

    [Header("Áudio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.05f;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // SEMI-AUTO: apenas no clique, não atira segurando
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireOne();
        }
    }

    void FireOne()
    {
        Transform origin = muzzle ? muzzle : transform;
        Instantiate(bullet, origin.position, origin.rotation);

        if (audioSource && audioSource.clip)
        {
            float oldPitch = audioSource.pitch;
            audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(audioSource.clip, volume);
            audioSource.pitch = oldPitch;
        }
    }
}
