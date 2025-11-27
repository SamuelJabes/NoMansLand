using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootMG : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform muzzle;                   // opcional
    [Min(0.03f)] public float fireInterval = 0.08f; // menor intervalo entre tiros (DPS)
    [Tooltip("Pequena dispers�o por tiro (graus). 0 = sem spread")]
    [Range(0f, 10f)] public float spreadDegrees = 2.0f;

    [Header("Áudio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.03f;

    [Header("Mobile")]
    public ShootButton shootButton;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Auto-discovery do ShootButton se não conectado manualmente
        if (shootButton == null)
        {
            shootButton = FindObjectOfType<ShootButton>();
        }
    }

    void Update()
    {
        // AUTOMÁTICO: pode segurar para atirar
        if (GetShootInput() && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            FireOne();
        }
    }

    bool GetShootInput()
    {
        // Mobile: usar ShootButton
        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsMobile)
        {
            if (shootButton != null)
            {
                return shootButton.IsPressed; // MG é sempre automático (hold)
            }
        }

        // PC: usar mouse
        return Input.GetMouseButton(0);
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
