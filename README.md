# Quiz Game

Juego de trivia con 5 niveles de dificultad y acumulación de premio, temporizador en tiempo real y retiro voluntario. Backend en **.NET 10** con `Clean Architecture + DDD táctico con Vertical Slices`, Frontend en **Angular 20** `zoneless` con PrimeNG, comunicación híbrida REST + **SignalR**, y persistencia en **SQL Server 2022** con el `Patrón Transactional Outbox`.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/es-es/)
[![Angular](https://img.shields.io/badge/Angular-20-DD0031?logo=angular&logoColor=white)](https://angular.dev/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/es-co/sql-server/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://docs.docker.com/compose/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Descripción del reto

Es un juego de preguntas y respuestas con opción múltiple (4 opciones, 1 correcta)
organizado en niveles de dificultad. El jugador comienza en el nivel más fácil; cada acierto acumula premio y avanza de nivel; el primer fallo termina la partida perdiendo lo acumulado; el jugador puede retirarse voluntariamente en cualquier momento entre rondas conservando el premio ganado hasta ese punto; y cada pregunta tiene un límite de tiempo, transcurrido el cual la partida finaliza forzosamente.

| Funcionalidad | Estado | Dónde |
| --- | --- | --- |
| Configurar el juego | ✅ | `GameOptions` (`appsettings.json`), `ConfigureGameQuery`, `GET /api/v1/games/settings` |
| Iniciar el juego | ✅ | `StartGameCommand`, `Game.Create`, `POST /api/v1/games` |
| Responder a la pregunta | ✅ | `AnswerQuestionCommand`, `GameHub.SubmitAnswerAsync` |
| Aumentar de nivel | ✅ | `Game.Answer`, `RoundAdvancedDomainEvent` |
| Acumular premio | ✅ | `Game.Answer`, `PrizeAccumulatedDomainEvent`, `TieredPrizeStrategy` |
| Finalizar voluntario | ✅ | `WithdrawGameCommand`, `GameHub.WithdrawAsync` |
| Ganador de ronda final | ✅ | `Game.Answer`, `GameWonDomainEvent` |
| Fin del juego forzado | ✅ | `ForceEndGameCommand`, `GameTimeoutService` |
| Gestión de categorías y preguntas | ✅ | `CategoryEndpoints`, `QuestionEndpoints`, panel `/admin` |

## Stack tecnológico

### Backend

| Tecnología | Versión | Propósito |
| --- | --- | --- |
| .NET / ASP.NET Core | 10.0 | Runtime y Minimal API |
| Entity Framework Core | 10.0.0 | ORM, migraciones, `IDbContextFactory` |
| SignalR | 10.0.0 | Canal en tiempo real (`GameHub`) |
| FluentValidation | 11.10.0 | Validación de comandos en el pipeline |
| Microsoft.Extensions.Resilience (Polly) | 9.10.0 | Reintentos y Circuit Breaker del Outbox |
| Scalar.AspNetCore | 1.2.64 | UI de documentación sobre OpenAPI nativo |
| xUnit + Shouldly + NSubstitute | 2.8.1 / 4.2.1 / 5.1.0 | Pruebas unitarias |
| Testcontainers.MsSql | 4.15.0 | SQL Server real en contenedor para pruebas de integración |
| SQL Server 2022 | `mcr.microsoft.com/mssql/server:2022-latest` | Persistencia |

### Frontend

| Tecnología | Versión | Propósito |
| --- | --- | --- |
| Angular | 20.3.30 | Framework SPA, `provideZonelessChangeDetection` |
| @ngrx/signals | 20.1.0 | `signalStore` para estado de features (`GameStore`, `CategoryStore`) |
| PrimeNG + @primeuix/themes | 20.2.0 / ^1.1.1 | Componentes UI, preset Aura |
| @microsoft/signalr | ^8.0.7 | Cliente WebSocket hacia `GameHub` |
| RxJS | ~7.8.0 | Streams de eventos del gateway en tiempo real |
| Vitest + @vitest/coverage-v8 | ^3.2.7 | Pruebas unitarias y cobertura |
| ESLint + angular-eslint | ^9.19.0 / ^20.7.0 | Linting |
| Node.js | ≥ 22.0.0 | Runtime de build (`engines` en `package.json`) |

## Arquitectura

### ¿Por qué Clean Architecture + DDD táctico con Vertical Slices?

Las reglas del juego imponen restricciones severas: cuatro opciones con una única respuesta correcta por pregunta, categorías habilitadas únicamente tras alcanzar cinco preguntas activas, estados irreversibles de partida y penalización total del acumulado al fallar. Para evitar que estas reglas se omitan en casos límite, las modelamos dentro de un agregado rico de DDD en lugar de utilizar servicios anémicos o validaciones dispersas en controladores. Gracias a Clean Architecture, el dominio permanece agnóstico a tecnologías como EF Core o SignalR. Además, la adopción de Vertical Slices (`Features/...`) permite acoplar código por caso de uso en lugar de dispersarlo en capas horizontales rígidas (`Controllers/`, `Services/`, `Repositories/`), mejorando drásticamente la mantenibilidad.

### Regla de dependencia

```text
┌─────────────────────────────────────────────────────────────┐
│                       QuizGame.Api                          │
│          (Minimal APIs, GameHub SignalR, Program.cs)        │
└───────────────┬─────────────────────────────┬───────────────┘
                │ depende de                  │ depende de
                ▼                             ▼
┌──────────────────────────────┐┌─────────────────────────────┐
│   QuizGame.Infrastructure    ││    QuizGame.Application     │
│ (EF Core, Outbox, Polly, DB) ││ (Vertical Slices, Pipeline) │
└───────────────┬──────────────┘└──────────────┬──────────────┘
                │                              │
                │ implementa / depende de      │ depende de
                └───────────────┬──────────────┘
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                      QuizGame.Domain                        │
│          (Agregados, Value Objects, Reglas puras)           │
│                                                             │
│          ───► NO DEPENDE DE NINGUNA OTRA CAPA ◄───          │
└─────────────────────────────────────────────────────────────┘
```

`QuizGame.Domain` no referencia ningún paquete de Entity Framework, ASP.NET Core ni SignalR. `QuizGame.Application` solo conoce abstracciones (`IGameRepository`, `IUnitOfWork`, `IGameNotifier`); sus implementaciones concretas viven en `Infrastructure` y `Api`.

## Prerrequisitos

### Local

| Herramienta | Versión mínima | Verificación |
| --- | --- | --- |
| .NET SDK | 10.0 | `dotnet --version` |
| Node.js | 22.x | `node --version` |
| SQL Server LocalDB | incluido con Visual Studio o [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) | `sqllocaldb info` |
| dotnet-ef | 10.x (herramienta global) | `dotnet ef --version` |
| Docker Desktop | Versión reciente | `docker --version` |

### Docker Compose

| Herramienta | Versión mínima | Verificación |
| --- | --- | --- |
| Docker Desktop con Compose V2 | Versión reciente | `docker compose version` |

## Ejecución — Local

```powershell
git clone <repo-url> && cd quiz-game-fullstack_ciel

# Base de datos (LocalDB)
cd backend

dotnet tool install --global dotnet-ef
dotnet ef database update --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api

# API (deja esta terminal abierta)
dotnet run --project src/QuizGame.Api --launch-profile Local

# Frontend (en otra terminal)
cd ../frontend
npm ci
npm start
```

**URLs**:

| Servicio | URL |
| --- | --- |
| Frontend (Angular dev server) | <http://localhost:4200> |
| API | <http://localhost:5121> |
| Documentación OpenAPI (Scalar) | <http://localhost:5121/scalar/v1> |
| Health checks | <http://localhost:5121/health/live>, <http://localhost:5121/health/ready> |

Alternativa sin `dotnet-ef`: ejecutar los scripts de [`scripts/database/`](scripts/database/) en orden ascendente con `sqlcmd` contra `(localdb)\MSSQLLocalDB`.

## Ejecución — Docker Compose

```powershell
# Preparar el secreto (una sola vez)
cp .env.example .env      # Ajustar valor MSSQL_SA_PASSWORD

# Construir y levantar
docker compose up --build -d

# Ver estado y logs
docker compose ps
docker compose logs -f quiz-game-backend
```

Comandos de operación:

```powershell
# Detener
docker compose stop

# Detener y eliminar contenedores
docker compose down

# Reset total, incluidos los datos
docker compose down -v

# Reconstruir solo un servicio
docker compose up --build -d quiz-game-backend
```

| Servicio | Imagen | Puerto host | Puerto contenedor |
| --- | --- | --- | --- |
| `quiz-game-frontend` | build local (`frontend/Dockerfile`, Nginx) | `8080` | `80` |
| `quiz-game-backend` | build local (`backend/Dockerfile`, ASP.NET Core) | `5000` | `8080` |
| `quiz-game-db` | `mcr.microsoft.com/mssql/server:2022-latest` | `1433` | `1433` |

**URLs**:

Frontend <http://localhost:8080>, API <http://localhost:5000>, Documentación <http://localhost:5000/scalar/v1>. `quiz-game-backend` espera a que `quiz-game-db` pase su healthcheck (`condition: service_healthy`) y aplica las migraciones automáticamente al iniciar (solo en el perfil `Docker`, ver `Program.cs`).

## Frontend Environments

En el archivo `environment.ts` y `proxy.conf.json` tener en cuenta los puertos de **apiBaseUrl** (`/api`) y **hubUrl** (`/hubs`) para hacer el cambio, según el entorno en ejecución.

## Configuración

```text
backend/src/QuizGame.Api/
├── appsettings.json            → configuración común (Resilience, Logging), sin cadena de conexión
├── appsettings.Local.json      → modo local (LocalDB, Integrated Security)
└── appsettings.Docker.json     → modo docker-compose (SQL Server en contenedor, marcador de contraseña)
```

### ¿Por qué el perfil Local no lleva usuario ni contraseña?

LocalDB se autentica con el usuario de Windows del proceso: **no admite `User Id`/`Password`**. Por eso el perfil Local usa `Integrated Security=true` y omite esos dos parámetros — es una desviación deliberada, no un olvido. Consecuencia práctica: **el modo Local no necesita ninguna variable de entorno de contraseña**.

### Gestión del secreto en modo Docker

La contraseña **no está en ningún archivo versionado**. La cadena `DefaultConnection` sí vive
completa en `appsettings.Docker.json`, pero con un marcador en lugar del secreto:

```json
"DefaultConnection": "Server=quiz-game-db,1433;Database=QuizGameDb;User Id=sa;Password=${MSSQL_SA_PASSWORD};..."
```

Una sola variable (`MSSQL_SA_PASSWORD`), definida en `.env`, consumida por los dos servicios.
Cambiarla requiere editar un único archivo.

## Base de datos

Los scripts en [`scripts/database/`](scripts/database/) son **generados** a partir de las migraciones de EF Core (`backend/src/QuizGame.Infrastructure/Persistence/Migrations/`) **no se editan a mano**. El número mayor de versión avanza con cada migración nueva (`1.0 → 2.0 → … → 5.0`); un número menor (`x.1`) quedaría reservado para una corrección manual aplicada sobre un script ya entregado.

| Script | Migración que lo genera | Contenido |
| --- | --- | --- |
| `1.0.quiz-game-schema-script.sql` | `InitialCreate` | Tablas `Categories`, `Questions`, `Answers`, `Games`, `Rounds` |
| `2.0.quiz-game-seed-data-script.sql` | `SeedInitialData` | `INSERT` de las 3 categorías y 18 preguntas sembradas |
| `3.0.quiz-game-stored-procedures-script.sql` | `AddStoredProcedures` | `sp_GetRandomQuestionByCategory_v1`, `sp_GetGameSummary_v1` |
| `4.0.quiz-game-idempotency-outbox-script.sql` | `AddIdempotencyAndOutbox` | Tablas `IdempotencyRecords`, `OutboxMessages` |
| `5.0.quiz-game-resilience-columns-script.sql` | `AddResilienceColumns` | Columnas de soporte para reintentos/circuito del Outbox |

Cada script es idempotente (`--idempotent`): ejecutarlo más de una vez sobre la misma base es seguro.

### Cómo regenerarlos

No existe un script auxiliar versionado para esto (se ejecuta el comando de EF Core directamente,
sin envoltorio, para no atarlo a PowerShell ni Bash):

```powershell
cd backend

dotnet ef migrations script 0 InitialCreate --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api --idempotent --output ../scripts/database/1.0.quiz-game-schema-script.sql

dotnet ef migrations script InitialCreate SeedInitialData --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api --idempotent --output ../scripts/database/2.0.quiz-game-seed-data-script.sql

dotnet ef migrations script SeedInitialData AddStoredProcedures --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api --idempotent --output ../scripts/database/3.0.quiz-game-stored-procedures-script.sql

dotnet ef migrations script AddStoredProcedures AddIdempotencyAndOutbox --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api --idempotent --output ../scripts/database/4.0.quiz-game-idempotency-outbox-script.sql

dotnet ef migrations script AddIdempotencyAndOutbox AddResilienceColumns --project src/QuizGame.Infrastructure --startup-project src/QuizGame.Api --idempotent --output ../scripts/database/5.0.quiz-game-resilience-columns-script.sql
```

### Contenido del seed

3 categorías (`Software Development`, `Software Architecture`, `Data & AI Engineering`), cada una con dificultad 1 a 3, un premio base, y 6 preguntas activas (18 en total) con 4 respuestas cada una.

## Pruebas

### Pruebas Backend

```powershell
cd backend
dotnet test                                           # las 4 suites (unitarias + integración)
dotnet test tests/QuizGame.Domain.UnitTests           # solo dominio
dotnet test tests/QuizGame.Application.UnitTests      # solo aplicación
dotnet test tests/QuizGame.Api.IntegrationTests       # requiere Docker (Testcontainers.MsSql)
dotnet test --collect:"XPlat Code Coverage"           # con cobertura (coverlet)
```

### Pruebas Frontend

```powershell
cd frontend
npm test                       # Vitest en watch
npm test -- --watch=false      # una sola corrida (CI)
npm run test:coverage          # con cobertura (@vitest/coverage-v8)
npm run lint                   # ESLint + angular-eslint
```

### Qué se prueba

| Capa | Qué cubre |
| --- | --- |
| `QuizGame.Domain.UnitTests` | Invariantes de los agregados (`Game`, `Question`, `Category`), Value Objects, `GameStateMachine`, `GameRules`, igualdad de entidades |
| `QuizGame.Application.UnitTests` | Handlers de comandos/consultas, comportamientos del pipeline, estrategias de premio y selección de preguntas |
| `QuizGame.Api.IntegrationTests` | Endpoints REST de punta a punta contra un SQL Server real (`Testcontainers.MsSql`) vía `WebApplicationFactory` |
| Frontend (Vitest) | Guards de rutas, mappers DTO↔dominio, `GameStore` (`@ngrx/signals`), componentes de presentación (`CountdownBar`, `PrizeDisplay`, `QuestionCard`), deduplicador de eventos SignalR |

## API y tiempo real

### Endpoints REST (`/api/v1`)

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/games/settings` | Configuración vigente del juego (rondas, tiempo por pregunta, premios) |
| `POST` | `/games` | Inicia una partida (`{ playerName }`) |
| `GET` | `/games/{id}` | Estado actual de una partida |
| `GET` | `/games/{id}/summary` | Resumen final (premio, estado, jugador) |
| `GET` | `/categories?onlyActive=` | Lista categorías |
| `GET` | `/categories/{id}` | Detalle de una categoría |
| `POST` | `/categories` | Crea una categoría |
| `PUT` | `/categories/{id}` | Actualiza una categoría |
| `DELETE` | `/categories/{id}` | Desactiva una categoría |
| `GET` | `/categories/{categoryId}/questions` | Lista preguntas de una categoría |
| `GET` | `/questions/{id}` | Detalle de una pregunta |
| `POST` | `/questions` | Crea una pregunta (con sus 4 respuestas) |
| `PUT` | `/questions/{id}` | Actualiza una pregunta |
| `DELETE` | `/questions/{id}` | Desactiva una pregunta |
| `GET` | `/health/live` / `/health/ready` | Liveness / readiness (base de datos + circuito del Outbox) |

Todas las rutas comparten la política de *rate limiting* `api` (100 solicitudes/minuto por ventana fija, `429 Too Many Requests` al superarla).

### Contrato de eventos SignalR (`/hubs/game`)

| Evento | Payload | Cuándo se emite |
| --- | --- | --- |
| `GameStartedAsync` | `{ gameId, playerName, totalRounds, startedOnUtc }` | Al confirmarse `StartGameCommand` |
| `RoundStartedAsync` | `{ gameId, roundNumber, questionId, questionText, answers[], prizeAtStake, deadlineUtc }` | Al asignarse una nueva pregunta |
| `AnswerEvaluatedAsync` | `{ gameId, roundNumber, isCorrect, correctAnswerId, accumulatedPrize, status }` | Tras evaluar la respuesta del jugador |
| `RoundAdvancedAsync` | `{ gameId, previousRoundNumber, currentRoundNumber, accumulatedPrize }` | Al avanzar de ronda tras un acierto no final |
| `PrizeAccumulatedAsync` | `{ gameId, roundNumber, prizeWon, accumulatedPrize }` | Junto con cada acierto, antes o después de avanzar ronda |
| `GameEndedAsync` | `{ gameId, playerName, status, finalPrize, endedOnUtc }` | Victoria, derrota, retiro o fin forzado |
| `TimeRemainingAsync` | `{ gameId, roundNumber, secondsRemaining }` | Cada segundo, mientras haya una ronda abierta (`GameTimeoutService`) |

Métodos invocables por el cliente: `JoinGameAsync(gameId)`, `SubmitAnswerAsync(request)`,
`WithdrawAsync(request)`, `LeaveGameAsync(gameId)`.

### Documentación interactiva

OpenAPI nativo de .NET 10 servido con **Scalar**: `http://localhost:5121/scalar/v1` (Local) o `http://localhost:5000/scalar/v1` (Docker).

## Estructura del repositorio

```text
quiz-game-fullstack_ciel/
├── backend/                    → API .NET 10, Clean Architecture
│   ├── src/
│   └── tests/
├── frontend/                   → SPA Angular 20 zoneless
│   ├── src/
│   └── public/
├── docs/                       → diagramas de arquitectura
│   └── diagrams/               → archivos PNG de cada diagrama
├── scripts/database/           → scripts .sql generados desde las migraciones EF Core
├── docker-compose.yml          → orquestación de los 3 contenedores (db, backend, frontend)
├── .env.example                → plantilla de variables de entorno para Docker Compose
└── README.md
```
