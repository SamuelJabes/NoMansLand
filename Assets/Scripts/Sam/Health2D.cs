using UnityEngine;
using UnityEngine.Events;

public class Health2D : MonoBehaviour
{
    public float maxHP = 100f;
    public UnityEvent onDeath; // opcional p/ animação, drop, etc.

    float hp;

    void Awake() => hp = maxHP;

    public void ApplyDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f) Die();
    }

    void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject); // ou desabilite/animar antes de destruir
    }
}
