using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    int currentHealth;
    bool dead;

    [Header("Feedback de Dano (Escolha 1)")]
    [Tooltip("Se marcado, usa troca de material (mais robusto contra animações que keyframam cor).")]
    public bool useMaterialSwap = false;

    [Tooltip("Material de flash (ex.: um material que deixa a sprite bem clara). Se vazio, cai no tint de cor.")]
    public Material flashMaterial;

    [Header("Feedback via Tint (Escolha 2)")]
    [Tooltip("Cor aplicada temporariamente nos SpriteRenderers (se não usar material swap).")]
    public Color flashColor = new Color(1f, 0.95f, 0.8f); // 'branco quente' para aparecer melhor que branco puro
    public float flashDuration = 0.08f;

    // Cache de renderers e estados originais
    SpriteRenderer[] renderers;
    Color[] baseColors;
    Material[] baseMaterials;

    void Awake()
    {
        // Pega TODOS os SpriteRenderers (no próprio GO e nos filhos)
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: false);

        // Guarda cores e materiais originais
        baseColors = new Color[renderers.Length];
        baseMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
            baseMaterials[i] = renderers[i].sharedMaterial; // sharedMaterial para não instanciar cópias
        }

        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        currentHealth -= amount;

        // Para qualquer flash anterior e inicia um novo
        StopAllCoroutines();
        StartCoroutine(FlashOnce());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashOnce()
    {
        if (useMaterialSwap && flashMaterial != null)
        {
            // === Estratégia 1: Troca de material ===
            for (int i = 0; i < renderers.Length; i++)
            {
                // só troca se for diferente para evitar gerar instâncias desnecessárias
                if (renderers[i] && renderers[i].sharedMaterial != flashMaterial)
                    renderers[i].sharedMaterial = flashMaterial;
            }

            yield return new WaitForSeconds(flashDuration);

            // restaura materiais
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].sharedMaterial = baseMaterials[i];
            }
        }
        else
        {
            // === Estratégia 2: Tint de cor ===
            // Observação: se sua animação tiver keyframes de SpriteRenderer.color,
            // ela pode sobrescrever este tint. Nesse caso, prefira material swap.
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].color = baseColors[i];
            }
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        // desativa colisões
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        // desativa visuais
        foreach (var r in renderers) if (r) r.enabled = false;

        Destroy(gameObject);
    }

    // Se este objeto for desativado/destruído durante um flash, restaura estado
    void OnDisable()
    {
        RestoreVisuals();
    }
    void OnDestroy()
    {
        RestoreVisuals();
    }

    void RestoreVisuals()
    {
        if (renderers == null) return;

        // restaura cor e material originais
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i]) continue;
            renderers[i].color = baseColors[i];
            renderers[i].sharedMaterial = baseMaterials[i];
        }
    }
}
