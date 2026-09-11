import { Game } from '@domain/models/game';

export function aGame(overrides: Partial<Game> = {}): Game {
  return {
    id: 'game-1',
    playerName: 'Ada',
    status: 'InProgress',
    currentRound: 1,
    totalRounds: 5,
    accumulatedPrize: 0,
    prizeAtStake: 100,
    currentQuestion: null,
    deadlineUtc: null,
    ...overrides
  };
}
