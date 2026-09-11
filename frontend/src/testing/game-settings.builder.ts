import { GameSettings, RoundConfiguration } from '@domain/models/game-settings';

export function aRoundConfiguration(overrides: Partial<RoundConfiguration> = {}): RoundConfiguration {
  return {
    roundNumber: 1,
    categoryId: 'category-1',
    categoryName: 'Software Development',
    difficultyLevel: 1,
    prizeAmount: 100,
    availableQuestions: 5,
    ...overrides
  };
}

export function aGameSettings(overrides: Partial<GameSettings> = {}): GameSettings {
  return {
    totalRounds: 5,
    questionTimeLimitSeconds: 30,
    rounds: Array.from({ length: 5 }, (_, index) =>
      aRoundConfiguration({ roundNumber: index + 1, prizeAmount: (index + 1) * 100 })
    ),
    ...overrides
  };
}
