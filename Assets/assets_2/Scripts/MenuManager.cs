using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad = "GameScene";

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip confirmClip;

    public void Play()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
        // opcional: aguardar SFX terminar antes de trocar cena (usar coroutine)
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
