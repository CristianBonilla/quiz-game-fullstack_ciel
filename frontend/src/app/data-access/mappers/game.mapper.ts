import { AnswerEvaluation } from '@domain/models/answer-evaluation';
import { Game } from '@domain/models/game';
import { GameSettings, RoundConfiguration } from '@domain/models/game-settings';
import { GameSummary } from '@domain/models/game-summary';
import { PlayableQuestion } from '@domain/models/question';
import { Round } from '@domain/models/round';
import { StartGameRequest, SubmitAnswerRequest, WithdrawRequest } from '@domain/models/requests';
import {
  AnswerQuestionDto,
  GameConfigurationDto,
  GameStateDto,
  GameSummaryDto,
  PlayableQuestionDto,
  RoundConfigurationDto,
  RoundSummaryDto,
  StartGameRequestDto,
  SubmitAnswerRequestDto,
  WithdrawRequestDto
} from '../dto/game.dto';
import { toDifficultyLevel, toGameStatus, toOptionalUtcDate, toRoundOutcome, toUtcDate } from './primitive.mapper';

export function toPlayableQuestion(dto: PlayableQuestionDto): PlayableQuestion {
  return {
    id: dto.id,
    categoryId: dto.categoryId,
    text: dto.text,
    answers: dto.answers.map((answer) => ({ id: answer.id, text: answer.text }))
  };
}

export function toGame(dto: GameStateDto): Game {
  return {
    id: dto.gameId,
    playerName: dto.playerName,
    status: toGameStatus(dto.status),
    currentRound: dto.currentRound,
    totalRounds: dto.totalRounds,
    accumulatedPrize: dto.accumulatedPrize,
    prizeAtStake: dto.prizeAtStake,
    currentQuestion: dto.currentQuestion === null ? null : toPlayableQuestion(dto.currentQuestion),
    deadlineUtc: toOptionalUtcDate(dto.deadlineUtc)
  };
}

export function toAnswerEvaluation(dto: AnswerQuestionDto): AnswerEvaluation {
  return {
    gameId: dto.gameId,
    answeredRound: dto.answeredRound,
    isCorrect: dto.isCorrect,
    correctAnswerId: dto.correctAnswerId,
    status: toGameStatus(dto.status),
    accumulatedPrize: dto.accumulatedPrize,
    currentRound: dto.currentRound,
    prizeAtStake: dto.prizeAtStake,
    nextQuestion: dto.nextQuestion === null ? null : toPlayableQuestion(dto.nextQuestion),
    deadlineUtc: toOptionalUtcDate(dto.deadlineUtc)
  };
}

export function toRound(dto: RoundSummaryDto): Round {
  return {
    number: dto.number,
    questionId: dto.questionId,
    outcome: toRoundOutcome(dto.outcome),
    prizeAtStake: dto.prizeAtStake,
    selectedAnswerId: dto.selectedAnswerId,
    answeredAtUtc: toOptionalUtcDate(dto.answeredAtUtc)
  };
}

export function toGameSummary(dto: GameSummaryDto): GameSummary {
  return {
    id: dto.gameId,
    playerName: dto.playerName,
    status: toGameStatus(dto.status),
    finalPrize: dto.finalPrize,
    roundsPlayed: dto.roundsPlayed,
    startedAtUtc: toUtcDate(dto.startedAtUtc),
    endedAtUtc: toOptionalUtcDate(dto.endedAtUtc),
    rounds: dto.rounds.map(toRound)
  };
}

export function toRoundConfiguration(dto: RoundConfigurationDto): RoundConfiguration {
  return {
    roundNumber: dto.roundNumber,
    categoryId: dto.categoryId,
    categoryName: dto.categoryName,
    difficultyLevel: toDifficultyLevel(dto.difficultyLevel),
    prizeAmount: dto.prizeAmount,
    availableQuestions: dto.availableQuestions
  };
}

export function toGameSettings(dto: GameConfigurationDto): GameSettings {
  return {
    totalRounds: dto.totalRounds,
    questionTimeLimitSeconds: dto.questionTimeLimitSeconds,
    rounds: dto.rounds.map(toRoundConfiguration)
  };
}

export function toStartGameRequestDto(request: StartGameRequest): StartGameRequestDto {
  return { playerName: request.playerName };
}

export function toSubmitAnswerRequestDto(request: SubmitAnswerRequest): SubmitAnswerRequestDto {
  return { gameId: request.gameId, answerId: request.answerId, requestId: request.requestId };
}

export function toWithdrawRequestDto(request: WithdrawRequest): WithdrawRequestDto {
  return { gameId: request.gameId };
}
