import { GameSummary } from '@domain/models/game-summary';
import { Round } from '@domain/models/round';

export function aRound(overrides: Partial<Round> = {}): Round {
  return {
    number: 1,
    questionId: 'question-1',
    outcome: 'Correct',
    prizeAtStake: 100,
    selectedAnswerId: 'answer-1',
    answeredAtUtc: new Date('2026-01-01T00:00:00.000Z'),
    ...overrides
  };
}

export function aGameSummary(overrides: Partial<GameSummary> = {}): GameSummary {
  return {
    id: 'game-1',
    playerName: 'Ada',
    status: 'Won',
    finalPrize: 2600,
    roundsPlayed: 5,
    startedAtUtc: new Date('2026-01-01T00:00:00.000Z'),
    endedAtUtc: new Date('2026-01-01T00:10:00.000Z'),
    rounds: [aRound()],
    ...overrides
  };
}
