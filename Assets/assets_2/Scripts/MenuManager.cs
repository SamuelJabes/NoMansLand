using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad = "GameScene";

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip confirmClip;

    [Header("Background Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;

    void Start()
    {
        // Toca a música de fundo do menu
        if (musicSource && menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true; // Loop infinito
            musicSource.Play();
        }
    }

    public void Play()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
        // Para a música antes de trocar de cena
        if (musicSource) musicSource.Stop();
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}