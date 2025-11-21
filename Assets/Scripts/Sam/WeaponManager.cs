using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Armas disponíveis")]
    public GameObject pistolPrefab;  // Prefab da Pistol
    public GameObject shotgunPrefab; // Prefab da Shotgun
    public GameObject mgPrefab;      // Prefab da MG

    [Header("Spawn point da arma")]
    public Vector3 spawnOffset = Vector3.zero;   // Offset do spawn (deixe 0,0,0)

    [Header("Estado")]
    public WeaponType currentWeapon = WeaponType.Pistol; // Arma inicial

    private GameObject currentWeaponInstance;
    private GameObject existingWeapon; // Referência à Pistol já existente

    void Start()
    {
        // Procura por arma já existente na cena
        existingWeapon = GameObject.Find("Pistol");
        
        if (existingWeapon != null)
        {
            // Pistol já existe, só guarda referência
            currentWeaponInstance = existingWeapon;
            Debug.Log("Pistol já existe na cena, usando ela.");
        }
        else
        {
            // Cria a Pistol se não existir
            EquipWeapon(WeaponType.Pistol);
        }
    }

    public void EquipWeapon(WeaponType weaponType)
    {
        // Remove arma atual se existir
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }

        // Instancia a nova arma
        GameObject weaponPrefab = null;
        
        switch (weaponType)
        {
            case WeaponType.Pistol:
                weaponPrefab = pistolPrefab;
                break;
            case WeaponType.Shotgun:
                weaponPrefab = shotgunPrefab;
                break;
            case WeaponType.MG:
                weaponPrefab = mgPrefab;
                break;
        }

        if (weaponPrefab != null)
        {
            // Instancia na raiz da cena (WeaponOrbit2D cuida do posicionamento)
            currentWeaponInstance = Instantiate(weaponPrefab, transform.position + spawnOffset, Quaternion.identity);
            currentWeapon = weaponType;
            
            // Configura referência ao player no WeaponOrbit2D
            WeaponOrbit2D orbit = currentWeaponInstance.GetComponent<WeaponOrbit2D>();
            if (orbit != null)
            {
                orbit.player = transform; // Seta o player
            }
            
            Debug.Log($"Arma equipada: {weaponType}");
        }
        else
        {
            Debug.LogError("Prefab da arma não configurado!");
        }
    }

    public bool HasWeapon(WeaponType weaponType)
    {
        // Por enquanto, retorna true se já comprou
        // Você pode expandir isso com um sistema de inventário
        return currentWeapon == weaponType;
    }
}

public enum WeaponType
{
    Pistol,
    Shotgun,
    MG
}
