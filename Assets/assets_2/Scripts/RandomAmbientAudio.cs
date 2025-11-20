using System.Collections;
using UnityEngine;

public class RandomAmbientAudio : MonoBehaviour
{
    public static RandomAmbientAudio Instance;   // Singletonzinho simples

    [Header("Música de fundo")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    [Header("Sons aleatórios por cima")]
    public AudioSource sfxSource;
    public AudioClip[] randomClips;

    public float minDelay = 10f;
    public float maxDelay = 25f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    [Range(0f, 0.3f)]
    public float pitchVariance = 0.1f;

    void Awake()
    {
        // Singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Se quiser manter o áudio entre cenas:
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Música inicial
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        // Sons aleatórios
        if (sfxSource != null && randomClips != null && randomClips.Length > 0)
        {
            StartCoroutine(RandomSfxLoop());
        }
    }

    IEnumerator RandomSfxLoop()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (randomClips == null || randomClips.Length == 0) continue;

            AudioClip clip = randomClips[Random.Range(0, randomClips.Length)];
            if (clip == null) continue;

            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            sfxSource.PlayOneShot(clip, sfxVolume);
            sfxSource.pitch = originalPitch;
        }
    }

    // ===============================
    //   TROCAR MÚSICA (COM FADE)
    // ===============================
    public void ChangeMusic(AudioClip newClip, float fadeDuration = 1.5f, bool loop = true)
    {
        if (musicSource == null || newClip == null)
        {
            Debug.LogWarning("[RandomAmbientAudio] ChangeMusic chamado sem musicSource ou newClip.");
            return;
        }

        StopCoroutine(nameof(ChangeMusicCoroutine));
        StartCoroutine(ChangeMusicCoroutine(newClip, fadeDuration, loop));
    }

    private IEnumerator ChangeMusicCoroutine(AudioClip newClip, float fadeDuration, bool loop)
    {
        float startVolume = musicSource.volume;

        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}
