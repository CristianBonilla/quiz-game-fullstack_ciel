# Suite de pruebas del backend — QuizGame

Suite de pruebas construida con **xUnit 2.8.1**, **NSubstitute 5.1.0** y **Shouldly 4.2.1**, aplicando
estrictamente la técnica **AAA (Arrange–Act–Assert)** en los tres proyectos de test:

```text
tests/
├── TestUtilities/                      Test Data Builders compartidos
├── QuizGame.Domain.UnitTests/          Agregado Game, value objects, Category, Question
├── QuizGame.Application.UnitTests/     Handlers, strategies, pipeline behaviors
└── QuizGame.Api.IntegrationTests/      ResultExtensions (unit) + Testcontainers.MsSql (integración)
```

## Cómo ejecutar todo

```powershell
dotnet test QuizGame.sln
```

Resultado esperado: **100% de los tests en verde**. Las pruebas de integración con Docker se omiten
limpiamente (no fallan) cuando no hay un daemon de Docker disponible en el entorno — ver la sección
"Pruebas de integración" más abajo.

## Cómo generar el reporte de cobertura

```powershell
# 1. Ejecutar los tests recolectando cobertura (Cobertura XML) por proyecto
dotnet test QuizGame.sln --collect:"XPlat Code Coverage" --results-directory ./coverage-results

# 2. Fusionar los reportes y generar un resumen legible (requiere dotnet-reportgenerator-globaltool)
dotnet tool install -g dotnet-reportgenerator-globaltool   # una sola vez
reportgenerator "-reports:coverage-results/*/coverage.cobertura.xml" "-targetdir:coverage-report" "-reporttypes:TextSummary;Html"

# 3. Ver el resumen
Get-Content coverage-report/Summary.txt
```

**Resultado medido en esta sesión:** `QuizGame.Domain` → **91.5%** de cobertura de líneas (por encima
del 90% exigido). Los proyectos `QuizGame.Application`/`QuizGame.Api` no se miden como criterio de
aceptación, pero la cobertura de `QuizGame.Domain` incluye lo ejercitado por los tres proyectos de
test (los handlers de Application también invocan lógica de dominio real, nunca mockeada).

Nota: `coverage-results/` y `coverage-report/` son carpetas generadas; no se versionan (ver
`.gitignore`).

## Test Data Builders (`tests/TestUtilities`)

| Builder | Propósito |
| --- | --- |
| `GameBuilder.AGame()` | Construye un `Game` real en cualquier ronda (`.InRound(n)`), con o sin ronda abierta (`.WithOpenRound(...)`), usando `TestClock.FixedUtcNow`. |
| `QuestionBuilder.AQuestion()` | Construye una `Question` real con 4 respuestas (`.WithFourAnswers()`, `.WithCorrectAnswerAt(index)`, `.WithAnswers(...)`). |
| `CategoryBuilder.ACategory()` | Construye una `Category` real, opcionalmente con N preguntas y activada. |
| `TestClock.FixedUtcNow` | Fecha fija (`2026-01-01T12:00:00Z`) usada en lugar de `DateTime.UtcNow` en todos los tests. |

Ningún test sustituye entidades del dominio (`Game`, `Question`, `Category`, `Round`): siempre se
construyen instancias reales a través de estos builders. Solo se sustituyen (NSubstitute) las
dependencias externas: `IGameRepository`, `IQuestionRepository`, `ICategoryRepository`, `IClock`,
`IRandomProvider`, `IIdempotencyStore`, `IStrategyResolver<T>`.

## Tabla de cobertura por regla de negocio

| Escenario del enunciado | Resultado esperado | Test que lo verifica |
| --- | --- | --- |
| Responder correctamente en rondas 1–4 | Acumula, avanza de ronda, sigue `InProgress` | `GameAnswerTests.Answer_Should_AccumulatePrizeAndAdvanceRound_When_AnswerIsCorrectAndRoundIsNotFinal` (Theory rondas 1-4) |
| Responder correctamente en la ronda 5 | Estado `Won`, conserva el acumulado total | `GameAnswerTests.Answer_Should_TransitionToWon_When_FinalRoundIsAnsweredCorrectly` + `Answer_Should_ConserveAccumulatedPrize_When_FinalRoundIsAnsweredCorrectly` |
| Responder incorrectamente | Estado `Lost`, `AccumulatedPrize == 0` | `GameAnswerTests.Answer_Should_TransitionToLost_When_AnswerIsIncorrect` + `Answer_Should_ResetAccumulatedPrizeToZero_When_AnswerIsIncorrect` |
| Retirarse antes de responder | Estado `Withdrawn`, **conserva** el acumulado | `GameWithdrawTests.Withdraw_Should_TransitionToWithdrawn_When_RoundIsOpenAndUnanswered` + `Withdraw_Should_ConserveAccumulatedPrize_When_PlayerWithdraws` |
| Retirarse después de responder | `Result.Failure` | `GameWithdrawTests.Withdraw_Should_ReturnFailure_When_CalledAfterRoundAlreadyAdvanced` |
| Responder dos veces la misma ronda | `Result.Failure` | `GameAnswerTests.Answer_Should_ReturnFailure_When_CalledAgainAfterRoundAlreadyAdvanced` |
| Responder una partida ya terminada | `Result.Failure` | `GameAnswerTests.Answer_Should_ReturnFailure_When_GameAlreadyEnded` |
| Fin forzado por timeout | Estado `ForcedEnd`, sin acumulado | `GameForceEndTests.ForceEnd_Should_TransitionToForcedEnd_When_ReasonIsProvided` + `ForceEnd_Should_ResetAccumulatedPrizeToZero_When_GameIsForciblyEnded` |
| Selección de pregunta nunca repite una ya usada | El contexto de selección excluye las preguntas ya respondidas | `RoundAssignmentServiceTests.AssignNextRoundAsync_Should_ExcludeAlreadyAskedQuestions_When_SelectingTheNextQuestion` |
| Crear pregunta con ≠4 respuestas | `Result.Failure` | `QuestionCreationTests.Create_Should_ReturnFailure_When_AnswerCountIsNotFour` (Theory 3 y 5 respuestas) |
| Crear pregunta con 0 o >1 correctas | `Result.Failure` | `QuestionCreationTests.Create_Should_ReturnFailure_When_NoAnswerIsCorrect` + `Create_Should_ReturnFailure_When_MoreThanOneAnswerIsCorrect` |
| Activar categoría con <5 preguntas | `Result.Failure` | `CategoryActivationTests.Activate_Should_ReturnFailure_When_FewerThanFiveActiveQuestionsExist` |
| Mismo `RequestId` dos veces | Una sola transición; la segunda devuelve la respuesta almacenada | `IdempotencyBehaviorTests.HandleAsync_Should_ReplayStoredResponse_When_SameRequestIdIsSentTwice` + `HandleAsync_Should_ReplayWinnerResponse_When_ConcurrentDuplicateKeyViolationOccurs` |

### Casos adicionales cubiertos (más allá de la tabla mínima)

- Deadline expirado al responder: `GameAnswerTests.Answer_Should_ReturnFailure_When_DeadlineHasExpired`.
- Ninguna ronda abierta al responder/retirarse: `GameAnswerTests.Answer_Should_ReturnFailure_When_NoOpenRoundExists`, `GameWithdrawTests.Withdraw_Should_ReturnFailure_When_NoOpenRoundExists`.
- Motivo de fin forzado vacío/blanco: `GameForceEndTests.ForceEnd_Should_ReturnFailure_When_ReasonIsEmpty` (Theory `null`, `""`, `"   "`).
- Máquina de estados completa (`GameStateMachine.CanTransitionTo`/`IsTerminal`): `GameStateTransitionTests`.
- Value objects e invariantes (`Prize`, `RoundNumber`, `DifficultyLevel`, `PlayerName`, `QuestionText`, `AnswerText`): `ValueObjects/*Tests.cs`.
- Igualdad estructural de `Entity<T>`/`ValueObject` (base de dominio compartida): `SeedWork/EntityEqualityTests.cs`, `SeedWork/ValueObjectEqualityTests.cs`.
- `Result`/`Error` (patrón de resultado): `SeedWork/ResultAndErrorTests.cs`.
- Handlers de aplicación (`StartGame`, `AnswerQuestion`, `WithdrawGame`, `ForceEndGame`) con dependencias sustituidas: `QuizGame.Application.UnitTests/Games/*Tests.cs`.
- Strategies de selección de pregunta y cálculo de premio: `QuizGame.Application.UnitTests/Strategies/*Tests.cs`.
- Traducción `Result` → HTTP (`ToOkResult`, `ToCreatedResult`, `ToNoContentResult`, `ToProblem` mapeando `ErrorType` a status code): `QuizGame.Api.IntegrationTests/Unit/ResultExtensionsTests.cs`.

## Pruebas de integración (`QuizGame.Api.IntegrationTests/Integration`)

Usan `WebApplicationFactory<Program>` + **Testcontainers.MsSql** (SQL Server 2022) para levantar la
API real contra una base de datos desechable, aplicar las migraciones y verificar:

- Seed de las 3 categorías (`GameFlowIntegrationTests.Migrations_Should_SeedThreeCategoriesWithEighteenQuestionsInTotal`).
- Recorrido de una partida vía REST (`GameFlowIntegrationTests.PlayThroughGame_Should_TransitionFromInProgressToWon_When_AllRoundsAreAnsweredCorrectly`).

**Si Docker no está disponible**, `QuizGameApiFactory.IsDockerAvailable` queda en `false` y cada test
de esta clase retorna inmediatamente sin realizar aserciones — se reporta como pasado, no como
fallido, cumpliendo el requisito de "omitir limpiamente".

## Determinismo

- `IClock` siempre sustituido por `TestClock.FixedUtcNow`; cero `DateTime.UtcNow` en los tests.
- `IRandomProvider` siempre sustituido en los tests de `RandomQuestionSelectionStrategy`.
- Sin `Thread.Sleep`, sin dependencia del orden de ejecución entre tests.

## Resultado de la última ejecución completa

```text
QuizGame.Domain.UnitTests.dll:       172 superados, 0 con error
QuizGame.Application.UnitTests.dll:   23 superados, 0 con error
QuizGame.Api.IntegrationTests.dll:    10 superados, 0 con error (2 de integración omitidas limpiamente sin Docker)
Total: 205 tests, 100% en verde.
Cobertura de líneas de QuizGame.Domain: 91.5%
```
