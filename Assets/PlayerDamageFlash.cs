using System.Collections;
using UnityEngine;

public class PlayerDamageFlash : MonoBehaviour
{
    [Header("Feedback de Dano (Escolha 1)")]
    [Tooltip("Usar troca de material para feedback?")]
    public bool useMaterialSwap = false;

    [Tooltip("Material que 'pisca' quando toma dano")]
    public Material flashMaterial;

    [Header("Feedback via Tint (Escolha 2)")]
    [Tooltip("Cor que o sprite fica quando toma dano")]
    public Color flashColor = new Color(1f, 0.3f, 0.3f); // Vermelho claro

    [Tooltip("Duração do flash em segundos")]
    public float flashDuration = 0.15f;

    private SpriteRenderer[] renderers;
    private Color[] baseColors;
    private Material[] baseMaterials;

    void Awake()
    {
        // Pega todos os SpriteRenderers (incluindo filhos, como arma)
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        // Guarda cores e materiais originais
        baseColors = new Color[renderers.Length];
        baseMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
            baseMaterials[i] = renderers[i].sharedMaterial;
        }
    }

    /// <summary>
    /// Chame este método quando o player tomar dano
    /// </summary>
    public void Flash()
    {
        StopAllCoroutines(); // Para qualquer flash anterior
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Aplica o efeito
        if (useMaterialSwap && flashMaterial != null)
        {
            // Método 1: Troca de material
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] && renderers[i].sharedMaterial != flashMaterial)
                    renderers[i].sharedMaterial = flashMaterial;
            }

            yield return new WaitForSeconds(flashDuration);

            // Restaura material original
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].sharedMaterial = baseMaterials[i];
            }
        }
        else
        {
            // Método 2: Muda a cor (tint)
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            // Restaura cor original
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].color = baseColors[i];
            }
        }
    }

    /// <summary>
    /// Restaura visuais originais (útil se precisar forçar reset)
    /// </summary>
    public void RestoreVisuals()
    {
        if (renderers == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i]) continue;
            renderers[i].color = baseColors[i];
            renderers[i].sharedMaterial = baseMaterials[i];
        }
    }
}