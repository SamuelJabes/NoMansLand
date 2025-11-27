using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Botão de disparo para mobile - segure para atirar
/// </summary>
[RequireComponent(typeof(Image))]
public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Estado")]
    [Tooltip("Se true, o jogador está segurando o botão de tiro")]
    public bool IsPressed { get; private set; }
    
    [Tooltip("Se true, o botão foi pressionado neste frame (para tiro único)")]
    public bool WasPressedThisFrame { get; private set; }

    [Header("Feedback Visual")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color pressedColor = new Color(1f, 0.3f, 0.3f, 0.8f);
    
    private Image buttonImage;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
        IsPressed = false;
        WasPressedThisFrame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        WasPressedThisFrame = true;
        if (buttonImage != null)
            buttonImage.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }

    void LateUpdate()
    {
        // Reseta o flag no final do frame
        WasPressedThisFrame = false;
    }

    void OnDisable()
    {
        // Garante que se o botão for desativado, para de atirar
        IsPressed = false;
        WasPressedThisFrame = false;
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }
}
