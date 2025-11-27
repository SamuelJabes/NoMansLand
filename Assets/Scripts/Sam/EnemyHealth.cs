using System.Collections;
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    int currentHealth;
    bool dead;

    [Header("Pooling")]
    [Tooltip("Pool ao qual este inimigo pertence (� setado automaticamente pelo ObjectPool).")]
    public ObjectPool pool;

    [Header("Score")]
    public int scoreValue = 1;
    public static event Action<EnemyHealth> OnAnyEnemyDied;

    [Header("Feedback de Dano (Escolha 1)")]
    public bool useMaterialSwap = false;
    public Material flashMaterial;

    [Header("Feedback via Tint (Escolha 2)")]
    public Color flashColor = new Color(1f, 0.95f, 0.8f);
    public float flashDuration = 0.08f;

    SpriteRenderer[] renderers;
    Color[] baseColors;
    Material[] baseMaterials;

    void Awake()
    {
        // CR�TICO: includeInactive=true para funcionar com pooling
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        baseColors = new Color[renderers.Length];
        baseMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
            baseMaterials[i] = renderers[i].sharedMaterial;
        }
    }

    void OnEnable()
    {
        // reset estado quando volta do pool
        dead = false;
        currentHealth = maxHealth;

        // reativa colisores
        var cols = GetComponentsInChildren<Collider2D>(includeInactive: true);
        foreach (var c in cols) c.enabled = true;

        // reativa sprites e for�a alpha 1
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

            // CR�TICO: recria arrays se necess�rio para evitar sprites invis�veis
            baseColors = new Color[renderers.Length];
            baseMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = renderers[i].color;
                baseMaterials[i] = renderers[i].sharedMaterial;
            }
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            r.enabled = true;

            // garante que n�o ficou transparente
            var col = baseColors != null && baseColors.Length > i ? baseColors[i] : Color.white;
            col.a = 1f;
            r.color = col;
        }

        // reseta visuais (materiais)
        RestoreVisuals();

        Debug.Log($"[EnemyHealth] {gameObject.name} reativado do pool com {renderers.Length} sprites");
    }

    // ======================
    // AQUI O DANO ACONTECE
    // ======================
    void OnTriggerEnter2D(Collider2D other)
    {
        int dmg = 0;

        // Bala normal
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            dmg = bullet.damage;
            Destroy(bullet.gameObject); // some com a bala
        }
        else
        {
            // Pellet de shotgun
            ShotgunPellet pellet = other.GetComponent<ShotgunPellet>();
            if (pellet != null)
            {
                dmg = pellet.damage;
                Destroy(pellet.gameObject); // some com o pellet
            }
        }

        if (dmg <= 0) return; // n�o era proj�til

        TakeDamage(dmg);
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        currentHealth -= amount;

        StopAllCoroutines();
        StartCoroutine(FlashOnce());

        if (currentHealth <= 0)
        {
            dead = true; // CRÍTICO: marca como morto IMEDIATAMENTE antes de chamar Die()
            Die();
        }
    }

    /// <summary>
    /// Retorna se o inimigo está morto (para auto-aim system)
    /// </summary>
    public bool IsDead()
    {
        return dead;
    }

    IEnumerator FlashOnce()
    {
        if (useMaterialSwap && flashMaterial != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] && renderers[i].sharedMaterial != flashMaterial)
                    renderers[i].sharedMaterial = flashMaterial;
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].sharedMaterial = baseMaterials[i];
            }
        }
        else
        {
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
        // desativa colisores pra n�o tomar mais tiro / bater em nada
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        // INCREMENTO DO SCORE
        OnAnyEnemyDied?.Invoke(this);

        // aqui voc� pode tocar anima��o de morte, som, etc.
        StartCoroutine(DespawnAfterDelay(0.1f));
    }

    IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Despawn();
    }

    // Agora � PUBLIC pra podermos chamar de fora se quiser
    public void Despawn()
    {
        if (pool != null)
        {
            pool.ReturnObjectToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// For�a o inimigo a voltar para o pool SEM dar score e SEM anima de morte.
    /// �til pra limpar zumbis de outras �reas quando come�a a boss fight.
    /// </summary>
    public void ForceDespawnWithoutScore()
    {
        // Bloqueia qualquer l�gica de morte/dano futura
        dead = true;

        // Cancela corrotinas de flash
        StopAllCoroutines();

        // Vai direto pro pool / destroy
        if (pool != null)
        {
            pool.ReturnObjectToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void RestoreVisuals()
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
