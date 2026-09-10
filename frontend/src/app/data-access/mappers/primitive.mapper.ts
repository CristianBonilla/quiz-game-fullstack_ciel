import { DIFFICULTY_LEVEL_MAXIMUM, DIFFICULTY_LEVEL_MINIMUM, DifficultyLevel } from '@domain/enums/difficulty-level';
import { GameStatus } from '@domain/enums/game-status';
import { RoundOutcome } from '@domain/enums/round-outcome';

const GAME_STATUSES: readonly string[] = ['NotStarted', 'InProgress', 'Won', 'Lost', 'Withdrawn', 'ForcedEnd'];
const ROUND_OUTCOMES: readonly string[] = ['Pending', 'Correct', 'Incorrect'];

/**
 * The API serialises DateTime values that may reach the wire without a timezone designator when the
 * provider returns them as Unspecified. Everything the backend stores is UTC, so normalise here
 * rather than letting the browser reinterpret them as local time.
 */
export function toUtcDate(value: string): Date {
  const hasTimeZone = /(?:Z|[+-]\d{2}:?\d{2})$/.test(value);
  return new Date(hasTimeZone ? value : `${value}Z`);
}

export function toOptionalUtcDate(value: string | null): Date | null {
  return value === null ? null : toUtcDate(value);
}

export function toGameStatus(value: string): GameStatus {
  if (!GAME_STATUSES.includes(value)) {
    throw new Error(`Unknown game status received from the server: ${value}`);
  }

  return value as GameStatus;
}

export function toRoundOutcome(value: string): RoundOutcome {
  if (!ROUND_OUTCOMES.includes(value)) {
    throw new Error(`Unknown round outcome received from the server: ${value}`);
  }

  return value as RoundOutcome;
}

export function toDifficultyLevel(value: number): DifficultyLevel {
  if (!Number.isInteger(value) || value < DIFFICULTY_LEVEL_MINIMUM || value > DIFFICULTY_LEVEL_MAXIMUM) {
    throw new Error(`Difficulty level out of range received from the server: ${value}`);
  }

  return value as DifficultyLevel;
}
