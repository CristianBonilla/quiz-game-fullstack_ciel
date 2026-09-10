import { GameStatus } from '../enums/game-status';
import { Game } from '../models/game';
import { GameSettings } from '../models/game-settings';

export const MINIMUM_QUESTIONS_PER_CATEGORY = 5;

const TERMINAL_STATUSES: readonly GameStatus[] = ['Won', 'Lost', 'Withdrawn', 'ForcedEnd'];

export function isTerminalStatus(status: GameStatus): boolean {
  return TERMINAL_STATUSES.includes(status);
}

export function canWithdraw(game: Game): boolean {
  return game.status === 'InProgress';
}

export function isFinalRound(round: number, settings: GameSettings): boolean {
  return round >= settings.totalRounds;
}

export function progressPercent(game: Game, settings: GameSettings): number {
  if (settings.totalRounds <= 0) {
    return 0;
  }

  const completedRounds = Math.min(Math.max(game.currentRound - 1, 0), settings.totalRounds);
  return Math.round((completedRounds / settings.totalRounds) * 100);
}

/** Preview only: the server remains the sole authority over the real accumulated prize. */
export function prizeIfCorrect(game: Game): number {
  return game.accumulatedPrize + game.prizeAtStake;
}

export function isGameConfigured(settings: GameSettings): boolean {
  return (
    settings.rounds.length === settings.totalRounds &&
    settings.rounds.every((round) => round.availableQuestions >= MINIMUM_QUESTIONS_PER_CATEGORY)
  );
}
