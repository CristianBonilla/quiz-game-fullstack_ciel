import { PlayableAnswerDto } from './game.dto';

export interface GameStartedNotificationDto {
  readonly gameId: string;
  readonly playerName: string;
  readonly totalRounds: number;
  readonly startedOnUtc: string;
}

export interface RoundStartedNotificationDto {
  readonly gameId: string;
  readonly roundNumber: number;
  readonly questionId: string;
  readonly questionText: string;
  readonly answers: readonly PlayableAnswerDto[];
  readonly prizeAtStake: number;
  readonly deadlineUtc: string;
}

export interface AnswerEvaluatedNotificationDto {
  readonly gameId: string;
  readonly roundNumber: number;
  readonly isCorrect: boolean;
  readonly correctAnswerId: string;
  readonly accumulatedPrize: number;
  readonly status: string;
}

export interface RoundAdvancedNotificationDto {
  readonly gameId: string;
  readonly previousRoundNumber: number;
  readonly currentRoundNumber: number;
  readonly accumulatedPrize: number;
}

export interface PrizeAccumulatedNotificationDto {
  readonly gameId: string;
  readonly roundNumber: number;
  readonly prizeWon: number;
  readonly accumulatedPrize: number;
}

export interface GameEndedNotificationDto {
  readonly gameId: string;
  readonly playerName: string;
  readonly status: string;
  readonly finalPrize: number;
  readonly endedOnUtc: string;
}

export interface TimeRemainingNotificationDto {
  readonly gameId: string;
  readonly roundNumber: number;
  readonly secondsRemaining: number;
}
