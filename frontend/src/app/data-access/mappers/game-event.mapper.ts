import {
  AnswerEvaluatedEvent,
  GameEndedEvent,
  GameStartedEvent,
  PrizeAccumulatedEvent,
  RoundAdvancedEvent,
  RoundStartedEvent,
  TimeRemainingEvent
} from '@domain/models/game-event';
import {
  AnswerEvaluatedNotificationDto,
  GameEndedNotificationDto,
  GameStartedNotificationDto,
  PrizeAccumulatedNotificationDto,
  RoundAdvancedNotificationDto,
  RoundStartedNotificationDto,
  TimeRemainingNotificationDto
} from '../dto/notification.dto';
import { toGameStatus, toUtcDate } from './primitive.mapper';

export function toGameStartedEvent(dto: GameStartedNotificationDto): GameStartedEvent {
  return {
    type: 'GameStarted',
    gameId: dto.gameId,
    playerName: dto.playerName,
    totalRounds: dto.totalRounds,
    startedOnUtc: toUtcDate(dto.startedOnUtc)
  };
}

export function toRoundStartedEvent(dto: RoundStartedNotificationDto): RoundStartedEvent {
  return {
    type: 'RoundStarted',
    gameId: dto.gameId,
    roundNumber: dto.roundNumber,
    questionId: dto.questionId,
    questionText: dto.questionText,
    answers: dto.answers.map((answer) => ({ id: answer.id, text: answer.text })),
    prizeAtStake: dto.prizeAtStake,
    deadlineUtc: toUtcDate(dto.deadlineUtc)
  };
}

export function toAnswerEvaluatedEvent(dto: AnswerEvaluatedNotificationDto): AnswerEvaluatedEvent {
  return {
    type: 'AnswerEvaluated',
    gameId: dto.gameId,
    roundNumber: dto.roundNumber,
    isCorrect: dto.isCorrect,
    correctAnswerId: dto.correctAnswerId,
    accumulatedPrize: dto.accumulatedPrize,
    status: toGameStatus(dto.status)
  };
}

export function toRoundAdvancedEvent(dto: RoundAdvancedNotificationDto): RoundAdvancedEvent {
  return {
    type: 'RoundAdvanced',
    gameId: dto.gameId,
    previousRoundNumber: dto.previousRoundNumber,
    currentRoundNumber: dto.currentRoundNumber,
    accumulatedPrize: dto.accumulatedPrize
  };
}

export function toPrizeAccumulatedEvent(dto: PrizeAccumulatedNotificationDto): PrizeAccumulatedEvent {
  return {
    type: 'PrizeAccumulated',
    gameId: dto.gameId,
    roundNumber: dto.roundNumber,
    prizeWon: dto.prizeWon,
    accumulatedPrize: dto.accumulatedPrize
  };
}

export function toGameEndedEvent(dto: GameEndedNotificationDto): GameEndedEvent {
  return {
    type: 'GameEnded',
    gameId: dto.gameId,
    playerName: dto.playerName,
    status: toGameStatus(dto.status),
    finalPrize: dto.finalPrize,
    endedOnUtc: toUtcDate(dto.endedOnUtc)
  };
}

export function toTimeRemainingEvent(dto: TimeRemainingNotificationDto): TimeRemainingEvent {
  return {
    type: 'TimeRemaining',
    gameId: dto.gameId,
    roundNumber: dto.roundNumber,
    secondsRemaining: dto.secondsRemaining
  };
}
