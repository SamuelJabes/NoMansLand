using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score UI")]
    [Tooltip("Arraste aqui o TextMeshPro do placar (ex.: ScoreText).")]
    public TMP_Text scoreText;

    [Tooltip("Prefixo que aparece antes do número.")]
    public string prefix = "Score: ";

    [Tooltip("Quantos dígitos com zero à esquerda (ex.: 3 => 000, 001, 012).")]
    public int minDigits = 3;

    [Header("Coin UI")]
    [Tooltip("Texto TMP que mostra a quantidade de moedas (ex.: CoinText).")]
    public TMP_Text coinText;

    [Tooltip("Dígitos das coins (5 => 00000, 00001, 00123).")]
    public int coinDigits = 5;

    [Header("Conversão")]
    [Tooltip("Quantas moedas ganha para cada 1 ponto de score.")]
    public int coinsPerScore = 10;

    [Header("UI References (Auto-find)")]
    [Tooltip("Nome do GameObject que contém o ScoreText")]
    public string scoreTextObjectName = "ScoreText";

    [Tooltip("Nome do GameObject que contém o CoinText")]
    public string coinTextObjectName = "CoinText";

    private int score;
    private int coins;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // Inscreve-se no evento de cena carregada
        SceneManager.sceneLoaded += OnSceneLoaded;

        UpdateScoreUI();
        UpdateCoinUI();
    }

    void OnDestroy()
    {
        // Remove a inscrição quando destruir
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado toda vez que uma cena é carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Cena carregada: {scene.name}");

        // Reconecta as referências UI
        ReconnectUI();

        // Atualiza a UI com os valores atuais
        UpdateScoreUI();
        UpdateCoinUI();
    }

    // Tenta encontrar e reconectar os textos UI na cena atual
    void ReconnectUI()
    {
        // Procura pelo ScoreText
        if (scoreText == null || scoreText.gameObject.scene != SceneManager.GetActiveScene())
        {
            GameObject scoreObj = GameObject.Find(scoreTextObjectName);
            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<TMP_Text>();
                Debug.Log($"ScoreText reconectado: {scoreText != null}");
            }
            else
            {
                Debug.LogWarning($"Não foi possível encontrar GameObject '{scoreTextObjectName}'");
            }
        }

        // Procura pelo CoinText
        if (coinText == null || coinText.gameObject.scene != SceneManager.GetActiveScene())
        {
            GameObject coinObj = GameObject.Find(coinTextObjectName);
            if (coinObj != null)
            {
                coinText = coinObj.GetComponent<TMP_Text>();
                Debug.Log($"CoinText reconectado: {coinText != null}");
            }
            else
            {
                Debug.LogWarning($"Não foi possível encontrar GameObject '{coinTextObjectName}'");
            }
        }
    }

    void OnEnable()
    {
        EnemyHealth.OnAnyEnemyDied += OnEnemyDied;
    }

    void OnDisable()
    {
        EnemyHealth.OnAnyEnemyDied -= OnEnemyDied;
    }

    private void OnEnemyDied(EnemyHealth e)
    {
        AddScore(e ? e.scoreValue : 1);
    }

    // =================== SCORE ===================

    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        score += amount;
        if (score < 0) score = 0;

        int coinsToAdd = amount * Mathf.Max(0, coinsPerScore);
        if (coinsToAdd > 0)
        {
            AddCoins(coinsToAdd);
        }

        UpdateScoreUI();

        var punch = scoreText ? scoreText.GetComponent<ScoreTextPunch>() : null;
        if (punch) punch.Punch();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    public int CurrentScore => score;

    void UpdateScoreUI()
    {
        if (!scoreText) return;
        string formatted = score.ToString("D" + Mathf.Max(1, minDigits));
        scoreText.text = $"{prefix}{formatted}";
    }

    // =================== COINS ===================

    void AddCoins(int amount)
    {
        coins += amount;
        if (coins < 0) coins = 0;

        UpdateCoinUI();

        var punch = coinText ? coinText.GetComponent<ScoreTextPunch>() : null;
        if (punch) punch.Punch();
    }

    public bool TrySpendCoins(int amount)
    {
        if (amount <= 0) return true;
        if (coins < amount) return false;

        coins -= amount;
        UpdateCoinUI();
        return true;
    }

    public void ResetCoins()
    {
        coins = 0;
        UpdateCoinUI();
    }

    public int CurrentCoins => coins;

    void UpdateCoinUI()
    {
        if (!coinText) return;
        string formatted = coins.ToString("D" + Mathf.Max(1, coinDigits));
        coinText.text = formatted;
    }

    public void ResetAll()
    {
        score = 0;
        coins = 0;
        UpdateScoreUI();
        UpdateCoinUI();
    }
}