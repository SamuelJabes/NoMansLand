using UnityEngine;
using UnityEngine.Events;

public class Health2D : MonoBehaviour
{
    public float maxHP = 100f;
    public UnityEvent onDeath;
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
        Destroy(gameObject);
    }
}
