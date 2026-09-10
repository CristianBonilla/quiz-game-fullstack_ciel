import { DifficultyLevel } from '../enums/difficulty-level';

export interface RoundConfiguration {
  readonly roundNumber: number;
  readonly categoryId: string;
  readonly categoryName: string;
  readonly difficultyLevel: DifficultyLevel;
  readonly prizeAmount: number;
  readonly availableQuestions: number;
}

export interface GameSettings {
  readonly totalRounds: number;
  readonly questionTimeLimitSeconds: number;
  readonly rounds: readonly RoundConfiguration[];
}
