using UnityEngine;
using TMPro;

public class WeaponStation : MonoBehaviour
{
    [Header("Configuração da Arma")]
    public WeaponType weaponType = WeaponType.MG;
    public int price = 1000;

    [Header("UI")]
    public TextMeshProUGUI messageText;
    public GameObject interactionUI;  // Painel com texto e fundo

    [Header("Visual Feedback")]
    public SpriteRenderer stationSprite;
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.gray;
    public Color highlightColor = Color.yellow;

    [Header("Comportamento")]
    public bool destroyAfterPurchase = true;  // Se true, some após comprar
    public bool canBuyMultipleTimes = false;  // Se true, pode comprar várias vezes

    [Header("Audio (Opcional)")]
    public AudioClip purchaseSuccessClip;  // Som de compra bem-sucedida
    public AudioClip purchaseFailClip;     // Som de moedas insuficientes

    private bool playerInRange = false;
    private bool alreadyPurchased = false;
    private WeaponManager playerWeaponManager;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = ScoreManager.Instance;
        
        if (interactionUI != null)
            interactionUI.SetActive(false);

        UpdateStationVisual();
    }

    void Update()
    {
        if (!playerInRange) return;

        // Detecta input E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPurchase();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerWeaponManager = other.GetComponent<WeaponManager>();

            if (playerWeaponManager == null)
            {
                Debug.LogError("Player não tem WeaponManager!");
                return;
            }

            ShowInteractionUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionUI();
        }
    }

    void ShowInteractionUI()
    {
        if (interactionUI == null || messageText == null) return;

        // Verifica se já comprou
        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            messageText.text = "Já adquirido!";
            interactionUI.SetActive(true);
            return;
        }

        // Verifica se já tem essa arma equipada
        if (playerWeaponManager.currentWeapon == weaponType)
        {
            messageText.text = $"{weaponType} já equipada!";
            interactionUI.SetActive(true);
            return;
        }

        // Mostra mensagem de compra
        string weaponName = weaponType == WeaponType.MG ? "MG" : 
                            weaponType == WeaponType.Shotgun ? "Shotgun" : "Pistola";
        messageText.text = $"[E] Comprar {weaponName}\n<size=120%>{price} Moedas</size>";
        interactionUI.SetActive(true);

        // Highlight visual
        if (stationSprite != null)
            stationSprite.color = highlightColor;
    }

    void HideInteractionUI()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);

        UpdateStationVisual();
    }

    void TryPurchase()
    {
        // Verifica se já comprou
        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            Debug.Log("Arma já foi comprada!");
            return;
        }

        // Verifica se tem moedas suficientes
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager não encontrado!");
            return;
        }

        if (scoreManager.CurrentCoins < price)
        {
            // Toca som de erro
            if (purchaseFailClip != null)
            {
                AudioSource.PlayClipAtPoint(purchaseFailClip, transform.position, 0.7f);
            }

            // Moedas insuficientes
            if (messageText != null)
            {
                messageText.text = $"<color=red>Sem moedas!</color>\n<size=90%>Faltam: {price - scoreManager.CurrentCoins}</size>";
            }
            Debug.Log($"Moedas insuficientes! Tem: {scoreManager.CurrentCoins}, Precisa: {price}");
            return;
        }

        // Compra com sucesso!
        if (scoreManager.TrySpendCoins(price))
        {
            // Toca som de sucesso
            if (purchaseSuccessClip != null)
            {
                AudioSource.PlayClipAtPoint(purchaseSuccessClip, transform.position, 0.7f);
            }

            // Equipa a arma
            playerWeaponManager.EquipWeapon(weaponType);
            
            alreadyPurchased = true;

            // Feedback visual
            if (messageText != null)
            {
                string weaponName = weaponType == WeaponType.MG ? "AK-47" : 
                                    weaponType == WeaponType.Shotgun ? "Shotgun" : "Pistola";
                messageText.text = $"<color=green>{weaponName}\nAdquirida!</color>";
            }

            Debug.Log($"{weaponType} comprada por {price} moedas!");

            // Some após compra se configurado
            if (destroyAfterPurchase)
            {
                Invoke(nameof(DestroyStation), 1f); // Espera 1 segundo antes de sumir
            }
            else
            {
                HideInteractionUI();
            }
        }
    }

    void DestroyStation()
    {
        Destroy(gameObject);
    }

    void UpdateStationVisual()
    {
        if (stationSprite == null) return;

        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            stationSprite.color = unavailableColor;
        }
        else
        {
            stationSprite.color = availableColor;
        }
    }
}
