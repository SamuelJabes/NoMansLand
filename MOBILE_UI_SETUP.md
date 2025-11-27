# 🎮 Guia de Setup - UI Mobile Controls

## 📋 Checklist de Implementação

### ✅ Scripts Criados (FEITO)
- [x] MobileInputManager.cs
- [x] MobileJoystick.cs
- [x] AutoAimSystem.cs
- [x] ShootButton.cs
- [x] InteractButton.cs

### ✅ Scripts Adaptados (FEITO)
- [x] PlayerMovementSam.cs - Usa joystick ou teclado
- [x] WeaponOrbit2D.cs - Usa auto-aim ou mouse
- [x] Shoot.cs - Usa botão ou mouse
- [x] WeaponStation.cs - Usa botão E ou tecla
- [x] DoorPurchase.cs - Usa botão E ou tecla
- [x] EnemyHealth.cs - Método IsDead() adicionado
- [x] BossHealth.cs - Método IsDead() adicionado

---

## 🎨 AGORA: Criar UI no Unity Editor

### **PASSO 1: Criar Canvas Mobile**

1. **Criar Canvas Principal**
   - Hierarchy: `Right-click > UI > Canvas`
   - Renomeie para: `MobileCanvas`

2. **Configurar Canvas**
   - Select `MobileCanvas`
   - Inspector > Canvas Scaler:
     - **UI Scale Mode**: `Scale With Screen Size`
     - **Reference Resolution**: `1920 x 1080`
     - **Screen Match Mode**: `Match Width Or Height`
     - **Match**: `0.5` (meio-termo)

3. **Adicionar MobileInputManager**
   - `MobileCanvas` > Inspector
   - `Add Component` > `Mobile Input Manager`
   - **Force Mobile Mode**: ☐ (deixe desmarcado)
     - ✅ Desmarcado = Detecta automaticamente
     - ☑️ Marcado = Força controles mobile no PC (para testar)

---

### **PASSO 2: Criar Virtual Joystick**

1. **Criar Container**
   - `MobileCanvas` > `Right-click > Create Empty`
   - Renomeie: `Joystick`

2. **Posicionar à Esquerda**
   - Select `Joystick`
   - Inspector > Rect Transform:
     - **Anchors**: Bottom-Left
     - **Pos X**: `200`
     - **Pos Y**: `200`
     - **Width**: `300`
     - **Height**: `300`

3. **Criar Background**
   - `Joystick` > `Right-click > UI > Image`
   - Renomeie: `Background`
   - Inspector:
     - **Color**: Branco semi-transparente (Alpha = 0.3)
     - **Rect Transform**: Stretch (Fill parent)

4. **Criar Handle**
   - `Joystick` > `Right-click > UI > Image`
   - Renomeie: `Handle`
   - Inspector:
     - **Color**: Branco semi-transparente (Alpha = 0.6)
     - **Width**: `120`
     - **Height**: `120`
     - **Pos X/Y**: `0, 0` (centralizado)

5. **Adicionar Script**
   - Select `Joystick`
   - `Add Component` > `Mobile Joystick`
   - Inspector:
     - **Background**: Arraste `Background`
     - **Handle**: Arraste `Handle`
     - **Handle Range**: `100` (distância máxima do centro)
     - **Return To Center**: ✅ (volta ao centro quando solta)

---

### **PASSO 3: Criar Botão de Disparo**

1. **Criar Botão**
   - `MobileCanvas` > `Right-click > UI > Image`
   - Renomeie: `ShootButton`

2. **Posicionar à Direita**
   - Inspector > Rect Transform:
     - **Anchors**: Bottom-Right
     - **Pos X**: `-200`
     - **Pos Y**: `200`
     - **Width**: `200`
     - **Height**: `200`

3. **Visual**
   - Inspector:
     - **Color**: Vermelho semi-transparente `(1, 0.3, 0.3, 0.5)`
     - **Image**: (Pode adicionar sprite de mira/alvo depois)

4. **Adicionar Texto**
   - `ShootButton` > `Right-click > UI > Text - TextMeshPro`
   - Renomeie: `Label`
   - Text: `🔫` ou `FIRE`
   - **Font Size**: `60`
   - **Alignment**: Center/Middle

5. **Adicionar Script**
   - Select `ShootButton`
   - `Add Component` > `Shoot Button`
   - Inspector:
     - **Normal Color**: Vermelho claro `(1, 1, 1, 0.5)`
     - **Pressed Color**: Vermelho forte `(1, 0.3, 0.3, 0.8)`

---

### **PASSO 4: Criar Botão de Interação (E)**

1. **Criar Botão**
   - `MobileCanvas` > `Right-click > UI > Image`
   - Renomeie: `InteractButton`

2. **Posicionar à Direita (acima do Shoot)**
   - Inspector > Rect Transform:
     - **Anchors**: Bottom-Right
     - **Pos X**: `-200`
     - **Pos Y**: `450`
     - **Width**: `180`
     - **Height**: `180`

3. **Visual**
   - Inspector:
     - **Color**: Verde semi-transparente `(0.3, 1, 0.3, 0.5)`

4. **Adicionar Texto**
   - `InteractButton` > `Right-click > UI > Text - TextMeshPro`
   - Renomeie: `Label`
   - Text: `E`
   - **Font Size**: `80`
   - **Alignment**: Center/Middle

5. **Adicionar Script**
   - Select `InteractButton`
   - `Add Component` > `Interact Button`
   - Inspector:
     - **Normal Color**: Verde claro `(1, 1, 1, 0.5)`
     - **Pressed Color**: Verde forte `(0.3, 1, 0.3, 0.8)`
     - **Hide When Not Needed**: ✅ (aparece só quando necessário)

6. **Adicionar Canvas Group** (já é feito automaticamente pelo script)

---

### **PASSO 5: Criar Auto-Aim System**

1. **Criar GameObject**
   - Hierarchy: `Right-click > Create Empty`
   - Renomeie: `AutoAimSystem`

2. **Adicionar Script**
   - Select `AutoAimSystem`
   - `Add Component` > `Auto Aim System`
   - Inspector:
     - **Player**: Arraste o GameObject `Player`
     - **Detection Range**: `15` (alcance de detecção)
     - **Enemy Layer**: `Enemy` (crie layer se não existir)
     - **Show Debug Gizmos**: ✅ (para visualizar no Scene)

---

### **PASSO 6: Conectar Scripts do Player**

1. **PlayerMovementSam**
   - Select `Player`
   - Inspector > PlayerMovementSam:
     - **Mobile Joystick**: Arraste `Joystick` do Canvas

2. **WeaponOrbit2D (Arma)**
   - Select a arma atual (Pistol/Shotgun/MG)
   - Inspector > WeaponOrbit2D:
     - **Auto Aim**: Arraste `AutoAimSystem` da Hierarchy

3. **Shoot (Arma)**
   - Select a arma atual
   - Inspector > Shoot:
     - **Shoot Button**: Arraste `ShootButton` do Canvas

---

### **PASSO 7: Configurar Layers (IMPORTANTE)**

1. **Criar Layer "Enemy"**
   - Menu: `Edit > Project Settings > Tags and Layers`
   - **Layers**:
     - Layer 6: `Enemy`

2. **Aplicar Layer aos Inimigos**
   - Select prefabs: `Small Zombie`, `Large Zombie`
   - Inspector > Layer: `Enemy`
   - Apply to children: Yes

3. **Aplicar ao Boss**
   - Select `Boss` (se tiver na cena)
   - Inspector > Layer: `Enemy`

---

## 🧪 TESTAR NO UNITY EDITOR

### **Modo PC (Mouse/Teclado)**

1. Play
2. WASD = Movimento
3. Mouse = Mira
4. Click = Atirar
5. E = Interagir

### **Modo Mobile (Simular com Mouse)**

1. Select `MobileCanvas` > MobileInputManager
2. ✅ **Force Mobile Mode** (marque)
3. Play
4. **Joystick**: Click e arraste
5. **Shoot Button**: Segure click
6. **Interact Button**: Click

---

## ✅ Checklist Final

- [ ] Canvas criado com Canvas Scaler configurado
- [ ] MobileInputManager no Canvas
- [ ] Joystick funcional (testa arrastando)
- [ ] Botão de disparo funcional
- [ ] Botão de interação funcional (aparece/desaparece)
- [ ] AutoAimSystem criado e configurado
- [ ] Layer "Enemy" criado e aplicado
- [ ] PlayerMovementSam conectado ao joystick
- [ ] WeaponOrbit2D conectado ao auto-aim
- [ ] Shoot conectado ao shoot button
- [ ] Testado no Editor (Force Mobile Mode)

---

## 🎨 Melhorias Visuais (Opcional)

### **Sprites para Botões**

Você pode criar ou baixar sprites:
- **Joystick**: Círculos com seta
- **Shoot Button**: Mira/alvo
- **Interact Button**: Ícone de mão/E

### **Animações**

Adicionar:
- Fade in/out do InteractButton
- Pulse no ShootButton ao disparar
- Feedback tátil (vibração) - adicionar depois

---

## 🚀 Próximos Passos

Depois de testar:
1. Build APK Android
2. Testar em dispositivo real
3. Ajustar posições/tamanhos dos botões
4. Ajustar sensibilidade do joystick
5. Ajustar alcance do auto-aim

---

**Status**: ⏳ Aguardando setup da UI no Unity Editor

**Dúvidas?** Qualquer problema, me avisa que eu ajudo! 👍
