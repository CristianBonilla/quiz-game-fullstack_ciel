export const DIFFICULTY_LEVEL_MINIMUM = 1;
export const DIFFICULTY_LEVEL_MAXIMUM = 5;

export type DifficultyLevel = 1 | 2 | 3 | 4 | 5;

export function isDifficultyLevel(value: number): value is DifficultyLevel {
  return Number.isInteger(value) && value >= DIFFICULTY_LEVEL_MINIMUM && value <= DIFFICULTY_LEVEL_MAXIMUM;
}
