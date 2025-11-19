using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootMG : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform muzzle;                   // opcional
    [Min(0.03f)] public float fireInterval = 0.08f; // menor intervalo entre tiros (DPS)
    [Tooltip("Pequena dispersão por tiro (graus). 0 = sem spread")]
    [Range(0f, 10f)] public float spreadDegrees = 2.0f;

    [Header("Áudio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.03f;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // AUTOMÁTICO: pode segurar para atirar
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            FireOne();
        }
    }

    void FireOne()
    {
        Transform origin = muzzle ? muzzle : transform;

        // aplica pequeno spread ao Z (top-down 2D)
        float z = origin.rotation.eulerAngles.z;
        float jitter = (spreadDegrees > 0f) ? Random.Range(-spreadDegrees * 0.5f, spreadDegrees * 0.5f) : 0f;
        Quaternion rot = Quaternion.Euler(0f, 0f, z + jitter);

        Instantiate(bullet, origin.position, rot);

        if (audioSource && audioSource.clip)
        {
            float oldPitch = audioSource.pitch;
            audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(audioSource.clip, volume);
            audioSource.pitch = oldPitch;
        }
    }
}
