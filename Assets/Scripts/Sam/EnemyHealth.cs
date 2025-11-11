using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    int currentHealth;

    [Header("Feedback de Dano")]
    public float flashDuration = 0.08f;
    public Color flashColor = Color.white;

    SpriteRenderer sr;
    Color baseColor;
    bool dead;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        currentHealth -= amount;

        // pisca branco
        StopAllCoroutines();
        StartCoroutine(FlashOnce());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashOnce()
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = baseColor;
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        // desativa colisão/render e destrói
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
        sr.enabled = false;

        Destroy(gameObject); // ou Destroy(gameObject, 0.1f);
    }
}
