#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
BACKEND_DIR="$REPO_ROOT/backend"
INFRA_PROJECT="src/QuizGame.Infrastructure"
STARTUP_PROJECT="src/QuizGame.Api"

mkdir -p "$REPO_ROOT/scripts/database"

declare -a FROM=("0" "InitialCreate" "SeedInitialData" "AddStoredProcedures" "AddIdempotencyAndOutbox")
declare -a TO=("InitialCreate" "SeedInitialData" "AddStoredProcedures" "AddIdempotencyAndOutbox" "AddResilienceColumns")
declare -a OUT=(
    "1.0.quiz-game-schema-script.sql"
    "2.0.quiz-game-seed-data-script.sql"
    "3.0.quiz-game-stored-procedures-script.sql"
    "4.0.quiz-game-idempotency-outbox-script.sql"
    "5.0.quiz-game-resilience-columns-script.sql"
)

cd "$BACKEND_DIR"

for i in "${!FROM[@]}"; do
    output="../scripts/database/${OUT[$i]}"
    echo "Generating ${OUT[$i]} (${FROM[$i]} -> ${TO[$i]})..."

    if ! dotnet ef migrations script "${FROM[$i]}" "${TO[$i]}" \
        --project "$INFRA_PROJECT" \
        --startup-project "$STARTUP_PROJECT" \
        --idempotent \
        --output "$output"; then
        echo "Failed to generate ${OUT[$i]}" >&2
        exit 1
    fi
done

echo "All database scripts regenerated successfully."
exit 0
