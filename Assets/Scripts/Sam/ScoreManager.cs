using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score UI")]
    [Tooltip("Arraste aqui o TextMeshPro do placar (ex.: ScoreText).")]
    public TMP_Text scoreText;

    [Tooltip("Prefixo que aparece antes do número.")]
    public string prefix = "Score: ";

    [Tooltip("Quantos dígitos com zero à esquerda (ex.: 2 => 00, 01, 12).")]
    public int minDigits = 2;   // 2 dígitos

    [Header("Coin UI")]
    [Tooltip("Texto TMP que mostra a quantidade de moedas (ex.: CoinText).")]
    public TMP_Text coinText;

    [Tooltip("Dígitos das coins (2 => 00, 01, 12).")]
    public int coinDigits = 2;

    [Tooltip("Quantos pontos de score geram 1 coin.")]
    public int scorePerCoin = 10;

    private int score;
    private int coins;
    private int scoreRemainder;   // acumula até bater scorePerCoin

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // habilite se o placar deve persistir entre cenas

        UpdateScoreUI();
        UpdateCoinUI();
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
        // Se o inimigo tiver um valor próprio de score, usa; senão soma 1
        AddScore(e ? e.scoreValue : 1);
    }

    // =================== SCORE ===================

    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        score += amount;
        if (score < 0) score = 0;

        // acumula para conversão em coins
        scoreRemainder += amount;
        while (scoreRemainder >= scorePerCoin)
        {
            scoreRemainder -= scorePerCoin;
            AddCoins(1);
        }

        UpdateScoreUI();

        // animação de "punch" no score, se existir
        var punch = scoreText ? scoreText.GetComponent<ScoreTextPunch>() : null;
        if (punch) punch.Punch();
    }

    public void ResetScore()
    {
        score = 0;
        scoreRemainder = 0;
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

        // punch opcional na HUD de coin
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
        scoreRemainder = 0;
        UpdateScoreUI();
        UpdateCoinUI();
    }
}
