using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartsHealthUI : MonoBehaviour
{
    [Header("Hearts setup")]
    public List<Image> heartImages;
    public Sprite fullSprite;
    public Sprite halfSprite;
    public Sprite emptySprite;

    [Header("Health values")]
    public int maxHearts = 3;
    int maxUnits;
    [SerializeField] int currentUnits;

    [Header("Damage feedback")]
    public float flashDuration = 0.18f;
    public Color flashColor = Color.white;
    public AudioSource audioSource;
    public AudioClip hurtSfx;

    [Header("Player Visual Feedback")]
    [Tooltip("Referência ao script PlayerDamageFlash do player")]
    public PlayerDamageFlash playerFlash;

    [Header("Game Over")]
    public string gameOverScene = "Game_Over";
    public float deathDelay = 1f;
    public AudioClip deathSfx;

    void Awake()
    {
        maxUnits = maxHearts * 2;
        if (currentUnits == 0) currentUnits = maxUnits;

        if (heartImages == null || heartImages.Count != maxHearts)
        {
            Debug.LogWarning($"[HeartsHealthUI] heartImages count ({(heartImages == null ? 0 : heartImages.Count)}) != maxHearts ({maxHearts}). Ajuste no Inspector.");
        }

        // Tenta encontrar o PlayerDamageFlash automaticamente se não foi configurado
        if (playerFlash == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerFlash = player.GetComponent<PlayerDamageFlash>();
            }
        }

        UpdateHeartsVisual();
    }

    public void TakeDamage(int units = 1)
    {
        if (units <= 0) return;
        currentUnits = Mathf.Clamp(currentUnits - units, 0, maxUnits);
        UpdateHeartsVisual();

        if (currentUnits <= 0)
        {
            StartCoroutine(HandleDeath());
        }
        else
        {
            // Toca som de dano
            if (audioSource && hurtSfx) audioSource.PlayOneShot(hurtSfx);

            // Flash visual no PLAYER (sprite pisca)
            if (playerFlash != null)
            {
                playerFlash.Flash();
            }

            // Flash nos corações da UI
            StartCoroutine(FlashCurrentHeart());
        }
    }

    IEnumerator HandleDeath()
    {
        Debug.Log("Player morreu!");

        // Flash final no player
        if (playerFlash != null)
        {
            playerFlash.Flash();
        }

        if (audioSource && deathSfx)
        {
            audioSource.PlayOneShot(deathSfx);
        }

        yield return new WaitForSeconds(deathDelay);
        SceneManager.LoadScene(gameOverScene);
    }

    public void Heal(int units = 1)
    {
        if (units <= 0) return;
        currentUnits = Mathf.Clamp(currentUnits + units, 0, maxUnits);
        UpdateHeartsVisual();
    }

    public void SetFullHealth()
    {
        currentUnits = maxUnits;
        UpdateHeartsVisual();
    }

    void UpdateHeartsVisual()
    {
        if (heartImages == null || heartImages.Count == 0)
        {
            Debug.LogWarning("[HeartsHealthUI] heartImages está vazio!");
            return;
        }

        int heartsToUpdate = Mathf.Min(maxHearts, heartImages.Count);

        for (int i = 0; i < heartsToUpdate; i++)
        {
            if (heartImages[i] == null)
            {
                Debug.LogWarning($"[HeartsHealthUI] heartImages[{i}] está null!");
                continue;
            }

            int heartUnits = Mathf.Clamp(currentUnits - i * 2, 0, 2);

            if (heartUnits >= 2)
                heartImages[i].sprite = fullSprite;
            else if (heartUnits == 1)
                heartImages[i].sprite = halfSprite;
            else
                heartImages[i].sprite = emptySprite;

            heartImages[i].SetNativeSize();
        }
    }

    IEnumerator FlashCurrentHeart()
    {
        float t = 0f;
        Color[] original = new Color[heartImages.Count];
        for (int i = 0; i < heartImages.Count; i++) original[i] = heartImages[i].color;

        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = Mathf.PingPong(t * 8f, 1f);

            for (int i = 0; i < heartImages.Count; i++)
            {
                heartImages[i].color = Color.Lerp(original[i], flashColor, lerp * 0.6f);
            }
            yield return null;
        }

        for (int i = 0; i < heartImages.Count; i++) heartImages[i].color = original[i];
    }

    public int GetCurrentUnits() => currentUnits;
    public int GetMaxUnits() => maxUnits;
}