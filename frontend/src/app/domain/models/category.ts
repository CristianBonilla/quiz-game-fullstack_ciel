import { DifficultyLevel } from '../enums/difficulty-level';

export interface Category {
  readonly id: string;
  readonly name: string;
  readonly description: string;
  readonly difficultyLevel: DifficultyLevel;
  readonly prizeAmount: number;
  readonly isActive: boolean;
  readonly questionCount: number;
}
