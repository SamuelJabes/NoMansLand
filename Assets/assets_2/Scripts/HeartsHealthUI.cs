using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HeartsHealthUI : MonoBehaviour
{
    [Header("Hearts setup")]
    public List<Image> heartImages; // arraste Heart0, Heart1, Heart2 aqui (ordem esquerda -> direita)
    public Sprite fullSprite; // heart_full
    public Sprite halfSprite; // heart_half
    public Sprite emptySprite; // heart_empty

    [Header("Health values")]
    public int maxHearts = 3;       // quantos corações visíveis (3)
    int maxUnits;                  // maxHearts * 2
    [SerializeField] int currentUnits; // vida em unidades de meio-coração (0..maxUnits)

    [Header("Damage feedback")]
    public float flashDuration = 0.18f;
    public Color flashColor = Color.white;
    public AudioSource audioSource;
    public AudioClip hurtSfx; // opcional

    [Header("Game Over")]
    public string gameOverScene = "Main_menu"; // nome da cena para voltar ao menu
    public float deathDelay = 1f; // delay antes de voltar ao menu
    public AudioClip deathSfx; // som de morte (opcional)

    void Awake()
    {
        maxUnits = maxHearts * 2;
        // inicia com vida cheia (padrão). Se quiser começar diferente, set currentUnits no inspector.
        if (currentUnits == 0) currentUnits = maxUnits;
        // sanity check: heartImages deve ter o mesmo número que maxHearts.
        if (heartImages == null || heartImages.Count != maxHearts)
        {
            Debug.LogWarning($"[HeartsHealthUI] heartImages count ({(heartImages==null?0:heartImages.Count)}) != maxHearts ({maxHearts}). Ajuste no Inspector.");
        }

        UpdateHeartsVisual();
    }

    // chamada pública: leva 'units' (meio-coração = 1). Ex: TakeDamage(1) tira meio coração.
    public void TakeDamage(int units = 1)
    {
        if (units <= 0) return;
        currentUnits = Mathf.Clamp(currentUnits - units, 0, maxUnits);
        UpdateHeartsVisual();
        
        // Verifica se morreu
        if (currentUnits <= 0)
        {
            StartCoroutine(HandleDeath());
        }
        else
        {
            if (audioSource && hurtSfx) audioSource.PlayOneShot(hurtSfx);
            // feedback visual: piscar o coração que foi atingido
            StartCoroutine(FlashCurrentHeart());
        }
    }

    IEnumerator HandleDeath()
    {
        Debug.Log("Player morreu!");
        
        // Toca som de morte (se configurado)
        if (audioSource && deathSfx) 
        {
            audioSource.PlayOneShot(deathSfx);
        }
        
        // Opcional: desabilitar controles do player aqui
        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        // if (player != null)
        // {
        //     // Desabilita scripts de movimento
        //     var movement = player.GetComponent<PlayerMovement>();
        //     if (movement) movement.enabled = false;
        // }
        
        // Aguarda o delay
        yield return new WaitForSeconds(deathDelay);
        
        // Volta para o menu ou reinicia a cena
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

    // Atualiza cada heartImage de acordo com currentUnits (0..maxUnits)
    void UpdateHeartsVisual()
    {
        for (int i = 0; i < maxHearts; i++)
        {
            // unidades correspondentes a este heart: possible values 0,1,2
            int heartUnits = Mathf.Clamp(currentUnits - i * 2, 0, 2);
            // heartUnits == 2 -> full | ==1 -> half | ==0 -> empty
            if (heartUnits >= 2)
                heartImages[i].sprite = fullSprite;
            else if (heartUnits == 1)
                heartImages[i].sprite = halfSprite;
            else
                heartImages[i].sprite = emptySprite;

            // assegurar que a imagem seja visível mesmo se sprites tiverem form sizes diferentes
            heartImages[i].SetNativeSize();
        }
    }

    // pisca o último coração que mudou (visual feedback). Simples: pisca todos por enquanto.
    IEnumerator FlashCurrentHeart()
    {
        float t = 0f;
        // guarda cores originais
        Color[] original = new Color[heartImages.Count];
        for (int i = 0; i < heartImages.Count; i++) original[i] = heartImages[i].color;

        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = Mathf.PingPong(t * 8f, 1f); // pulso rápido
            Color col = Color.Lerp(original[0], flashColor, lerp); // usa a primeira cor como base
            for (int i = 0; i < heartImages.Count; i++)
            {
                heartImages[i].color = Color.Lerp(original[i], flashColor, lerp * 0.6f);
            }
            yield return null;
        }

        // restaurar cores
        for (int i = 0; i < heartImages.Count; i++) heartImages[i].color = original[i];
    }

    // expose getters
    public int GetCurrentUnits() => currentUnits;
    public int GetMaxUnits() => maxUnits;
}