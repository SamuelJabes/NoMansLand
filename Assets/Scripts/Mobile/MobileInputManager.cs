using UnityEngine;

/// <summary>
/// Gerencia inputs mobile e PC automaticamente.
/// Singleton para acesso global.
/// </summary>
public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }

    [Header("Configuração")]
    [Tooltip("Forçar modo mobile mesmo no Editor (para testes)")]
    public bool forceMobileMode = false;

    [Header("Estado (Read-Only)")]
    [SerializeField] private bool isMobilePlatform;
    [SerializeField] private bool useMobileControls;

    public bool IsMobile => useMobileControls;
    public bool IsPC => !useMobileControls;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        DetectPlatform();
    }

    void DetectPlatform()
    {
        // Detecta se é plataforma mobile
        isMobilePlatform = Application.isMobilePlatform;

        // Usa controles mobile se:
        // - Estiver em plataforma mobile (Android/iOS)
        // - OU forçar modo mobile no Editor (para testes)
        useMobileControls = isMobilePlatform || forceMobileMode;

        Debug.Log($"[MobileInputManager] Platform: {Application.platform}");
        Debug.Log($"[MobileInputManager] IsMobile: {isMobilePlatform}");
        Debug.Log($"[MobileInputManager] UseMobileControls: {useMobileControls}");
        Debug.Log($"[MobileInputManager] ForceMobileMode: {forceMobileMode}");
    }

    /// <summary>
    /// Verifica se há input touch disponível
    /// </summary>
    public bool HasTouchInput()
    {
        return Input.touchCount > 0;
    }

    /// <summary>
    /// Verifica se há input de mouse disponível
    /// </summary>
    public bool HasMouseInput()
    {
        return Input.GetMouseButton(0) || Input.GetMouseButton(1);
    }
}
