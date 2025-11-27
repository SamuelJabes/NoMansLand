# 📱 Guia de Configuração Mobile - NoMansLand

## ✅ FASE 1.2: Configuração Inicial do Projeto Unity para Android

### 1️⃣ **Build Settings - Adicionar Plataforma Android**

1. Abra o Unity Editor
2. Vá em `File > Build Settings`
3. Selecione **Android** na lista de plataformas
4. Clique em **Switch Platform** (isso pode demorar alguns minutos)
5. Verifique as cenas incluídas no build:
   - ✅ Main_menu (Index 0)
   - ✅ tete_mapa (Index 1) - **Cena principal do jogo**
   - ❌ SampleScene (remover se não for usar)
   - ✅ Game_Over (Index 3)
   - ✅ End (Index 4)

**IMPORTANTE**: Se houver outras cenas principais do jogo (sam_mapa_1, mapa_oficial), adicione-as também!

---

### 2️⃣ **Player Settings - Configurações Android**

Vá em `Edit > Project Settings > Player` (ou clique em "Player Settings" no Build Settings)

#### **📋 Company & Product**
- **Company Name**: `DefaultCompany` (ou mude para seu nome/equipe)
- **Product Name**: `No Mans Land` ✅ (já está configurado)

#### **📦 Identification**
Na aba **Android** (ícone do Android), expanda **Other Settings**:

- **Package Name**: `com.NoMansLand.Mobile`
  - Formato: `com.CompanyName.ProductName`
  - **NÃO use espaços ou caracteres especiais**
  - Exemplo: `com.SeuNome.NoMansLand`

- **Version**: `1.0`
- **Bundle Version Code**: `1` (incrementar a cada build)

#### **🎮 Orientation**
Em **Resolution and Presentation**:

- **Default Orientation**: `Landscape Left` (recomendado para shooter)
  - Ou `Auto Rotation` se quiser suportar ambas orientações
  - **Desmarque Portrait** se não for usar modo retrato

#### **🔧 Configuration**
Em **Other Settings**:

- **Scripting Backend**: 
  - `IL2CPP` (recomendado para performance e suporte ARM64)
  - Ou `Mono` (mais rápido para testar, mas obsoleto)

- **Target Architectures**:
  - ✅ **ARM64** (obrigatório - Google Play exige)
  - ⚠️ ARMv7 (opcional, para dispositivos antigos)

- **Minimum API Level**: `Android 7.0 'Nougat' (API level 24)` ou superior
- **Target API Level**: `Automatic (highest installed)`

#### **🎨 Graphics**
Em **Other Settings > Rendering**:

- **Graphics APIs**: 
  - `OpenGLES3` (primeira opção)
  - `OpenGLES2` (fallback para dispositivos antigos)
  - Remova `Vulkan` se tiver problemas de compatibilidade

- **Color Space**: `Linear` ✅ (já está configurado para URP)
- **Multithreaded Rendering**: ✅ Ativado (performance)

#### **🔐 Publishing Settings**
Em **Publishing Settings** (só necessário para builds de produção):

- **Keystore** (deixe em branco por enquanto para testes)
- Quando for fazer build final:
  1. Crie um novo keystore (botão "Create New")
  2. **GUARDE O ARQUIVO .keystore E A SENHA!**
  3. Será necessário para updates futuros no Google Play

---

### 3️⃣ **Quality Settings - Otimização Mobile**

Vá em `Edit > Project Settings > Quality`

1. Selecione o **Android Icon** (verde)
2. Configure o preset para **Medium** ou **Low** (para melhor performance)
3. Ajustes recomendados:
   - **V Sync Count**: `Don't Sync` (melhor para mobile)
   - **Anti Aliasing**: `Disabled` ou `2x Multi Sampling`
   - **Shadow Resolution**: `Medium Resolution`
   - **Shadow Distance**: `50-75` (reduzir se tiver lag)

---

### 4️⃣ **Input System - Verificação**

O projeto já usa o novo Input System. Verifique:

1. `Edit > Project Settings > Player > Other Settings`
2. **Active Input Handling**: deve estar em `Input System Package (New)` ou `Both`

---

### 5️⃣ **URP (Universal Render Pipeline) - Otimização Mobile**

1. Localize o asset `UniversalRenderPipelineAsset` em `Assets/Settings/`
2. Configure para mobile:
   - **Rendering Path**: `Forward`
   - **MSAA**: `Disabled` ou `2x`
   - **HDR**: ❌ Desativado (economiza memória)
   - **Shadow Resolution**: `1024` ou `2048`
   - **Max Lights**: `4-8` (reduzir para performance)

---

### 6️⃣ **Testar Build APK (Desenvolvimento)**

1. Conecte um dispositivo Android via USB (modo desenvolvedor ativado)
2. Ou use um emulador Android
3. Em `Build Settings`:
   - ✅ **Development Build** (para debug)
   - ✅ **Script Debugging** (se precisar)
   - Clique em **Build and Run**
4. Escolha onde salvar o APK (ex: `Builds/NoMansLand_v1.0_dev.apk`)

---

## 📝 **Checklist de Verificação**

Antes de continuar para a próxima fase:

- [ ] ✅ Plataforma mudada para Android no Build Settings
- [ ] ✅ Package name configurado (sem espaços)
- [ ] ✅ Orientação definida (Landscape Left)
- [ ] ✅ ARM64 habilitado (Target Architectures)
- [ ] ✅ Minimum API Level >= 24
- [ ] ✅ Quality settings otimizados para mobile
- [ ] ✅ URP configurado para mobile
- [ ] ✅ APK de teste compilado e rodando no dispositivo

---

## 🚀 **Próximos Passos**

Após completar essas configurações:
1. Testar o jogo no Android (mesmo sem controles mobile ainda)
2. Verificar performance (FPS, temperatura do dispositivo)
3. Partir para **FASE 2**: Implementação dos controles mobile (Virtual Joystick + Auto-aim)

---

## 🆘 **Troubleshooting**

### Problema: "Android SDK not found"
- Instale o **Android SDK** via Unity Hub → Installs → Add Modules → Android Build Support

### Problema: Build demora muito
- Primeira build sempre é lenta (compilação IL2CPP)
- Builds subsequentes são mais rápidas

### Problema: APK não instala no dispositivo
- Verifique se o modo desenvolvedor está ativado
- Aceite instalação de fontes desconhecidas
- Verifique compatibilidade do API Level

### Problema: Jogo roda muito lento
- Reduza Quality Settings para Low
- Desative sombras ou reduza resolução
- Reduza número de inimigos/particle effects

---

**Status**: ⏳ Aguardando configuração no Unity Editor

**Próxima Tarefa**: Implementar controles mobile (Virtual Joystick)
