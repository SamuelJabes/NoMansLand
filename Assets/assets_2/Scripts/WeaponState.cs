using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponState
{
    public WeaponConfig2D config;
    public int clip;
    public int reserve;

    public WeaponState(WeaponConfig2D cfg)
    {
        config = cfg;
        clip = cfg.clipSize;
        reserve = cfg.startingReserve;
    }
}

public class WeaponInventory2D : MonoBehaviour
{
    [SerializeField] private List<WeaponConfig2D> startingWeapons = new();
    public int CurrentIndex { get; private set; } = 0;
    public WeaponState Current => (weapons.Count > 0 ? weapons[CurrentIndex] : null);

    private List<WeaponState> weapons = new();

    void Awake()
    {
        weapons.Clear();
        foreach (var w in startingWeapons)
            if (w) weapons.Add(new WeaponState(w));
        CurrentIndex = Mathf.Clamp(CurrentIndex, 0, Mathf.Max(0, weapons.Count - 1));
    }

    void Update()
    {
        // Troca rápida: números 1..9
        for (int i = 0; i < Mathf.Min(9, weapons.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectIndex(i);
        }
        // Scroll opcional
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0) SelectIndex((CurrentIndex - (int)Mathf.Sign(scroll) + weapons.Count) % weapons.Count);
    }

    public void SelectIndex(int idx)
    {
        if (weapons.Count == 0) return;
        idx = Mathf.Clamp(idx, 0, weapons.Count - 1);
        if (idx == CurrentIndex) return;
        CurrentIndex = idx;
        // Aqui você pode disparar evento de troca para atualizar UI/anim
        // Debug.Log($"Arma atual: {Current.config.weaponName}");
    }
}
