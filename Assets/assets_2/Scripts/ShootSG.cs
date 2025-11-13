using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShootSG : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform muzzle;                      // opcional
    [Min(0.15f)] public float fireRate = 0.7f;    // tempo entre disparos de cartucho
    [Min(1)] public int pellets = 7;              // quantos projéteis por tiro
    [Range(0f, 45f)] public float coneDegrees = 12f; // abertura total do cone (graus)

    [Header("Repetição")]
    [Tooltip("Se true, permite segurar para disparos repetidos respeitando o fireRate. Se false, apenas clique único.")]
    public bool allowHoldToFire = false;

    [Header("Áudio")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.5f)] public float pitchVariance = 0.04f;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool wantsFire = allowHoldToFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (wantsFire && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireShot();
        }
    }

    void FireShot()
    {
        Transform origin = muzzle ? muzzle : transform;

        float baseZ = origin.rotation.eulerAngles.z;

        // Distribui pellets no cone. Aqui: random uniforme dentro do cone.
        for (int i = 0; i < pellets; i++)
        {
            float offset = (coneDegrees <= 0f) ? 0f : Random.Range(-coneDegrees * 0.5f, coneDegrees * 0.5f);
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
