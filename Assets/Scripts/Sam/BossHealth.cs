using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // <- IMPORTANTE pra trocar de cena

public class BossHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 50;
    int currentHealth;
    bool dead;

    [Header("Feedback de Dano (Material Swap)")]
    public bool useMaterialSwap = false;
    public Material flashMaterial;

    [Header("Feedback via Tint (Cor)")]
    public Color flashColor = new Color(1f, 0.95f, 0.8f);
    public float flashDuration = 0.08f;

    [Header("Cena ao morrer")]
    [Tooltip("Nome da cena de final (precisa estar no Build Settings).")]
    public string endSceneName = "End";
    [Tooltip("Delay antes de carregar a cena (pra dar tempo de animação, som, etc).")]
    public float sceneChangeDelay = 2f;

    SpriteRenderer[] renderers;
    Color[] baseColors;
    Material[] baseMaterials;

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: false);

        baseColors = new Color[renderers.Length];
        baseMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
            baseMaterials[i] = renderers[i].sharedMaterial;
        }

        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        dead = false;
        currentHealth = maxHealth;

        // reativa colisores
        var cols = GetComponentsInChildren<Collider2D>(includeInactive: true);
        foreach (var c in cols) c.enabled = true;

        // reativa sprites / cores
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            r.enabled = true;
            var col = baseColors != null && baseColors.Length > i ? baseColors[i] : Color.white;
            col.a = 1f;
            r.color = col;
        }

        RestoreVisuals();
    }

    // ====== DANO ENTRANDO PELO COLIDER (BULLET / SHOTGUN) ======
    void OnTriggerEnter2D(Collider2D other)
    {
        int dmg = 0;

        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            dmg = bullet.damage;
            Destroy(bullet.gameObject);
        }
        else
        {
            ShotgunPellet pellet = other.GetComponent<ShotgunPellet>();
            if (pellet != null)
            {
                dmg = pellet.damage;
                Destroy(pellet.gameObject);
            }
        }

        if (dmg <= 0) return;

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
            dead = true;
            Die();
        }
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
        // desativa colisores
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        // aqui você pode tocar animação / som de morte
        // ex: GetComponent<Animator>()?.SetTrigger("Die");

        StartCoroutine(LoadEndSceneAfterDelay());
    }

    IEnumerator LoadEndSceneAfterDelay()
    {
        yield return new WaitForSeconds(sceneChangeDelay);

        if (!string.IsNullOrEmpty(endSceneName))
        {
            SceneManager.LoadScene(endSceneName);
        }
        else
        {
            Debug.LogError("[BossHealth] endSceneName não configurado!");
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
