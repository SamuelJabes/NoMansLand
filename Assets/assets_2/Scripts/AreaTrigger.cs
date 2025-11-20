using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaTrigger : MonoBehaviour
{
    [Header("Spawner de inimigos responsável")]
    public EnemySpawner2Types spawner;

    [Header("Configuração da Área")]
    [Tooltip("Índice da área: 1, 2 ou 3")]
    public int areaIndex = 1;

    [Tooltip("Se marcado, desbloqueia essa área ao passar aqui.")]
    public bool unlockAreaOnEnter = false;

    [Tooltip("Se marcado, define esta área como 'área atual' do jogador ao entrar.")]
    public bool setCurrentAreaOnEnter = true;

    [Header("Uma vez só?")]
    [Tooltip("Se true, esse trigger só funciona uma vez e depois se desabilita.")]
    public bool triggerOnlyOnce = true;

    private bool alreadyTriggered = false;

    void Reset()
    {
        // Facilita: garante que o collider seja Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyTriggered && triggerOnlyOnce) return;

        if (!other.CompareTag("Player")) return;
        if (spawner == null)
        {
            Debug.LogWarning($"[AreaTrigger] Spawner não atribuído em {name}.");
            return;
        }

        if (unlockAreaOnEnter)
        {
            spawner.UnlockArea(areaIndex);
        }

        if (setCurrentAreaOnEnter)
        {
            spawner.SetCurrentArea(areaIndex);
        }

        if (triggerOnlyOnce)
        {
            alreadyTriggered = true;
        }
    }
}
