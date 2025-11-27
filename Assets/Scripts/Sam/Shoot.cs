using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public float fireRate = 0.2f; // intervalo entre tiros (em segundos)

    [Header("�udio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.05f;

    [Header("Mobile")]
    public ShootButton shootButton;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Tenta encontrar o botão de tiro automaticamente
        if (shootButton == null)
        {
            shootButton = FindObjectOfType<ShootButton>();
        }
    }

    void Update()
    {
        bool wantsToShoot = GetShootInput();

        if (wantsToShoot && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Instantiate(bullet, transform.position, transform.rotation);

            if (audioSource != null && audioSource.clip != null)
            {
                float oldPitch = audioSource.pitch;
                audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
                audioSource.PlayOneShot(audioSource.clip, volume);
                audioSource.pitch = oldPitch;
            }
        }
    }

    bool GetShootInput()
    {
        // Mobile: usa botão de tiro
        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsMobile)
        {
            if (shootButton != null)
                return shootButton.IsPressed;
        }

        // PC: usa mouse
        return Input.GetMouseButton(0);
    }
}