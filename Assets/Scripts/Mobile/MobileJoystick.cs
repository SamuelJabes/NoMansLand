using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Virtual Joystick para controle de movimento mobile.
/// Detecta touch/drag e retorna direção normalizada.
/// </summary>
public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Componentes")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Configurações")]
    [Tooltip("Raio máximo que o handle pode se mover")]
    [SerializeField] private float handleRange = 50f;

    [Tooltip("Valor mínimo para considerar input (deadzone)")]
    [Range(0f, 1f)]
    [SerializeField] private float deadzone = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.5f;

    // Estado
    private Vector2 inputDirection = Vector2.zero;
    private bool isPressed = false;
    private CanvasGroup canvasGroup;

    // Propriedades públicas
    public Vector2 Direction => inputDirection;
    public float Horizontal => inputDirection.x;
    public float Vertical => inputDirection.y;
    public bool IsPressed => isPressed;

    void Awake()
    {
        // Setup inicial
        if (background == null) background = GetComponent<RectTransform>();
        if (handle == null) handle = transform.GetChild(0).GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Começa semi-transparente
        canvasGroup.alpha = inactiveAlpha;
    }

    void Update()
    {
        // Feedback visual (fade in/out)
        float targetAlpha = isPressed ? activeAlpha : inactiveAlpha;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Converte posição do touch para coordenadas locais do joystick
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        // Normaliza pela área do joystick
        position = position / handleRange;

        // Limita magnitude máxima a 1
        inputDirection = (position.magnitude > 1f) ? position.normalized : position;

        // Aplica deadzone
        if (inputDirection.magnitude < deadzone)
        {
            inputDirection = Vector2.zero;
        }

        // Atualiza posição visual do handle
        handle.anchoredPosition = inputDirection * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        inputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Retorna direção normalizada (magnitude 0-1)
    /// </summary>
    public Vector2 GetDirection()
    {
        return inputDirection;
    }

    /// <summary>
    /// Retorna magnitude do input (0-1)
    /// </summary>
    public float GetMagnitude()
    {
        return inputDirection.magnitude;
    }

    // Para debug no Inspector
    void OnValidate()
    {
        if (handleRange < 10f) handleRange = 10f;
    }

#if UNITY_EDITOR
    // Visual debug no Scene view
    void OnDrawGizmos()
    {
        if (Application.isPlaying && isPressed)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)inputDirection);
        }
    }
#endif
}
