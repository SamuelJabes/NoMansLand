# 🚗💰 GUIA COMPLETO: Sistema de Portas/Veículos Compráveis

## 📋 ÍNDICE
1. [Visão Geral do Sistema](#1-visão-geral-do-sistema)
2. [Preparação da Cena sam_mapa_3](#2-preparação-da-cena-sam_mapa_3)
3. [Criar UI para DoorPurchase](#3-criar-ui-para-doorpurchase)
4. [Configurar Veículos/Portas](#4-configurar-veículosportas)
5. [Adicionar Áudio (Opcional)](#5-adicionar-áudio-opcional)
6. [Testar o Sistema](#6-testar-o-sistema)
7. [Troubleshooting](#7-troubleshooting)
8. [Personalizações Avançadas](#8-personalizações-avançadas)

---

## 1. VISÃO GERAL DO SISTEMA

### O que o sistema faz:

✅ **Bloqueia passagens** com carros/ônibus/objetos
✅ **Brilha/pulsa** quando o player se aproxima
✅ **Mostra UI de interação** com preço (500 moedas)
✅ **Verifica moedas** via ScoreManager
✅ **Fade out suave** ao comprar (1.5s)
✅ **Feedback visual** (mensagens de sucesso/erro)
✅ **Suporte para áudio** (sons de compra/erro)

### Como funciona:

```
Player se aproxima → Veículo brilha + UI aparece
Player aperta [E] → Verifica moedas
                   ├─ Moedas suficientes → Gasta 500, fade out, libera passagem
                   └─ Moedas insuficientes → Mostra erro "Faltam X moedas"
```

### Componentes do sistema:

1. **DoorPurchase.cs** - Script principal (já criado ✅)
2. **DoorInteractionUI** - Panel de UI separado
3. **Collider2D (Trigger)** - Detecta proximidade do player
4. **SpriteRenderer** - Para efeito de brilho
5. **ScoreManager** - Sistema de moedas (já existe)

---

## 2. PREPARAÇÃO DA CENA sam_mapa_3

### Passo 1: Abrir a cena

1. **No Project:**
   - Navegue até `Assets/Scenes/`
   - Duplo-clique em `sam_mapa_3.unity`

2. **Se a cena não existir:**
   - File → New Scene
   - Ctrl+S → Salve como `sam_mapa_3.unity` em `Assets/Scenes/`

### Passo 2: Verificar ScoreManager

O sistema de portas usa o `ScoreManager` para verificar/gastar moedas.

1. **Verificar se ScoreManager existe:**
   - Hierarchy → busque "ScoreManager" (Ctrl+F)
   
2. **Se NÃO existir:**
   - Procure em outra cena (ex: sam_mapa_2)
   - Copie o GameObject ScoreManager
   - Cole em sam_mapa_3

3. **Ou crie um novo:**
   - Create Empty GameObject → nomeie "ScoreManager"
   - Add Component → ScoreManager script
   - Configure no Inspector

### Passo 3: Preparar Sprites dos Veículos

Você precisa de sprites para os veículos bloqueantes.

**Opções:**

**A) Usar sprites existentes do projeto**
**B) Criar placeholders temporários:**
```
- GameObject → 2D Object → Sprite → Square
- Nomeie: "Car_North" e "Car_West"
- Ajuste escala para parecer um veículo (ex: X=4, Y=2)
- Mude a cor do SpriteRenderer (cinza, azul, etc.)
```

**C) Importar sprites de veículos:**
- Baixe de: opengameart.org, kenney.nl, itch.io
- Arraste para `Assets/assets_2/Sprites/` (ou crie essa pasta)

---

## 3. CRIAR UI PARA DOORPURCHASE

### Passo 1: Criar/Usar Canvas

1. **Se já tem Canvas (HUD_Canvas):**
   - Use o Canvas existente
   - Pule para Passo 2

2. **Se NÃO tem Canvas:**
   - Hierarchy → Botão direito → UI → Canvas
   - Nomeie: `HUD_Canvas`
   - Configure:
     ```
     Render Mode: Screen Space - Overlay
     Canvas Scaler → UI Scale Mode: Scale With Screen Size
     Reference Resolution: 1920 x 1080
     ```

### Passo 2: Criar Panel para DoorInteraction

1. **Criar Panel:**
   - Botão direito no Canvas → UI → Panel
   - Nomeie: `DoorInteractionUI`

2. **Configurar RectTransform:**
   ```
   Anchor Preset: Bottom-Center
   Pivot: (0.5, 0)
   Pos X: 0
   Pos Y: 150
   Width: 400
   Height: 120
   ```

3. **Configurar Image (background):**
   ```
   Color: Preto semi-transparente (R:0, G:0, B:0, A:0.8)
   ```

### Passo 3: Criar Text dentro do Panel

1. **Criar TextMeshPro:**
   - Botão direito em `DoorInteractionUI` → UI → Text - TextMeshPro
   - Nomeie: `InteractionText`
   
2. **Se pedir para importar TMP Essentials:**
   - Clique "Import TMP Essentials"
   - Aguarde importação

3. **Configurar RectTransform:**
   ```
   Anchor: Stretch (ALT+SHIFT, canto inferior direito)
   Left: 10
   Right: 10
   Top: 10
   Bottom: 10
   ```

4. **Configurar TextMeshProUGUI:**
   ```
   Text: [E] Abrir Passagem
        500 Moedas
   
   Font: Liberation Sans SDF (padrão)
   Font Style: Bold
   Font Size: 28
   Alignment: Center (horizontal e vertical)
   Wrapping: Enabled
   Color: Branco (R:1, G:1, B:1, A:1)
   ```

### Passo 4: Desativar o Panel por padrão

1. **Selecione `DoorInteractionUI`**
2. **No topo do Inspector:**
   - Desmarque o checkbox ao lado do nome
   - Isso desativa o GameObject

**Hierarquia final:**
```
HUD_Canvas
  ├── (outros elementos como HeartsHealthUI, WeaponInteractionUI)
  └── DoorInteractionUI (DESATIVADO por padrão)
      └── InteractionText (TextMeshPro)
```

---

## 4. CONFIGURAR VEÍCULOS/PORTAS

Agora vamos configurar os 2 veículos bloqueantes (Norte e Oeste).

### Configuração do Veículo Norte (Passagem Norte)

#### Passo 1: Criar GameObject

1. **No Hierarchy:**
   - Create Empty GameObject (ou 2D Object → Sprite)
   - Nomeie: `Car_North`

2. **Posicionar:**
   - Coloque na posição que bloqueia a passagem norte
   - Ex: Position (X:0, Y:10, Z:0) - ajuste conforme seu mapa

#### Passo 2: Adicionar SpriteRenderer

1. **Se não tem SpriteRenderer:**
   - Add Component → Sprite Renderer

2. **Configurar:**
   ```
   Sprite: [seu sprite de carro/ônibus]
   Color: Branco (R:1, G:1, B:1, A:1)
   Sorting Layer: Default (ou crie "Obstacles")
   Order in Layer: 5
   ```

3. **Ajustar escala:**
   - Scale: Ajuste para cobrir a passagem
   - Ex: X=3, Y=1.5 (depende do sprite)

#### Passo 3: Adicionar Collider

1. **Add Component → Box Collider 2D**

2. **Configurar:**
   ```
   Is Trigger: ✓ (MARQUE ISSO!)
   Size: Ajuste para área de interação
   ```
   
   **Dica:** O collider define a área onde o player precisa estar para interagir. Faça um pouco maior que o sprite para facilitar a interação.

3. **Visualizar área:**
   - No Scene View, o collider aparece em verde
   - Ajuste `Edit Collider` para cobrir a área desejada

#### Passo 4: Adicionar Collider de Bloqueio (OPCIONAL)

Se quiser que o player **NÃO atravesse** antes de comprar:

1. **Add Component → Box Collider 2D** (um segundo collider)

2. **Configurar:**
   ```
   Is Trigger: ✗ (DESMARCADO!)
   Size: Tamanho do sprite (para bloquear fisicamente)
   ```

3. **Desabilitar após compra:**
   - O script DoorPurchase destrói o GameObject inteiro
   - Então esse collider também sumirá ✅

#### Passo 5: Adicionar Script DoorPurchase

1. **Add Component → Door Purchase**

2. **Configurar no Inspector:**
   ```
   PURCHASE SETTINGS:
     Price: 500
     Door Name: "Passagem Norte"
   
   UI REFERENCES:
     Interaction UI: Arraste DoorInteractionUI do Canvas
     Message Text: Arraste InteractionText do DoorInteractionUI
   
   VISUAL EFFECTS:
     Highlight Color: Amarelo (R:1, G:1, B:0, A:0.5)
     Pulse Speed: 2
     Fade Out Duration: 1.5
   
   AUDIO (OPTIONAL):
     Audio Source: (vazio por enquanto)
     Purchase Success Clip: (vazio)
     Purchase Fail Clip: (vazio)
   
   DEBUG:
     Show Debug Messages: ✓
   ```

#### Passo 6: Adicionar AudioSource (para futuros sons)

1. **Add Component → Audio Source**

2. **Configurar:**
   ```
   Play On Awake: ✗
   Loop: ✗
   Volume: 0.7
   ```

3. **Arrastar para o script:**
   - No DoorPurchase, campo `Audio Source`
   - Arraste o componente AudioSource

---

### Configuração do Veículo Oeste (Passagem Oeste)

**Repita TODOS os passos acima**, mas com estas mudanças:

```
GameObject Name: Car_West
Position: Posição que bloqueia passagem oeste (ex: X:-10, Y:0)
Door Name: "Passagem Oeste"
Interaction UI: Mesmo DoorInteractionUI (reutilizar)
Message Text: Mesmo InteractionText (reutilizar)
```

**IMPORTANTE:** Você pode usar a **mesma UI** para todas as portas! O script DoorPurchase atualiza o texto dinamicamente.

---

### Checklist de Configuração (para cada veículo):

- [ ] GameObject criado e posicionado
- [ ] SpriteRenderer com sprite configurado
- [ ] Box Collider 2D (Is Trigger = ✓)
- [ ] (Opcional) Box Collider 2D adicional (Is Trigger = ✗) para bloquear
- [ ] DoorPurchase script adicionado
- [ ] Price = 500
- [ ] Door Name configurado (único para cada porta)
- [ ] Interaction UI conectado
- [ ] Message Text conectado
- [ ] AudioSource adicionado
- [ ] GameObject ativo no Inspector

---

## 5. ADICIONAR ÁUDIO (OPCIONAL)

### Sons Recomendados

Você precisa de **2 sons**:

1. **purchaseSuccessClip** - Som de compra bem-sucedida
   - Tipo: "cha-ching", moeda, sino, "power up"
   - Duração: 0.5-1s

2. **purchaseFailClip** - Som de erro/moedas insuficientes
   - Tipo: "buzz", "error", "negative beep"
   - Duração: 0.3-0.5s

### Onde Encontrar Sons Gratuitos

**Sites recomendados:**
- **Freesound.org** (busque "coin", "success", "error", "buzz")
- **Zapsplat.com** (UI sounds)
- **Kenney.nl** (pacote de UI Audio)
- **OpenGameArt.org**

### Importar e Configurar

1. **Baixar os sons** (.wav ou .mp3)

2. **Importar para Unity:**
   - Arraste para `Assets/assets_2/sounds/` (ou crie essa pasta)

3. **Configurar cada AudioClip:**
   - Selecione o arquivo no Project
   - Inspector:
     ```
     Load Type: Decompress On Load (para sons curtos)
     Preload Audio Data: ✓
     Compression Format: Vorbis (para .ogg) ou PCM (para qualidade máxima)
     Quality: 70-100
     ```

4. **Conectar aos veículos:**
   - Selecione `Car_North`
   - No DoorPurchase script:
     - `Purchase Success Clip`: Arraste o som de sucesso
     - `Purchase Fail Clip`: Arraste o som de erro
   - Repita para `Car_West`

---

## 6. TESTAR O SISTEMA

### Teste 1: Verificação Básica

1. **Salvar a cena:** Ctrl+S

2. **Aperte Play** ▶️

3. **Verificar no Console:**
   ```
   Deve aparecer (se showDebugMessages = true):
   - Nenhum erro vermelho
   - ScoreManager encontrado
   ```

### Teste 2: Aproximação e UI

1. **Mova o player até um veículo**

2. **O que deve acontecer:**
   - ✅ Veículo começa a **brilhar/pulsar** (amarelado)
   - ✅ UI aparece na parte inferior: `[E] Abrir Passagem Norte / 500 Moedas`
   - ✅ Console mostra: "Player entrou na área de Passagem Norte"

3. **Afaste-se do veículo:**
   - ✅ Brilho para
   - ✅ UI desaparece
   - ✅ Console mostra: "Player saiu da área de Passagem Norte"

### Teste 3: Tentar Comprar SEM Moedas

1. **Certifique-se que tem MENOS de 500 moedas**
   - No Inspector, selecione ScoreManager
   - Veja o valor de `currentCoins`

2. **Aproxime do veículo e aperte [E]**

3. **O que deve acontecer:**
   - ✅ Som de erro toca (se configurado)
   - ✅ UI mostra: "Sem moedas! / Faltam: X"
   - ✅ Console mostra: "Moedas insuficientes! Precisa de mais X moedas"
   - ❌ Veículo **NÃO desaparece**

4. **Aguarde 2 segundos:**
   - ✅ UI volta ao normal: "[E] Abrir Passagem Norte / 500 Moedas"

### Teste 4: Comprar COM Moedas Suficientes

#### Opção A - Ganhar moedas matando inimigos:
- Mate 50 inimigos (10 moedas cada = 500 moedas)

#### Opção B - Dar moedas via Inspector (DEBUG):
1. **Pause o jogo** (ou no Play Mode)
2. **Selecione ScoreManager** no Hierarchy
3. **No Inspector:**
   - Encontre o campo `currentCoins` (pode estar em "Debug" mode)
   - Mude para `500` ou mais
4. **Resume o jogo**

#### Testar a compra:

1. **Aproxime do veículo**
2. **Aperte [E]**
3. **O que deve acontecer:**
   - ✅ Som de sucesso toca (se configurado)
   - ✅ ScoreManager perde 500 moedas
   - ✅ UI mostra: "Passagem Norte Desbloqueada!" (verde)
   - ✅ Veículo faz **fade out suave** (1.5s)
   - ✅ Veículo **desaparece completamente**
   - ✅ Passagem está **livre**!
   - ✅ Console mostra: "Passagem Norte comprada! Gastou 500 moedas"

### Teste 5: Tentar Comprar Novamente

1. **Aproxime-se da área onde estava o veículo**
2. **Nada deve acontecer** (veículo foi destruído)

### Teste 6: Segunda Porta

1. **Repita os testes com o segundo veículo** (Car_West)
2. **Tudo deve funcionar igual**, mas com "Passagem Oeste"

---

## 7. TROUBLESHOOTING

### Problema 1: UI não aparece ao se aproximar

**Sintomas:**
- Veículo brilha, mas texto não aparece

**Soluções:**

✅ **Verificar se DoorInteractionUI está conectado:**
```
Selecione Car_North → Inspector → DoorPurchase
Campo "Interaction UI" → Deve ter DoorInteractionUI conectado
Campo "Message Text" → Deve ter InteractionText conectado
```

✅ **Verificar Canvas:**
```
Canvas deve ter:
- Render Mode: Screen Space - Overlay
- Canvas Scaler presente
```

✅ **Verificar EventSystem:**
```
Hierarchy → Deve ter um "EventSystem"
Se não tiver: GameObject → UI → Event System
```

### Problema 2: Veículo não brilha

**Sintomas:**
- UI aparece, mas sprite não pulsa

**Soluções:**

✅ **Verificar SpriteRenderer:**
```
Car_North → Inspector
Deve ter componente SpriteRenderer
Color deve ser visível (não transparente)
```

✅ **Verificar Highlight Color:**
```
DoorPurchase → Visual Effects
Highlight Color → Alpha (A) deve ser > 0
Ex: R:1, G:1, B:0, A:0.5
```

### Problema 3: Player atravessa o veículo

**Sintomas:**
- Player passa por cima/através do sprite

**Soluções:**

✅ **Adicionar Collider físico:**
```
Car_North → Add Component → Box Collider 2D
Is Trigger: ✗ (DESMARCADO)
Size: Ajuste para cobrir o sprite
```

✅ **Verificar Player:**
```
Player deve ter:
- Rigidbody2D (Dynamic)
- Collider2D (não trigger)
```

### Problema 4: [E] não funciona

**Sintomas:**
- UI aparece, mas apertar E não faz nada

**Soluções:**

✅ **Verificar tag do Player:**
```
Selecione Player → Tag DEVE ser "Player"
```

✅ **Verificar Input:**
```
O script usa Input.GetKeyDown(KeyCode.E)
Tente apertar E quando a UI estiver visível
```

✅ **Verificar Console:**
```
Deve aparecer mensagens de debug
Se não aparecer nada, o script não está rodando
```

### Problema 5: ScoreManager não encontrado

**Sintomas:**
- Console mostra: "ScoreManager não encontrado!"

**Soluções:**

✅ **Verificar se existe na cena:**
```
Hierarchy → Busque "ScoreManager"
Deve ter o script ScoreManager.cs
```

✅ **Verificar Singleton:**
```
ScoreManager deve ser Singleton
Código deve ter: public static ScoreManager Instance;
```

✅ **Copiar de outra cena:**
```
Abra sam_mapa_2 → Copie ScoreManager
Abra sam_mapa_3 → Cole (Ctrl+V)
```

### Problema 6: Moedas não diminuem

**Sintomas:**
- Veículo desaparece, mas moedas não são gastas

**Soluções:**

✅ **Verificar TrySpendCoins:**
```
ScoreManager deve ter método TrySpendCoins(int amount)
Retorna bool e diminui currentCoins
```

✅ **Verificar UI de moedas:**
```
Pode estar diminuindo mas UI não atualiza
Verifique ScoreManager.UpdateUI()
```

### Problema 7: Fade out não funciona

**Sintomas:**
- Veículo desaparece instantaneamente

**Soluções:**

✅ **Verificar Fade Out Duration:**
```
DoorPurchase → Visual Effects
Fade Out Duration deve ser > 0 (ex: 1.5)
```

✅ **Verificar SpriteRenderer:**
```
Fade out só funciona se tiver SpriteRenderer
Sem ele, desaparece instantaneamente
```

### Problema 8: Som não toca

**Sintomas:**
- Compra funciona, mas sem som

**Soluções:**

✅ **Verificar AudioSource:**
```
Car_North → Deve ter AudioSource component
No DoorPurchase → Audio Source deve estar conectado
```

✅ **Verificar AudioClips:**
```
DoorPurchase → Audio (Optional)
Purchase Success Clip → Deve ter o som
Purchase Fail Clip → Deve ter o som
```

✅ **Verificar Volume:**
```
AudioSource → Volume deve ser > 0 (ex: 0.7)
Audio Listener deve existir na câmera
```

### Problema 9: Compra múltipla

**Sintomas:**
- Pode comprar várias vezes (gasta moedas múltiplas)

**Soluções:**

✅ **Não deve acontecer** - o script tem proteção:
```csharp
if (isPurchased) return; // No início de OnTriggerEnter2D
```

Se acontecer, verifique se há **múltiplos DoorPurchase scripts** no mesmo GameObject.

---

## 8. PERSONALIZAÇÕES AVANÇADAS

### 8.1 Mudar o Preço de Portas Específicas

Cada porta pode ter preço diferente:

```
Car_North:
  Price: 500 (passagem fácil)

Car_West:
  Price: 1000 (passagem difícil/secreta)
```

### 8.2 Adicionar Animação em vez de Fade

Modifique `FadeOutAndDestroy()`:

```csharp
IEnumerator FadeOutAndDestroy()
{
    // Em vez de fade, mover para o lado
    Vector3 startPos = transform.position;
    Vector3 endPos = startPos + Vector3.left * 5f; // Move 5 unidades para esquerda
    
    float elapsedTime = 0f;
    while (elapsedTime < fadeOutDuration)
    {
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fadeOutDuration);
        yield return null;
    }
    
    Destroy(gameObject);
}
```

### 8.3 Requisito de Inimigos Mortos

Para abrir uma porta, deve ter matado X inimigos:

```csharp
[SerializeField] private int requiredKills = 20;

void TryPurchase()
{
    // Verificar kills (assumindo que ScoreManager tem esse dado)
    if (scoreManager.TotalKills < requiredKills)
    {
        messageText.text = $"<color=red>Mate {requiredKills} inimigos primeiro!</color>";
        return;
    }
    
    // Resto do código normal...
}
```

### 8.4 Múltiplas Portas com Ordem

Forçar ordem: só pode comprar Porta 2 após Porta 1:

```csharp
[SerializeField] private DoorPurchase requiredDoor; // Porta que precisa estar aberta

void TryPurchase()
{
    if (requiredDoor != null && !requiredDoor.IsPurchased)
    {
        messageText.text = $"<color=red>Abra {requiredDoor.DoorName} primeiro!</color>";
        return;
    }
    
    // Resto do código...
}
```

### 8.5 Indicador Visual de "Locked"

Adicione um ícone de cadeado:

1. **Crie um child GameObject:**
   - Botão direito em Car_North → 2D Object → Sprite
   - Nomeie: `LockIcon`
   - Sprite: [sprite de cadeado]
   - Posicione acima do veículo

2. **Modificar script:**
```csharp
[SerializeField] private GameObject lockIcon;

void OnPurchaseSuccess()
{
    // Esconde o cadeado
    if (lockIcon != null)
        lockIcon.SetActive(false);
    
    // Resto do código...
}
```

### 8.6 Partículas ao Desbloquear

Efeito de partículas quando compra:

1. **Adicionar Particle System:**
   - Car_North → Add Component → Particle System
   - Configure: Burst, cor dourada, etc.
   - Play On Awake: ✗

2. **No script:**
```csharp
[SerializeField] private ParticleSystem unlockParticles;

void OnPurchaseSuccess()
{
    if (unlockParticles != null)
        unlockParticles.Play();
    
    // Resto do código...
}
```

### 8.7 Salvar Estado das Portas

Para que portas abertas continuem abertas ao morrer/mudar de cena:

```csharp
void OnPurchaseSuccess()
{
    // Salvar no PlayerPrefs
    PlayerPrefs.SetInt($"Door_{doorName}_Purchased", 1);
    PlayerPrefs.Save();
    
    // Resto do código...
}

void Start()
{
    // Verificar se já foi comprada
    if (PlayerPrefs.GetInt($"Door_{doorName}_Purchased", 0) == 1)
    {
        // Já foi comprada antes - destruir imediatamente
        Destroy(gameObject);
        return;
    }
    
    // Resto do código...
}
```

### 8.8 Diferentes Efeitos Visuais de Highlight

**Opção A - Outline em vez de brilho:**
```csharp
// Use um Sprite Outline shader ou adicione um segundo sprite
```

**Opção B - Shake/Tremer:**
```csharp
IEnumerator ShakeHighlight()
{
    Vector3 originalPos = transform.position;
    
    while (playerInRange && !isPurchased)
    {
        float offsetX = Random.Range(-0.05f, 0.05f);
        float offsetY = Random.Range(-0.05f, 0.05f);
        transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
        
        yield return new WaitForSeconds(0.05f);
    }
    
    transform.position = originalPos;
}
```

---

## 📊 RESUMO DA CONFIGURAÇÃO

### Para cada Porta/Veículo:

```
GameObject (ex: Car_North)
├── SpriteRenderer (sprite do veículo)
├── Box Collider 2D (Is Trigger = ✓, para detecção)
├── Box Collider 2D (Is Trigger = ✗, para bloqueio físico) [OPCIONAL]
├── AudioSource (para sons)
└── DoorPurchase Script
    ├── Price: 500
    ├── Door Name: "Passagem Norte"
    ├── Interaction UI: DoorInteractionUI (do Canvas)
    ├── Message Text: InteractionText (do Panel)
    ├── Highlight Color: (1, 1, 0, 0.5)
    ├── Pulse Speed: 2
    ├── Fade Out Duration: 1.5
    ├── Audio Source: (AudioSource do próprio GameObject)
    ├── Purchase Success Clip: [som de sucesso]
    └── Purchase Fail Clip: [som de erro]
```

### UI Compartilhada (todas as portas usam a mesma):

```
HUD_Canvas
└── DoorInteractionUI (Panel - DESATIVADO por padrão)
    └── InteractionText (TextMeshPro)
```

---

## ✅ CHECKLIST FINAL

Antes de considerar completo:

**Scripts:**
- [ ] DoorPurchase.cs criado em `Assets/Scripts/Sam/`

**UI:**
- [ ] Canvas existe (HUD_Canvas)
- [ ] DoorInteractionUI Panel criado
- [ ] InteractionText TextMeshPro configurado
- [ ] Panel desativado por padrão

**Veículo Norte:**
- [ ] Car_North GameObject criado
- [ ] Posicionado bloqueando passagem norte
- [ ] SpriteRenderer com sprite
- [ ] Box Collider 2D (trigger) para detecção
- [ ] (Opcional) Box Collider 2D (não trigger) para bloqueio
- [ ] DoorPurchase script adicionado
- [ ] Price = 500, Door Name = "Passagem Norte"
- [ ] UI conectada (Interaction UI + Message Text)
- [ ] AudioSource adicionado

**Veículo Oeste:**
- [ ] Car_West GameObject criado
- [ ] Posicionado bloqueando passagem oeste
- [ ] Mesmas configurações do Norte
- [ ] Door Name = "Passagem Oeste"

**Sistema:**
- [ ] ScoreManager existe na cena
- [ ] Player tem tag "Player"
- [ ] Player tem Rigidbody2D + Collider2D

**Testes:**
- [ ] Aproximação faz brilhar e mostra UI
- [ ] [E] sem moedas mostra erro
- [ ] [E] com moedas compra e faz fade out
- [ ] Passagem fica livre após compra
- [ ] Segunda porta funciona independente

**Áudio (Opcional):**
- [ ] Sons baixados/criados
- [ ] AudioClips importados
- [ ] Sons conectados no script

---

## 🎮 PRÓXIMOS PASSOS

Após implementar o sistema básico, você pode:

1. **Adicionar mais portas** (basta duplicar Car_North e reposicionar)
2. **Variar os preços** (500, 1000, 2000...)
3. **Criar portas especiais** (requer kills, requer item, etc.)
4. **Adicionar visual de progresso** (barra mostrando moedas/preço)
5. **Salvar estado das portas** (PlayerPrefs)
6. **Adicionar NPCs vendedores** (em vez de interagir direto com veículo)
7. **Sistema de chaves** (encontrar chave OU comprar)

---

**Boa sorte com as portas compráveis! 🚗💰✨**

Se tiver dúvidas sobre algum passo, é só perguntar!
