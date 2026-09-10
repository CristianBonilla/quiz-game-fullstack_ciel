# Database scripts

These `.sql` files are **generated** from the EF Core migrations in
`backend/src/QuizGame.Infrastructure/Persistence/Migrations/`. Do not edit them by hand — regenerate
them with `scripts/generate-db-scripts.ps1` (Windows) or `scripts/generate-db-scripts.sh` (Linux/macOS).

Each script is idempotent (`--idempotent`): running it more than once against the same database is safe.

| Script | Migrations covered | Content |
| --- | --- | --- |
| `1.0.quiz-game-schema-script.sql` | `InitialCreate` | Tables: `Categories`, `Questions`, `Answers`, `Games`, `Rounds` |
| `2.0.quiz-game-seed-data-script.sql` | `SeedInitialData` | `INSERT` statements for the seed categories/questions/answers |
| `3.0.quiz-game-stored-procedures-script.sql` | `AddStoredProcedures` | `sp_GetRandomQuestionByCategory_v1`, `sp_GetGameSummary_v1` |
| `4.0.quiz-game-idempotency-outbox-script.sql` | `AddIdempotencyAndOutbox` | Tables: `IdempotencyRecords`, `OutboxMessages` |

The major version number advances with each new migration (`1.0 → 2.0 → 3.0 → 4.0`); the minor
number (`x.1`) is reserved for manual corrections applied on top of an already-delivered script.
