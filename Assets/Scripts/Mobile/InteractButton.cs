using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Botão de interação (E) para mobile - comprar armas, abrir portas
/// </summary>
[RequireComponent(typeof(Image))]
public class InteractButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Estado")]
    [Tooltip("Se true, o botão de interação foi pressionado neste frame")]
    public bool WasPressedThisFrame { get; private set; }

    [Header("Feedback Visual")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color pressedColor = new Color(0.3f, 1f, 0.3f, 0.8f);
    
    [Header("Visibilidade")]
    [Tooltip("Se true, o botão só aparece quando há algo para interagir")]
    [SerializeField] private bool hideWhenNotNeeded = true;
    
    private Image buttonImage;
    private CanvasGroup canvasGroup;
    private bool isVisible = false;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        buttonImage.color = normalColor;
        WasPressedThisFrame = false;

        if (hideWhenNotNeeded)
            Hide();
    }

    void LateUpdate()
    {
        // Reseta o estado a cada frame
        WasPressedThisFrame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WasPressedThisFrame = true;
        if (buttonImage != null)
            buttonImage.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }

    /// <summary>
    /// Mostra o botão (chamado por WeaponStation/DoorPurchase quando player entra no trigger)
    /// </summary>
    public void Show()
    {
        Debug.Log("[InteractButton] Show() chamado! hideWhenNotNeeded=" + hideWhenNotNeeded);
        
        if (!hideWhenNotNeeded) return;
        
        isVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        Debug.Log("[InteractButton] Botão agora visível! Alpha=" + canvasGroup.alpha);
    }

    /// <summary>
    /// Esconde o botão (chamado quando player sai do trigger)
    /// </summary>
    public void Hide()
    {
        if (!hideWhenNotNeeded) return;
        
        isVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnDisable()
    {
        WasPressedThisFrame = false;
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }
}
