using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/WeaponConfig2D", fileName = "NewWeaponConfig2D")]
public class WeaponConfig2D : ScriptableObject
{
    [Header("Identidade")]
    public string weaponName = "Pistol";
    public bool automatic = false;          // segurar para metralhar (rifle), clique único (pistola/shotgun)

    [Header("Balística")]
    public float damage = 20f;              // dano por pellet
    public int pelletsPerShot = 1;          // shotgun: >1 (ex: 8)
    public float spreadDegrees = 1.5f;      // abertura dos tiros (em graus)
    public float range = 25f;               // alcance do raycast
    public float fireRate = 6f;             // tiros por segundo

    [Header("Munição")]
    public int ammoPerShot = 1;             // shotgun consome 1 por disparo, não por pellet
    public int clipSize = 12;               // tamanho do pente
    public int startingReserve = 72;        // munição reserva inicial
    public float reloadTime = 1.3f;         // tempo para recarregar (pente inteiro)

    [Header("Visual")]
    public float tracerTime = 0.03f;        // duração do rastro (se usar LineRenderer)
}
