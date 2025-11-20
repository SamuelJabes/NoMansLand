using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Sistema de portas/veículos bloqueantes que podem ser comprados com moedas.
/// Similar ao WeaponStation mas para desbloqueio de áreas.
/// </summary>
public class DoorPurchase : MonoBehaviour
{
    [Header("Purchase Settings")]
    [SerializeField] private int price = 500;
    [SerializeField] private string doorName = "Passagem Norte"; // Nome para exibir na UI
    
    [Header("UI References")]
    [SerializeField] private GameObject interactionUI; // Panel de UI que aparece ao se aproximar
    [SerializeField] private TextMeshProUGUI messageText; // Texto da mensagem
    
    [Header("Visual Effects")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 0.5f); // Amarelo semi-transparente
    [SerializeField] private float pulseSpeed = 2f; // Velocidade da pulsação do brilho
    [SerializeField] private float fadeOutDuration = 1f; // Duração do fade out após compra
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip purchaseSuccessClip; // Som de compra bem-sucedida
    [SerializeField] private AudioClip purchaseFailClip; // Som de moedas insuficientes
    
    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;
    
    private bool playerInRange = false;
    private bool isPurchased = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private ScoreManager scoreManager;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning($"[DoorPurchase] {gameObject.name} não tem SpriteRenderer! O brilho não funcionará.");
        }
        
        // Esconde a UI no início
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
    
    void Start()
    {
        // Busca o ScoreManager
        scoreManager = ScoreManager.Instance;
        
        if (scoreManager == null)
        {
            Debug.LogError("[DoorPurchase] ScoreManager não encontrado! O sistema de moedas não funcionará.");
        }
        
        // Busca AudioSource se não configurado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        if (isPurchased || !playerInRange) return;
        
        // Detecta input de interação (tecla E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPurchase();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPurchased) return;
        
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowInteractionUI();
            StartCoroutine(PulseHighlight());
            
            if (showDebugMessages)
            {
                Debug.Log($"[DoorPurchase] Player entrou na área de {doorName}");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionUI();
            
            // Não para corrotinas se já foi comprado (fade out deve continuar)
            if (!isPurchased)
            {
                StopAllCoroutines(); // Para a pulsação apenas se não foi comprado
                
                // Restaura cor original
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
            }
            
            if (showDebugMessages)
            {
                Debug.Log($"[DoorPurchase] Player saiu da área de {doorName}");
            }
        }
    }
    
    void ShowInteractionUI()
    {
        if (interactionUI == null || messageText == null) return;
        
        interactionUI.SetActive(true);
        
        // Monta a mensagem com o preço
        string message = $"[E] Abrir {doorName}\n<size=120%>{price} Moedas</size>";
        messageText.text = message;
    }
    
    void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
    
    void TryPurchase()
    {
        if (scoreManager == null)
        {
            Debug.LogError("[DoorPurchase] ScoreManager não encontrado!");
            return;
        }
        
        // Verifica se tem moedas suficientes
        if (scoreManager.TrySpendCoins(price))
        {
            // Compra bem-sucedida!
            if (showDebugMessages)
            {
                Debug.Log($"[DoorPurchase] {doorName} comprada! Gastou {price} moedas.");
            }
            
            OnPurchaseSuccess();
        }
        else
        {
            // Moedas insuficientes
            int currentCoins = scoreManager.CurrentCoins;
            int deficit = price - currentCoins;
            
            if (showDebugMessages)
            {
                Debug.Log($"[DoorPurchase] Moedas insuficientes! Precisa de mais {deficit} moedas.");
            }
            
            OnPurchaseFail(deficit);
        }
    }
    
    void OnPurchaseSuccess()
    {
        isPurchased = true;
        Debug.Log($"[DoorPurchase] OnPurchaseSuccess chamado para {doorName}");
        
        // Toca som de sucesso (usando PlayClipAtPoint para som continuar após destruição)
        if (purchaseSuccessClip != null)
        {
            float volume = audioSource != null ? audioSource.volume : 0.7f;
            AudioSource.PlayClipAtPoint(purchaseSuccessClip, transform.position, volume);
            Debug.Log($"[DoorPurchase] Som de sucesso tocado");
        }
        
        // Mostra mensagem de sucesso
        if (messageText != null)
        {
            messageText.text = $"{doorName}\nComprada!";
        }
        
        // Para a pulsação
        StopAllCoroutines();
        Debug.Log($"[DoorPurchase] Corrotinas paradas, iniciando FadeOutAndDestroy");
        
        // Inicia fade out e destruição
        StartCoroutine(FadeOutAndDestroy());
    }
    
    void OnPurchaseFail(int deficit)
    {
        // Toca som de falha (usando PlayClipAtPoint)
        if (purchaseFailClip != null)
        {
            float volume = audioSource != null ? audioSource.volume : 0.7f;
            AudioSource.PlayClipAtPoint(purchaseFailClip, transform.position, volume);
        }
        
        // Mostra mensagem de erro temporária
        if (messageText != null)
        {
            StartCoroutine(ShowErrorMessage(deficit));
        }
    }
    
    IEnumerator ShowErrorMessage(int deficit)
    {
        if (messageText == null) yield break;
        
        // Salva mensagem original
        string originalMessage = $"[E] Abrir {doorName}\n<size=120%>{price} Moedas</size>";
        
        // Mostra erro
        messageText.text = $"<color=red>Sem moedas!</color>\n<size=90%>Faltam: {deficit}</size>";
        
        // Aguarda 2 segundos
        yield return new WaitForSeconds(2f);
        
        // Restaura mensagem original (se ainda estiver no range)
        if (playerInRange && !isPurchased)
        {
            messageText.text = originalMessage;
        }
    }
    
    IEnumerator PulseHighlight()
    {
        if (spriteRenderer == null) yield break;
        
        while (playerInRange && !isPurchased)
        {
            // Cria efeito de pulsação entre cor original e cor de destaque
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, highlightColor, pulse * 0.5f);
            
            yield return null;
        }
        
        // Restaura cor original ao sair
        if (!isPurchased)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        Debug.Log($"[DoorPurchase] FadeOutAndDestroy INICIADO para {doorName}");
        
        if (spriteRenderer == null)
        {
            // Se não tem sprite renderer, apenas destrói
            Debug.Log($"[DoorPurchase] Sem SpriteRenderer, aguardando 1s antes de destruir");
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
            yield break;
        }
        
        // Esconde UI
        HideInteractionUI();
        Debug.Log($"[DoorPurchase] UI escondida, iniciando fade out");
        
        // Fade out gradual
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            yield return null;
        }
        
        // Garante alpha zero
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        if (showDebugMessages)
        {
            Debug.Log($"[DoorPurchase] {doorName} destruída após fade out.");
        }
        
        // Desabilita colliders para não interferir mais
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        // Destrói o GameObject imediatamente após fade out
        // O som continua tocando porque usamos AudioSource.PlayClipAtPoint
        Debug.Log($"[DoorPurchase] DESTRUINDO {gameObject.name}");
        Destroy(gameObject);
        Debug.Log($"[DoorPurchase] Destroy chamado para {gameObject.name}");
    }
    
    // Método público para testar compra sem custo (debug)
    public void DebugUnlock()
    {
        if (!isPurchased)
        {
            Debug.Log($"[DoorPurchase] DEBUG: Desbloqueando {doorName} gratuitamente.");
            OnPurchaseSuccess();
        }
    }
    
    // Getters para debug/inspector
    public bool IsPurchased => isPurchased;
    public int Price => price;
    public string DoorName => doorName;
}
