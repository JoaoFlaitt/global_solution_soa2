# 🛰️ ORBIT GUARD AI
### Plataforma inteligente de previsão e resposta a eventos climáticos extremos
**Global Solution FIAP 2026 — Economia Espacial, IA e Sustentabilidade**

## Integrantes

Miguel Leal 553009
João Víctor Flaitt 553888
Lucas Bertolassi 553183
Lucca Calsolari 553678

---

## 1. Análise do Contexto da Global Solution

A **Economia Espacial** deixou de ser um nicho de poucos países para se tornar uma das alavancas mais estratégicas do século XXI. Constelações como Sentinel (ESA/Copernicus), Landsat (NASA), CBERS e Amazonia-1 (INPE) geram volumes massivos de **dados orbitais** — imagens multiespectrais, NDVI, temperatura de superfície, umidade do solo, focos de calor — que, se cruzados com **sensores IoT em campo**, **conectividade em regiões remotas** (LoRa, satélite de baixa órbita) e **Inteligência Artificial preditiva**, podem antecipar tragédias.

As **mudanças climáticas** intensificam enchentes (Petrópolis 2022, Rio Grande do Sul 2024), deslizamentos, queimadas (Pantanal e Amazônia) e secas históricas. O Brasil tem milhões de pessoas em áreas vulneráveis, frequentemente desassistidas por sistemas de monitoramento em tempo real. A integração entre **tecnologias espaciais e problemas terrestres** é, portanto, o caminho mais eficiente para salvar vidas.

A solução está alinhada aos **ODS da ONU**:
- **ODS 2 — Fome Zero**: previsão de secas protege a agricultura familiar.
- **ODS 8 — Trabalho Decente**: reduz prejuízos econômicos em regiões produtivas.
- **ODS 9 — Indústria, Inovação e Infraestrutura**: combina IoT, Cloud e dados orbitais.
- **ODS 11 — Cidades Sustentáveis**: protege populações urbanas em áreas de risco.
- **ODS 13 — Ação Contra a Mudança Global do Clima**: monitora e mitiga eventos extremos.

## 2. Problema escolhido

> **Falta de previsão e resposta rápida a eventos climáticos extremos (enchentes, deslizamentos, queimadas e secas) em regiões vulneráveis.**

Defesa Civil, prefeituras e populações em áreas de risco recebem alertas tarde demais — ou não recebem. Os dados existem, mas estão fragmentados em portais distintos (CEMADEN, INPE, ANA, INMET) e raramente são processados com IA em tempo real.

## 3. Solução: ORBIT GUARD AI

Plataforma SaaS em nuvem que integra:

| Camada | Tecnologia |
|---|---|
| **Aquisição** | Satélites (CBERS-4A, Amazonia-1, Sentinel-2) + Sensores IoT (pluviômetros, inclinômetros, nível de rio, qualidade do ar) |
| **Processamento** | IA preditiva (ML + Visão Computacional sobre imagens NDVI/térmicas) |
| **Núcleo (este repositório)** | **API .NET 8 Core** — orquestra dados, executa o motor preditivo, expõe REST autenticada |
| **Apresentação** | Dashboard Web (Defesa Civil) + App Mobile (Cidadão) — consomem a API |
| **Distribuição** | Notificação multicanal (Push, SMS, E-mail) |

### Objetivo
Prever eventos climáticos extremos com antecedência, emitir **alertas automáticos** para a população e apoiar **gestores públicos** na tomada de decisão.

## 4. Arquitetura

```
OrbitGuardAI/
├── Domain/             ← POO pura: entidades, interfaces, enums
│   ├── Entities/       Dispositivo (abstrata), Satélite, SensorIoT (herança/polimorfismo),
│   │                   Coordenada (VO), Alerta, AreaRisco, LeituraTelemetrica, Usuario
│   ├── Interfaces/     IRepositorio<T>, IPreditorClimaticoService, IAlertaService...
│   └── Enums/          TipoEventoClimatico, NivelSeveridade, StatusDispositivo...
├── Application/        ← Casos de uso
│   ├── DTOs/           VO/DTO records (LoginDTO, AlertaDTO, PrevisaoDTO...)
│   ├── Services/       PreditorClimaticoService, AlertaService, NotificacaoService
│   └── Exceptions/     OrbitGuardException + especializações
├── Infrastructure/     ← Implementações
│   ├── Data/           AppDbContext (EF Core) + Repositórios
│   ├── Auth/           TokenService (JWT + SHA-256)
│   └── External/       SatelliteDataGateway (NASA/INPE/Copernicus)
├── API/
│   ├── Controllers/    Auth, AreasRisco, Alertas, Telemetria, Dispositivos
│   └── Middleware/     ExceptionMiddleware
├── docs/               diagrama-fluxo.md (Mermaid)
├── Program.cs          DI, CORS, JWT, Swagger, Seed
└── appsettings.json
```

Veja `docs/diagrama-fluxo.md` para os **diagramas Mermaid** de fluxo de dados, sequência de geração de alerta e camadas.

## 5. Como executar

### Pré-requisitos
- .NET 8 SDK

### Passos
```bash
cd OrbitGuardAI
dotnet restore
dotnet run
```
A API sobe em `http://localhost:5080` e o **Swagger UI** abre na raiz (`/`).

### Credenciais de seed
- E-mail: `admin@orbitguard.ai`
- Senha: `orbit2026`

### Roteiro de teste sugerido (evidência de execução)
1. `POST /api/Auth/login` → recebe o JWT.
2. Clique em **Authorize** no Swagger e cole `Bearer <token>`.
3. `GET /api/AreasRisco` → 3 áreas pré-cadastradas (Petrópolis/RJ, Lajeado/RS, Lábrea/AM).
4. `POST /api/Alertas/avaliar/{areaId}` → roda IA preditiva e gera o alerta.
5. `GET /api/Alertas` → consome o que o app/dashboard mostraria.
6. `GET /api/Dispositivos/saude` → health-check polimórfico da frota.

## 6. Mapeamento dos requisitos da GS

| Requisito | Onde está |
|---|---|
| Modelagem de Domínio & POO | `Domain/Entities` — classes públicas, privadas, estáticas (`Dispositivo.TotalInstanciados`), herança (`Satelite : Dispositivo`, `SensorIoT : Dispositivo`) e polimorfismo (`Coletar()` sobrescrito) |
| Abstração e Interfaces | `Dispositivo` abstrata; `IPreditorClimaticoService`, `IAlertaService`, `IRepositorio<T>` com **DI** registrada em `Program.cs` |
| Lógica de Fluxo + Métodos + Datas | `PreditorClimaticoService.PreverAsync` (scoring com `Math.Clamp`/`switch`), `Alerta.TempoRestante()`, manipulação `DateTime` em históricos |
| Tratamento de Exceções | `OrbitGuardException` + especializações + `ExceptionMiddleware` global (sistemas críticos não podem cair) |
| VO / DTO | `Coordenada` (VO imutável com Haversine), `records` em `Application/DTOs` |
| Conexão com Banco | EF Core + `AppDbContext` + repositórios (InMemory por padrão; trocar para Sqlite/SQL Server via `UseSqlite`/`UseSqlServer`) |
| WebService / API | API REST .NET 8 com Controllers |
| Autenticação / Autorização | JWT Bearer + `[Authorize(Roles = "Gestor,Admin")]` |
| CORS | Política `OrbitGuardCors` em `Program.cs` |
| Swagger | `Swashbuckle` com auth Bearer; UI na raiz |
| Organização | Estrutura por camadas (Domain / Application / Infrastructure / API) |
| Diagrama de fluxos | `docs/diagrama-fluxo.md` |

## 7. Próximos passos (roadmap)
- Substituir o gateway simulado por integração real com Copernicus Open Hub e CEMADEN.
- Treinar modelo ONNX com histórico CEMADEN e plugar no `IPreditorClimaticoService`.
- App Flutter consumindo `/api/Alertas` com geofencing.
- Deploy em Azure App Service + Cosmos DB (atualmente já compatível).