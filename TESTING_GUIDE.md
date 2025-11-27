# 🧪 Guia de Testes Mobile - NoMansLand

## 📋 Checklist de Testes Android

Use este guia para testar o jogo em diferentes dispositivos e cenários.

---

## 🔍 Testes Funcionais

### ✅ Controles Touch

- [ ] Joystick virtual responde corretamente
- [ ] Movimento do player é fluido (sem travamentos)
- [ ] Auto-aim funciona e mira no inimigo mais próximo
- [ ] Botão de disparo responde instantaneamente
- [ ] Botão de interação (E) aparece quando necessário
- [ ] Multi-touch funciona (mover + atirar simultaneamente)

### ✅ Gameplay

- [ ] Player inicia com vida cheia (3 corações)
- [ ] Zumbis aparecem nos spawners
- [ ] Zumbis perseguem o player corretamente
- [ ] Tiros acertam e causam dano nos zumbis
- [ ] Coleta de moedas funciona
- [ ] Sistema de score incrementa corretamente
- [ ] WeaponStation permite comprar armas
- [ ] DoorPurchase desbloqueia áreas
- [ ] Boss aparece e ataca corretamente
- [ ] Game Over ocorre quando vida zera
- [ ] Vitória ocorre quando boss morre

### ✅ UI/UX

- [ ] Todos textos são legíveis no celular
- [ ] Botões são grandes o suficiente para tocar (mín 60x60px)
- [ ] HUD não obstrui gameplay importante
- [ ] Score e moedas visíveis o tempo todo
- [ ] Mensagens de compra aparecem claramente
- [ ] Transições entre cenas funcionam
- [ ] Menu principal responsivo

### ✅ Audio

- [ ] Música de fundo toca corretamente
- [ ] Sons de tiro audíveis
- [ ] Sons de dano/morte funcionam
- [ ] Música do boss muda ao entrar na arena
- [ ] Volume balanceado (não muito alto/baixo)

---

## 📱 Testes de Dispositivos

### Dispositivos Recomendados para Teste

Teste em pelo menos **2 dispositivos diferentes**:

#### **Tier 1: High-End** (2020+)
- Snapdragon 8xx / Exynos 9xx / Dimensity 9xxx
- 6GB+ RAM
- Android 11+
- **Exemplo**: Samsung Galaxy S21+, Xiaomi Mi 11

#### **Tier 2: Mid-Range** (2018-2020)
- Snapdragon 6xx / Exynos 7xx
- 3-4GB RAM
- Android 9-10
- **Exemplo**: Moto G8, Samsung A51

#### **Tier 3: Low-End** (2016-2018)
- Snapdragon 4xx
- 2GB RAM
- Android 7-8
- **Exemplo**: Moto E6, Samsung J7

### Checklist por Dispositivo

Para cada dispositivo, preencha:

**Dispositivo**: _________________
**Modelo**: _________________
**Android Version**: _________________
**RAM**: _________________

| Teste | Passou? | Observações |
|-------|---------|-------------|
| Instala sem erro | ☐ | |
| FPS >= 30 | ☐ | FPS médio: ___ |
| Sem lag no gameplay | ☐ | |
| Touch preciso | ☐ | |
| Áudio funciona | ☐ | |
| Não esquenta demais | ☐ | Temp: ___ |
| Bateria consome normal | ☐ | % em 10min: ___ |
| Sem crashes | ☐ | |

---

## ⚡ Testes de Performance

### FPS (Frames Per Second)

**Meta**: 30-60 FPS constante

**Como medir**:
1. Habilite Development Build
2. Use Profiler do Unity (conectado via ADB)
3. Ou adicione contador de FPS na tela

**Cenários de teste**:
- [ ] Menu principal: ___ FPS
- [ ] Jogo com 5 zumbis: ___ FPS
- [ ] Jogo com 15 zumbis: ___ FPS
- [ ] Boss fight: ___ FPS
- [ ] Muitos tiros na tela: ___ FPS

**Se FPS < 30**:
- Reduza Quality Settings para Low
- Diminua Shadow Distance/Resolution
- Reduza Max Lights no URP
- Diminua tamanho dos Object Pools

### Temperatura

**Meta**: Não esquentar excessivamente (< 40°C ambiente + 15°C)

**Como medir**:
- Use apps como CPU-Z, AIDA64
- Ou sinta o calor do dispositivo

**Teste**:
- Jogar por 10 minutos contínuos
- Verificar se esquenta muito

**Se esquentar muito**:
- Reduza qualidade gráfica
- Diminua spawn rate de inimigos
- Otimize iluminação (menos luzes dinâmicas)

### Consumo de Bateria

**Meta**: ~10-15% de bateria em 10 minutos de jogo

**Como medir**:
1. Carregue bateria a 100%
2. Desconecte carregador
3. Jogue por 10 minutos
4. Verifique percentual restante

**Se consumir muito** (>20% em 10min):
- Reduza qualidade gráfica
- Diminua frequência de spawn
- Otimize física (menos Rigidbody2D ativos)

---

## 🐛 Testes de Bugs Comuns

### Checklist de Bugs Conhecidos

- [ ] **Touch não registra**: Verifique Event System na cena
- [ ] **Joystick não move**: Verifique RectTransform anchors
- [ ] **Auto-aim não funciona**: Verifique layer dos inimigos
- [ ] **Arma não atira**: Verifique Input System
- [ ] **UI desalinhada**: Ajuste Canvas Scaler (Scale with Screen Size)
- [ ] **Zumbis não aparecem**: Verifique NavMesh e spawners
- [ ] **Crash ao trocar cena**: Verifique DontDestroyOnLoad (ScoreManager)
- [ ] **Sem áudio**: Verifique permissões Android
- [ ] **Lag ao spawnar**: Otimize Object Pooling

---

## 📊 Relatório de Teste (Template)

```markdown
## Teste Mobile - [Data]

### Informações do Build
- Versão: 1.0
- Build Number: 1
- Data da Build: __/__/____

### Dispositivo Testado
- Modelo: 
- Android Version: 
- RAM: 
- Chipset: 

### Resultados
| Categoria | Status | Notas |
|-----------|--------|-------|
| Controles | ✅/⚠️/❌ | |
| Gameplay | ✅/⚠️/❌ | |
| UI/UX | ✅/⚠️/❌ | |
| Performance | ✅/⚠️/❌ | FPS: ___ |
| Audio | ✅/⚠️/❌ | |
| Estabilidade | ✅/⚠️/❌ | Crashes: ___ |

### Bugs Encontrados
1. [Descrever bug]
2. [Descrever bug]

### Melhorias Sugeridas
1. [Sugestão]
2. [Sugestão]

### Conclusão
- [ ] ✅ Aprovado para publicação
- [ ] ⚠️ Necessita ajustes
- [ ] ❌ Necessita refatoração
```

---

## 🔧 Ferramentas de Debug

### Ativar Modo Desenvolvedor no Android

1. Vá em **Configurações > Sobre o Telefone**
2. Toque 7x em **Número da Versão**
3. Modo desenvolvedor ativado!
4. Vá em **Configurações > Opções do Desenvolvedor**
5. Ative **Depuração USB**

### Conectar Unity Profiler

1. Conecte dispositivo via USB
2. Unity: **Window > Analysis > Profiler**
3. Dropdown: Selecione seu dispositivo Android
4. Clique em **Record**
5. Monitore CPU, GPU, Memory, etc.

### Android Logcat (Logs em Tempo Real)

1. Unity: **Window > Analysis > Android Logcat**
2. Conecte dispositivo via USB
3. Visualize logs em tempo real
4. Filtre por "Unity" para ver apenas logs do jogo

---

## ✅ Aprovação Final

Para considerar o jogo **pronto para entrega**:

- [ ] ✅ Testado em 2+ dispositivos diferentes
- [ ] ✅ FPS >= 30 em dispositivos mid-range
- [ ] ✅ Sem crashes críticos
- [ ] ✅ Todos controles funcionam corretamente
- [ ] ✅ Gameplay completo (menu → jogo → game over)
- [ ] ✅ UI legível e responsiva
- [ ] ✅ Áudio funcionando
- [ ] ✅ APK < 200MB

**Data do último teste**: __/__/____

**Testadores**: ________________
