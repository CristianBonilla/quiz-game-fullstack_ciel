import { DifficultyLevel } from '@domain/enums/difficulty-level';

export const DIFFICULTY_LEVEL_LABELS: Record<DifficultyLevel, string> = {
  1: 'Muy Fácil',
  2: 'Fácil',
  3: 'Intermedio',
  4: 'Difícil',
  5: 'Experto'
};

export interface DifficultyLevelOption {
  readonly label: string;
  readonly value: DifficultyLevel;
}

export const DIFFICULTY_LEVEL_OPTIONS: readonly DifficultyLevelOption[] = (
  Object.keys(DIFFICULTY_LEVEL_LABELS) as unknown as DifficultyLevel[]
)
  .map(Number)
  .sort((a, b) => a - b)
  .map((value) => ({ label: DIFFICULTY_LEVEL_LABELS[value as DifficultyLevel], value: value as DifficultyLevel }));

export function difficultyLevelLabel(level: DifficultyLevel): string {
  return DIFFICULTY_LEVEL_LABELS[level];
}
