import { GameStatus } from '../enums/game-status';
import { PlayableAnswer } from './answer';

export interface GameStartedEvent {
  readonly type: 'GameStarted';
  readonly gameId: string;
  readonly playerName: string;
  readonly totalRounds: number;
  readonly startedOnUtc: Date;
}

export interface RoundStartedEvent {
  readonly type: 'RoundStarted';
  readonly gameId: string;
  readonly roundNumber: number;
  readonly questionId: string;
  readonly questionText: string;
  readonly answers: readonly PlayableAnswer[];
  readonly prizeAtStake: number;
  readonly deadlineUtc: Date;
}

export interface AnswerEvaluatedEvent {
  readonly type: 'AnswerEvaluated';
  readonly gameId: string;
  readonly roundNumber: number;
  readonly isCorrect: boolean;
  readonly correctAnswerId: string;
  readonly accumulatedPrize: number;
  readonly status: GameStatus;
}

export interface RoundAdvancedEvent {
  readonly type: 'RoundAdvanced';
  readonly gameId: string;
  readonly previousRoundNumber: number;
  readonly currentRoundNumber: number;
  readonly accumulatedPrize: number;
}

export interface PrizeAccumulatedEvent {
  readonly type: 'PrizeAccumulated';
  readonly gameId: string;
  readonly roundNumber: number;
  readonly prizeWon: number;
  readonly accumulatedPrize: number;
}

export interface GameEndedEvent {
  readonly type: 'GameEnded';
  readonly gameId: string;
  readonly playerName: string;
  readonly status: GameStatus;
  readonly finalPrize: number;
  readonly endedOnUtc: Date;
}

export interface TimeRemainingEvent {
  readonly type: 'TimeRemaining';
  readonly gameId: string;
  readonly roundNumber: number;
  readonly secondsRemaining: number;
}

export type GameEvent =
  | GameStartedEvent
  | RoundStartedEvent
  | AnswerEvaluatedEvent
  | RoundAdvancedEvent
  | PrizeAccumulatedEvent
  | GameEndedEvent
  | TimeRemainingEvent;

export type GameEventType = GameEvent['type'];

/**
 * The hub delivers events at-least-once and carries no event id, so identity is derived from the
 * fields that make each emission unique. TimeRemaining includes the seconds so genuine ticks pass.
 */
export function gameEventIdentity(event: GameEvent): string {
  switch (event.type) {
    case 'GameStarted':
      return `GameStarted:${event.gameId}`;
    case 'RoundStarted':
      return `RoundStarted:${event.gameId}:${event.roundNumber}`;
    case 'AnswerEvaluated':
      return `AnswerEvaluated:${event.gameId}:${event.roundNumber}`;
    case 'RoundAdvanced':
      return `RoundAdvanced:${event.gameId}:${event.previousRoundNumber}:${event.currentRoundNumber}`;
    case 'PrizeAccumulated':
      return `PrizeAccumulated:${event.gameId}:${event.roundNumber}`;
    case 'GameEnded':
      return `GameEnded:${event.gameId}`;
    case 'TimeRemaining':
      return `TimeRemaining:${event.gameId}:${event.roundNumber}:${event.secondsRemaining}`;
  }
}
