using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Índice da cena do jogo no Build Settings (geralmente 1)")]
    public int gameSceneIndex = 1; // índice da tete_mapa no Build Settings
    
    [Tooltip("Nome da cena do menu")]
    public string menuScene = "Main_menu";

    [Header("Score Display")]
    [Tooltip("Texto TMP para mostrar o score final")]
    public TMP_Text finalScoreText;
    
    [Tooltip("Texto TMP para mostrar as moedas finais")]
    public TMP_Text finalCoinsText;
    
    [Tooltip("Prefixo do score (ex: 'Score: ')")]
    public string scorePrefix = "Score: ";
    
    [Tooltip("Prefixo das moedas (ex: 'Coins: ')")]
    public string coinsPrefix = "Coins: ";

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip confirmClip;

    [Header("Replay Button")]
    public Image replayButtonImage;
    public Sprite replayNotPressed;
    public Sprite replayPressed;

    [Header("Menu Button")]
    public Image menuButtonImage;
    public Sprite menuNotPressed;
    public Sprite menuPressed;

    [Header("Animation")]
    public float pressedDuration = 0.2f;

    void Start()
    {
        DisplayFinalScore();
    }

    void DisplayFinalScore()
    {
        // Pega o score final do ScoreManager
        if (ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.CurrentScore;
            int finalCoins = ScoreManager.Instance.CurrentCoins;

            // Atualiza o texto do score
            if (finalScoreText != null)
            {
                finalScoreText.text = $"{scorePrefix}{finalScore:D3}"; // Formata com 3 dígitos
            }

            // Atualiza o texto das moedas
            if (finalCoinsText != null)
            {
                finalCoinsText.text = $"{coinsPrefix}{finalCoins:D5}"; // Formata com 5 dígitos
            }
        }
        else
        {
            Debug.LogWarning("ScoreManager.Instance não encontrado!");
            if (finalScoreText != null) finalScoreText.text = $"{scorePrefix}000";
            if (finalCoinsText != null) finalCoinsText.text = $"{coinsPrefix}00000";
        }
    }

    public void Replay()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
        
        // Reseta o score antes de reiniciar o jogo
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetAll();
        }
        
        StartCoroutine(ButtonPressAnimation(replayButtonImage, replayPressed, replayNotPressed, () => SceneManager.LoadScene(gameSceneIndex)));
    }

    public void BackToMenu()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
        
        // Reseta o score ao voltar ao menu
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetAll();
        }
        
        StartCoroutine(ButtonPressAnimation(menuButtonImage, menuPressed, menuNotPressed, () => SceneManager.LoadScene(menuScene)));
    }

    private IEnumerator ButtonPressAnimation(Image buttonImage, Sprite pressedSprite, Sprite normalSprite, System.Action onComplete)
    {
        if (buttonImage != null && pressedSprite != null)
        {
            // Troca para sprite pressionado
            buttonImage.sprite = pressedSprite;
            
            // Aguarda a duração configurada
            yield return new WaitForSeconds(pressedDuration);
            
            // Volta para sprite normal
            buttonImage.sprite = normalSprite;
        }
        
        // Executa a ação (trocar cena)
        onComplete?.Invoke();
    }
}