# 🔫 GUIA COMPLETO: Sistema de Compra e Troca de Armas

## 📋 ÍNDICE
1. [Visão Geral do Sistema](#1-visão-geral-do-sistema)
2. [Criar Scripts Necessários](#2-criar-scripts-necessários)
3. [Configurar Prefabs das Armas](#3-configurar-prefabs-das-armas)
4. [Criar Weapon Station no Mapa](#4-criar-weapon-station-no-mapa)
5. [Configurar UI de Interação](#5-configurar-ui-de-interação)
6. [Configurar Player](#6-configurar-player)
7. [Testar e Ajustar](#7-testar-e-ajustar)

---

## 1. VISÃO GERAL DO SISTEMA

### Como vai funcionar:
1. **Player começa com Pistol** equipada
2. **MG Station** colocada em algum lugar do mapa
3. **Player se aproxima** da MG Station
4. **UI aparece**: "Pressione E para comprar MG - 1000 moedas"
5. **Player pressiona E**:
   - Se tiver moedas suficientes → Compra a MG e troca automaticamente
   - Se não tiver → Mensagem "Moedas insuficientes"
6. **MG desaparece do mapa** após compra (ou fica disponível para sempre)

---

## 2. CRIAR SCRIPTS NECESSÁRIOS

### 2.1 WeaponManager.cs
**Gerencia qual arma está equipada e a troca**

Crie o arquivo em `Assets/Scripts/Sam/WeaponManager.cs`:

```csharp
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Armas disponíveis")]
    public GameObject pistolPrefab;  // Prefab da Pistol
    public GameObject mgPrefab;      // Prefab da MG

    [Header("Spawn point da arma")]
    public Transform weaponHolder;   // Onde a arma vai aparecer (filho do player)

    [Header("Estado")]
    public WeaponType currentWeapon = WeaponType.Pistol; // Arma inicial

    private GameObject currentWeaponInstance;

    void Start()
    {
        // Começa com a Pistol equipada
        EquipWeapon(WeaponType.Pistol);
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
            case WeaponType.MG:
                weaponPrefab = mgPrefab;
                break;
        }

        if (weaponPrefab != null && weaponHolder != null)
        {
            currentWeaponInstance = Instantiate(weaponPrefab, weaponHolder.position, weaponHolder.rotation, weaponHolder);
            currentWeapon = weaponType;
            Debug.Log($"Arma equipada: {weaponType}");
        }
        else
        {
            Debug.LogError("Prefab da arma ou WeaponHolder não configurado!");
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
    MG
}
```

---

### 2.2 WeaponStation.cs
**Script para a estação de compra de arma no mapa**

Crie o arquivo em `Assets/Scripts/Sam/WeaponStation.cs`:

```csharp
using UnityEngine;
using TMPro;

public class WeaponStation : MonoBehaviour
{
    [Header("Configuração da Arma")]
    public WeaponType weaponType = WeaponType.MG;
    public int price = 1000;

    [Header("UI")]
    public TextMeshProUGUI messageText;
    public GameObject interactionUI;  // Painel com texto e fundo

    [Header("Visual Feedback")]
    public SpriteRenderer stationSprite;
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.gray;
    public Color highlightColor = Color.yellow;

    [Header("Comportamento")]
    public bool destroyAfterPurchase = true;  // Se true, some após comprar
    public bool canBuyMultipleTimes = false;  // Se true, pode comprar várias vezes

    private bool playerInRange = false;
    private bool alreadyPurchased = false;
    private WeaponManager playerWeaponManager;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = ScoreManager.Instance;
        
        if (interactionUI != null)
            interactionUI.SetActive(false);

        UpdateStationVisual();
    }

    void Update()
    {
        if (!playerInRange) return;

        // Detecta input E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPurchase();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerWeaponManager = other.GetComponent<WeaponManager>();

            if (playerWeaponManager == null)
            {
                Debug.LogError("Player não tem WeaponManager!");
                return;
            }

            ShowInteractionUI();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionUI();
        }
    }

    void ShowInteractionUI()
    {
        if (interactionUI == null || messageText == null) return;

        // Verifica se já comprou
        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            messageText.text = "Já adquirido!";
            interactionUI.SetActive(true);
            return;
        }

        // Verifica se já tem essa arma equipada
        if (playerWeaponManager.currentWeapon == weaponType)
        {
            messageText.text = $"{weaponType} já equipada!";
            interactionUI.SetActive(true);
            return;
        }

        // Mostra mensagem de compra
        string weaponName = weaponType == WeaponType.MG ? "Metralhadora" : "Pistola";
        messageText.text = $"[E] Comprar {weaponName}\n<size=80%>{price} moedas</size>";
        interactionUI.SetActive(true);

        // Highlight visual
        if (stationSprite != null)
            stationSprite.color = highlightColor;
    }

    void HideInteractionUI()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);

        UpdateStationVisual();
    }

    void TryPurchase()
    {
        // Verifica se já comprou
        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            Debug.Log("Arma já foi comprada!");
            return;
        }

        // Verifica se tem moedas suficientes
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager não encontrado!");
            return;
        }

        if (scoreManager.CurrentCoins < price)
        {
            // Moedas insuficientes
            if (messageText != null)
            {
                messageText.text = $"<color=red>Moedas insuficientes!</color>\n<size=80%>Você tem: {scoreManager.CurrentCoins}\nPrecisa: {price}</size>";
            }
            Debug.Log($"Moedas insuficientes! Tem: {scoreManager.CurrentCoins}, Precisa: {price}");
            return;
        }

        // Compra com sucesso!
        if (scoreManager.TrySpendCoins(price))
        {
            // Equipa a arma
            playerWeaponManager.EquipWeapon(weaponType);
            
            alreadyPurchased = true;

            // Feedback visual
            if (messageText != null)
            {
                string weaponName = weaponType == WeaponType.MG ? "Metralhadora" : "Pistola";
                messageText.text = $"<color=green>{weaponName} adquirida!</color>";
            }

            Debug.Log($"{weaponType} comprada por {price} moedas!");

            // Some após compra se configurado
            if (destroyAfterPurchase)
            {
                Invoke(nameof(DestroyStation), 1f); // Espera 1 segundo antes de sumir
            }
            else
            {
                HideInteractionUI();
            }
        }
    }

    void DestroyStation()
    {
        Destroy(gameObject);
    }

    void UpdateStationVisual()
    {
        if (stationSprite == null) return;

        if (alreadyPurchased && !canBuyMultipleTimes)
        {
            stationSprite.color = unavailableColor;
        }
        else
        {
            stationSprite.color = availableColor;
        }
    }
}
```

---

## 3. CONFIGURAR PREFABS DAS ARMAS

### 3.1 Verificar Pistol.prefab

1. Abra `Assets/Prefabs/Pistol.prefab`
2. Certifique-se de que tem:
   - ✅ `Shoot` component (com bullet configurado)
   - ✅ `WeaponOrbit2D` component
   - ✅ `SpriteRenderer` com sprite da pistola
   - ✅ `AudioSource` (opcional, para som)

### 3.2 Verificar MG.prefab

1. Abra `Assets/Prefabs/MG.prefab`
2. Certifique-se de que tem os mesmos componentes:
   - ✅ `Shoot` component (com bullet e fireRate diferente)
   - ✅ `WeaponOrbit2D` component
   - ✅ `SpriteRenderer` com sprite da MG
   - ✅ `AudioSource` (opcional)

**Diferenças sugeridas entre Pistol e MG:**
```
Pistol:
  fireRate: 0.4 (2.5 tiros/segundo)
  damage: 1

MG:
  fireRate: 0.1 (10 tiros/segundo) - muito mais rápida!
  damage: 1
```

---

## 4. CRIAR WEAPON STATION NO MAPA

### 4.1 Criar GameObject da MG Station

1. **Na cena sam_mapa_1**, Hierarchy → Botão direito → **Create Empty**
2. Nomeie: `MG_Station`
3. **Posicione** em algum lugar do mapa (ex: X: 10, Y: 5, Z: 0)

### 4.2 Adicionar Visual

**Opção A - Usar sprite da MG:**
1. Em `MG_Station`, **Add Component** → `Sprite Renderer`
2. Configure:
   - **Sprite**: Use o mesmo sprite da MG ou crie um ícone específico
   - **Color**: Amarelo ou dourado (para indicar que é uma estação)
   - **Sorting Layer**: UI ou o mesmo layer dos objetos do mapa
   - **Order in Layer**: Alto o suficiente para aparecer

**Opção B - Criar sprite customizado:**
1. Crie um sprite de "arma no chão" ou "caixa de arma"
2. Arraste para a cena
3. Use como visual da station

### 4.3 Adicionar Collider

1. Em `MG_Station`, **Add Component** → `Box Collider 2D`
2. Configure:
   - **Is Trigger**: ✓ (MARQUE!)
   - **Size**: Ajuste para ser um pouco maior que o sprite (ex: 2x2)

### 4.4 Adicionar Script

1. Em `MG_Station`, **Add Component** → `Weapon Station`
2. Configure:
   - **Weapon Type**: `MG`
   - **Price**: `1000` (ou o valor que quiser)
   - **Destroy After Purchase**: ✓ (marca se quiser que suma)
   - **Station Sprite**: Arraste o Sprite Renderer do próprio GameObject
   - **Available Color**: Branco
   - **Unavailable Color**: Cinza
   - **Highlight Color**: Amarelo

---

## 5. CONFIGURAR UI DE INTERAÇÃO

### 5.1 Criar UI Panel para Mensagem

1. **No Canvas HUD_Canvas**, Botão direito → **UI → Panel**
2. Nomeie: `WeaponInteractionUI`
3. Configure RectTransform:
   ```
   Anchor: Bottom-Center
   Anchor Min: (0.5, 0)
   Anchor Max: (0.5, 0)
   Pivot: (0.5, 0)
   Pos X: 0
   Pos Y: 100
   Width: 400
   Height: 100
   ```

### 5.2 Configurar Panel

1. No componente **Image** do Panel:
   - **Color**: Preto semi-transparente (R:0, G:0, B:0, A:0.7)
   - **Source Image**: None (ou use um sprite de fundo arredondado)

### 5.3 Adicionar Texto

1. Botão direito em `WeaponInteractionUI` → **UI → Text - TextMeshPro**
2. Nomeie: `InteractionText`
3. Configure:
   - **Text**: "[E] Comprar Metralhadora\n1000 moedas"
   - **Font Size**: 24
   - **Alignment**: Center (horizontal e vertical)
   - **Color**: Branco
   - **RectTransform**: Stretch (preenche o pai todo)

### 5.4 Desativar por padrão

1. Selecione `WeaponInteractionUI`
2. **Desmarque o checkbox** ao lado do nome no Inspector (desativa o GameObject)

### 5.5 Conectar UI à Station

1. Volte para o `MG_Station` no mapa
2. No componente `Weapon Station`:
   - **Message Text**: Arraste `InteractionText` do Canvas
   - **Interaction UI**: Arraste `WeaponInteractionUI` do Canvas

---

## 6. CONFIGURAR PLAYER

### 6.1 Criar WeaponHolder

1. Selecione o GameObject **Player** na Hierarchy
2. Botão direito → **Create Empty**
3. Nomeie: `WeaponHolder`
4. Configure Transform:
   ```
   Position: (0, 0, 0) - relativo ao player
   Rotation: (0, 0, 0)
   Scale: (1, 1, 1)
   ```

Este será o ponto onde as armas vão aparecer!

### 6.2 Adicionar WeaponManager ao Player

1. Selecione o GameObject **Player**
2. **Add Component** → `Weapon Manager`
3. Configure:
   - **Pistol Prefab**: Arraste `Pistol.prefab` do Project
   - **MG Prefab**: Arraste `MG.prefab` do Project
   - **Weapon Holder**: Arraste o GameObject `WeaponHolder` (filho do Player)
   - **Current Weapon**: `Pistol`

### 6.3 Remover arma manual (se houver)

Se você já tem uma arma como filho direto do Player (não instanciada por script):
1. **Delete** esse GameObject de arma
2. O WeaponManager vai criar automaticamente no Start

---

## 7. TESTAR E AJUSTAR

### 7.1 Checklist antes de testar

- [ ] WeaponManager.cs criado em `Assets/Scripts/Sam/`
- [ ] WeaponStation.cs criado em `Assets/Scripts/Sam/`
- [ ] MG_Station criado no mapa
- [ ] MG_Station tem BoxCollider2D (IsTrigger = true)
- [ ] MG_Station tem WeaponStation script configurado
- [ ] WeaponInteractionUI criado no Canvas
- [ ] WeaponInteractionUI desativado por padrão
- [ ] Player tem WeaponManager
- [ ] Player tem WeaponHolder vazio
- [ ] Prefabs Pistol e MG estão OK
- [ ] Player tem tag "Player"

### 7.2 Teste 1: Pistol inicial

1. **Aperte Play**
2. **Observe**: Player deve começar com a Pistol
3. **Teste atirar**: Botão esquerdo do mouse deve disparar
4. ✅ Se funcionar, continue

❌ **Se não funcionar:**
- Verifique Console para erros
- Certifique-se de que Pistol.prefab tem Shoot script
- Verifique se WeaponHolder está configurado

### 7.3 Teste 2: Interação com MG Station

1. **Movimente o player** até a MG_Station
2. **Observe**: 
   - UI deve aparecer: "[E] Comprar Metralhadora - 1000 moedas"
   - Station deve ficar amarela (highlight)
3. **Afaste-se**: UI deve desaparecer
4. ✅ Se funcionar, continue

❌ **Se não funcionar:**
- BoxCollider2D está como Trigger?
- Player tem tag "Player"?
- UI está conectada no WeaponStation?

### 7.4 Teste 3: Compra sem dinheiro

1. **Aproxime-se da station**
2. **Pressione E** (provavelmente você não tem 1000 moedas ainda)
3. **Observe**: Deve mostrar "Moedas insuficientes!"
4. ✅ Funcionou? Continue

### 7.5 Teste 4: Compra com dinheiro

**Para testar rapidamente, mude o preço:**
1. Pare o Play Mode
2. Selecione `MG_Station`
3. Mude `Price` para `10`
4. Aperte Play novamente

**Ou ganhe moedas matando zumbis!**

5. **Quando tiver moedas suficientes**:
   - Aproxime-se da station
   - Pressione E
   - ✅ Pistol deve sumir e MG aparecer!
   - ✅ Station deve sumir (se destroyAfterPurchase = true)
   - ✅ Moedas devem ser deduzidas

### 7.6 Teste 5: Atirar com MG

1. **Após comprar a MG**, teste atirar
2. **Observe**: Fire rate deve ser muito mais rápido
3. ✅ Se atirar rapidamente, sucesso!

---

## 8. AJUSTES E BALANCEAMENTO

### 8.1 Preços sugeridos

```
Pistol: Gratuita (inicial)
MG: 1000 moedas (primeira arma comprável)
Shotgun (futura): 2000 moedas
```

### 8.2 Valores de moedas

Lembre-se que atualmente:
- **1 kill = 1 score = 10 moedas** (configurado no ScoreManager)

Para comprar MG por 1000 moedas:
- Precisa matar **100 zumbis**
- Ou ajuste `coinsPerScore` no ScoreManager para dar mais moedas

### 8.3 Diferenças das armas

**Balanceamento sugerido:**

| Arma | Fire Rate | Dano | Velocidade Bala | Alcance |
|------|-----------|------|----------------|---------|
| Pistol | 0.4s (2.5/s) | 1 | 8 | Alto |
| MG | 0.1s (10/s) | 1 | 10 | Médio |

**Para ajustar:**
1. Abra cada prefab (Pistol, MG)
2. No componente `Shoot`:
   - Mude `fireRate`
3. No prefab da Bullet:
   - Mude `speed`
   - Mude `damage`

---

## 9. EXPANSÕES FUTURAS

### 9.1 Múltiplas Weapon Stations

Crie várias stations no mapa:
1. **Duplique** `MG_Station` (Ctrl+D)
2. Renomeie: `Shotgun_Station`
3. Mude posição no mapa
4. Configure WeaponStation:
   - Weapon Type: (adicione Shotgun no enum)
   - Price: 2000

### 9.2 Sistema de Inventário

Expanda o WeaponManager para ter:
```csharp
public List<WeaponType> unlockedWeapons;
```

E adicione teclas para trocar:
```csharp
if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(WeaponType.Pistol);
if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(WeaponType.MG);
```

### 9.3 UI de Arma Equipada

Crie um ícone na HUD mostrando:
- Arma atual
- Munição (se implementar)
- Armas desbloqueadas

### 9.4 Munição Limitada

Adicione ao Shoot.cs:
```csharp
public int maxAmmo = 30;
public int currentAmmo = 30;
public int reserveAmmo = 90;
```

---

## 10. TROUBLESHOOTING

### Problema: Arma não aparece ao iniciar

**Soluções:**
1. WeaponHolder está configurado no WeaponManager?
2. Pistol.prefab está atribuído?
3. Verifique o Console para erros

### Problema: UI não aparece ao aproximar

**Soluções:**
1. BoxCollider2D está como `Is Trigger = true`?
2. Player tem a tag "Player"?
3. UI está desativada por padrão?
4. UI está conectada no WeaponStation script?

### Problema: Compra não funciona (apertar E não faz nada)

**Soluções:**
1. Player está dentro do trigger?
2. Tem moedas suficientes?
3. Verifique Console para mensagens de debug
4. ScoreManager.Instance está acessível?

### Problema: Arma some mas não aparece a nova

**Soluções:**
1. Prefab da nova arma está configurado?
2. WeaponHolder existe?
3. Verifique hierarquia no Play Mode

### Problema: Atirar não funciona após trocar arma

**Soluções:**
1. Novo prefab tem o componente `Shoot`?
2. Bullet está configurado no Shoot?
3. Verifique se Input está sendo detectado

---

## 11. CHECKLIST FINAL

Antes de considerar completo:

**Scripts:**
- [ ] WeaponManager.cs criado e funcionando
- [ ] WeaponStation.cs criado e funcionando
- [ ] Enum WeaponType definido

**Prefabs:**
- [ ] Pistol.prefab configurado
- [ ] MG.prefab configurado
- [ ] Ambos têm Shoot + WeaponOrbit2D

**Cena:**
- [ ] MG_Station no mapa
- [ ] WeaponInteractionUI no Canvas
- [ ] Player com WeaponManager
- [ ] WeaponHolder criado

**Testes:**
- [ ] Player começa com Pistol
- [ ] UI aparece ao aproximar da station
- [ ] Compra funciona (com moedas)
- [ ] Compra bloqueada sem moedas
- [ ] Arma troca corretamente
- [ ] MG atira mais rápido que Pistol
- [ ] Station some após compra

---

## 12. PRÓXIMOS PASSOS

Após ter o sistema básico funcionando:

1. **Adicione mais armas** (Shotgun, Sniper, etc.)
2. **Crie stations espalhadas** pelo mapa
3. **Implemente sistema de munição**
4. **Adicione recarga** (tecla R)
5. **Crie UI de inventário** de armas
6. **Adicione sons** de compra e troca
7. **Efeitos visuais** na compra (partículas, flash)
8. **Sistema de upgrade** (melhorar armas existentes)

**Boa sorte! 🔫💰🎮**
