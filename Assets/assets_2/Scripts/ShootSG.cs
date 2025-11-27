using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootSG : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform muzzle;                      // opcional
    [Min(0.15f)] public float fireRate = 0.7f;    // tempo entre disparos de cartucho
    [Min(1)] public int pellets = 7;              // quantos proj�teis por tiro
    [Range(0f, 45f)] public float coneDegrees = 12f; // abertura total do cone (graus)

    [Header("Repeticao")]
    [Tooltip("Se true, permite segurar para disparos repetidos respeitando o fireRate. Se false, apenas clique unico.")]
    public bool allowHoldToFire = false;

    [Header("Audio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.04f;

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
        bool wantsFire = GetShootInput();

        if (wantsFire && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireShot();
        }
    }

    bool GetShootInput()
    {
        // Mobile: usar ShootButton
        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsMobile)
        {
            if (shootButton != null)
            {
                return allowHoldToFire ? shootButton.IsPressed : shootButton.WasPressedThisFrame;
            }
        }

        // PC: usar mouse
        return allowHoldToFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
    }

    void FireShot()
    {
        Transform origin = muzzle ? muzzle : transform;
        float baseZ = origin.rotation.eulerAngles.z;

        // 3 pellets fixos: centro, diagonal cima (+15°), diagonal baixo (-15°)
        float[] angles = { 0f, 15f, -15f };

        foreach (float offset in angles)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, baseZ + offset);
            Instantiate(bullet, origin.position, rot);
        }

        if (audioSource && audioSource.clip)
        {
            float oldPitch = audioSource.pitch;
            audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayOneShot(audioSource.clip, volume);
            audioSource.pitch = oldPitch;
        }
    }
}
