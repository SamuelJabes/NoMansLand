using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VictoryManager : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Nome da cena do menu")]
    public string menuScene = "Main_menu";

    [Header("Score Display")]
    [Tooltip("Texto TMP para mostrar o score final")]
    public TMP_Text finalScoreText;
    
    [Tooltip("Texto TMP para mostrar as moedas finais")]
    public TMP_Text finalCoinsText;
    
    [Tooltip("Prefixo do score (ex: 'Final Score: ')")]
    public string scorePrefix = "Final Score: ";
    
    [Tooltip("Prefixo das moedas (ex: 'Coins: ')")]
    public string coinsPrefix = "Coins: ";

    [Header("Credits")]
    [Tooltip("Texto TMP para mostrar os créditos")]
    public TMP_Text creditsText;
    
    [Tooltip("Nome dos integrantes (um por linha)")]
    [TextArea(5, 10)]
    public string[] teamMembers = new string[]
    {
        "Nome Integrante 1",
        "Nome Integrante 2",
        "Nome Integrante 3",
        "Nome Integrante 4"
    };

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip confirmClip;
    public AudioClip victoryMusic;

    [Header("Menu Button")]
    public Image menuButtonImage;
    public Sprite menuNotPressed;
    public Sprite menuPressed;

    [Header("Animation")]
    public float pressedDuration = 0.2f;

    void Start()
    {
        DisplayFinalScore();
        DisplayCredits();
        PlayVictoryMusic();
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
                finalScoreText.text = $"{scorePrefix}{finalScore:D3}";
            }

            // Atualiza o texto das moedas
            if (finalCoinsText != null)
            {
                finalCoinsText.text = $"{coinsPrefix}{finalCoins:D5}";
            }
        }
        else
        {
            Debug.LogWarning("ScoreManager.Instance não encontrado!");
            if (finalScoreText != null) finalScoreText.text = $"{scorePrefix}000";
            if (finalCoinsText != null) finalCoinsText.text = $"{coinsPrefix}00000";
        }
    }

    void DisplayCredits()
    {
        if (creditsText == null) return;

        // Formata os créditos
        string credits = "=== CREDITS ===\n\n";
        credits += "Developed by:\n\n";
        
        foreach (string member in teamMembers)
        {
            credits += $"• {member}\n";
        }
        
        credits += "\n=== THANK YOU FOR PLAYING ===";
        
        creditsText.text = credits;
    }

    void PlayVictoryMusic()
    {
        if (sfxSource != null && victoryMusic != null)
        {
            sfxSource.clip = victoryMusic;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void BackToMenu()
    {
        if (sfxSource && confirmClip) sfxSource.PlayOneShot(confirmClip);
        
        // Para a música
        if (sfxSource) sfxSource.Stop();
        
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