using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Fade Out na Morte")]
    [Tooltip("Tempo que o boss leva para sumir (fade).")]
    public float fadeDuration = 1.2f;
    [Tooltip("Tempo extra depois do fade antes de carregar a cena.")]
    public float afterFadeDelay = 0.5f;

    [Header("Áudio de Morte")]
    [Tooltip("Som do grito/morte do boss.")]
    public AudioClip deathSFX;

    // Pode ir até 2 aqui, mas lembre que o AudioClip em si também tem volume.
    [Range(0f, 2f)]
    public float deathSFXVolume = 1.0f;

    [Tooltip("Se marcado, toca o som em 2D na câmera (sem perder volume por distância).")]
    public bool deathSFXAs2D = true;

    [Header("Camera Shake na Morte")]
    [Tooltip("Câmera que vai tremer. Se vazio, usa Camera.main.")]
    public Camera cameraToShake;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.25f;

    SpriteRenderer[] renderers;
    Color[] baseColors;
    Material[] baseMaterials;

    void Awake()
    {
        // cache dos renderers
        renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: false);

        baseColors = new Color[renderers.Length];
        baseMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].color;
            baseMaterials[i] = renderers[i].sharedMaterial;
        }

        currentHealth = maxHealth;

        // tenta pegar camera principal se não tiver setado
        if (cameraToShake == null)
        {
            cameraToShake = Camera.main;
        }
    }

    void OnEnable()
    {
        dead = false;
        currentHealth = maxHealth;

        // reativa colisores
        var cols = GetComponentsInChildren<Collider2D>(includeInactive: true);
        foreach (var c in cols) c.enabled = true;

        // reativa sprites e garante alpha 1
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

    // ======================
    // DANO ENTRANDO PELO COLIDER (BULLET / SHOTGUN)
    // ======================
    void OnTriggerEnter2D(Collider2D other)
    {
        int dmg = 0;

        // Bala normal
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            dmg = bullet.damage;
            Destroy(bullet.gameObject);
        }
        else
        {
            // Pellet de shotgun
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
            // troca material
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] && renderers[i].sharedMaterial != flashMaterial)
                    renderers[i].sharedMaterial = flashMaterial;
            }

            yield return new WaitForSeconds(flashDuration);

            // volta pro material original
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                    renderers[i].sharedMaterial = baseMaterials[i];
            }
        }
        else
        {
            // só pinta de outra cor
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
        // desliga lógica de movimento/ataque
        var bossController = GetComponent<BossController>();
        if (bossController != null)
            bossController.enabled = false;

        // desativa colisores pra não bater mais em nada
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols) c.enabled = false;

        // toca som de morte
        PlayDeathSFX();

        // inicia fade + camera shake + troca de cena
        StartCoroutine(FadeOutAndChangeScene());
    }

    void PlayDeathSFX()
    {
        if (deathSFX == null) return;

        // 2D na câmera (sem atenuação por distância)
        if (deathSFXAs2D && Camera.main != null)
        {
            GameObject go = new GameObject("BossDeathSFX");
            go.transform.position = Camera.main.transform.position;

            var src = go.AddComponent<AudioSource>();
            src.clip = deathSFX;
            src.volume = Mathf.Clamp01(deathSFXVolume);
            src.spatialBlend = 0f; // 0 = som 2D
            src.playOnAwake = false;

            src.Play();
            Destroy(go, deathSFX.length + 0.1f);
        }
        else
        {
            // fallback 3D no mundo (perde volume com distância)
            AudioSource.PlayClipAtPoint(
                deathSFX,
                transform.position,
                Mathf.Clamp01(deathSFXVolume)
            );
        }
    }

    IEnumerator FadeOutAndChangeScene()
    {
        // começa o shake da câmera em paralelo
        StartCoroutine(ShakeCameraCoroutine());

        float t = 0f;

        // guarda cores iniciais
        Color[] startColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i])
                startColors[i] = renderers[i].color;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fadeDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                var r = renderers[i];
                if (!r) continue;

                Color c = startColors[i];
                c.a = Mathf.Lerp(startColors[i].a, 0f, lerp);
                r.color = c;
            }

            yield return null;
        }

        // garante alpha 0
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;

            Color c = r.color;
            c.a = 0f;
            r.color = c;
        }

        yield return new WaitForSeconds(afterFadeDelay);

        if (!string.IsNullOrEmpty(endSceneName))
        {
            SceneManager.LoadScene(endSceneName);
        }
        else
        {
            Debug.LogError("[BossHealth] endSceneName não configurado!");
        }
    }

    IEnumerator ShakeCameraCoroutine()
    {
        if (cameraToShake == null) yield break;

        Transform camT = cameraToShake.transform;
        Vector3 originalPos = camT.position;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = shakeMagnitude * (1f - (elapsed / shakeDuration));

            Vector2 offset = Random.insideUnitCircle * strength;
            camT.position = originalPos + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        camT.position = originalPos;
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
