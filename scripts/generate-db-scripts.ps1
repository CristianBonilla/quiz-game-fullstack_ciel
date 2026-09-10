#Requires -Version 7.0
<#
.SYNOPSIS
    Regenerates the idempotent SQL deliverables in scripts/database/ from the EF Core migrations.
#>

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$backendDir = Join-Path $repoRoot "backend"
$outputDir = Join-Path $repoRoot "scripts/database"
$infraProject = "src/QuizGame.Infrastructure"
$startupProject = "src/QuizGame.Api"

$steps = @(
    @{ From = "0"; To = "InitialCreate"; Output = "1.0.quiz-game-schema-script.sql" },
    @{ From = "InitialCreate"; To = "SeedInitialData"; Output = "2.0.quiz-game-seed-data-script.sql" },
    @{ From = "SeedInitialData"; To = "AddStoredProcedures"; Output = "3.0.quiz-game-stored-procedures-script.sql" },
    @{ From = "AddStoredProcedures"; To = "AddIdempotencyAndOutbox"; Output = "4.0.quiz-game-idempotency-outbox-script.sql" },
    @{ From = "AddIdempotencyAndOutbox"; To = "AddResilienceColumns"; Output = "5.0.quiz-game-resilience-columns-script.sql" }
)

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

Push-Location $backendDir
try
{
    foreach ($step in $steps)
    {
        $outputPath = "../scripts/database/$($step.Output)"
        Write-Host "Generating $($step.Output) ($($step.From) -> $($step.To))..." -ForegroundColor Cyan

        dotnet ef migrations script $step.From $step.To `
            --project $infraProject `
            --startup-project $startupProject `
            --idempotent `
            --output $outputPath

        if ($LASTEXITCODE -ne 0)
        {
            Write-Error "Failed to generate $($step.Output)"
            exit 1
        }
    }
}
finally
{
    Pop-Location
}

Write-Host "All database scripts regenerated successfully." -ForegroundColor Green
exit 0
