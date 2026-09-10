export interface PlayableAnswerDto {
  readonly id: string;
  readonly text: string;
}

export interface PlayableQuestionDto {
  readonly id: string;
  readonly categoryId: string;
  readonly text: string;
  readonly answers: readonly PlayableAnswerDto[];
}

export interface GameStateDto {
  readonly gameId: string;
  readonly playerName: string;
  readonly status: string;
  readonly currentRound: number;
  readonly totalRounds: number;
  readonly accumulatedPrize: number;
  readonly prizeAtStake: number;
  readonly currentQuestion: PlayableQuestionDto | null;
  readonly deadlineUtc: string | null;
}

export interface AnswerQuestionDto {
  readonly gameId: string;
  readonly answeredRound: number;
  readonly isCorrect: boolean;
  readonly correctAnswerId: string;
  readonly status: string;
  readonly accumulatedPrize: number;
  readonly currentRound: number;
  readonly prizeAtStake: number;
  readonly nextQuestion: PlayableQuestionDto | null;
  readonly deadlineUtc: string | null;
}

export interface RoundSummaryDto {
  readonly number: number;
  readonly questionId: string;
  readonly outcome: string;
  readonly prizeAtStake: number;
  readonly selectedAnswerId: string | null;
  readonly answeredAtUtc: string | null;
}

export interface GameSummaryDto {
  readonly gameId: string;
  readonly playerName: string;
  readonly status: string;
  readonly finalPrize: number;
  readonly roundsPlayed: number;
  readonly startedAtUtc: string;
  readonly endedAtUtc: string | null;
  readonly rounds: readonly RoundSummaryDto[];
}

export interface RoundConfigurationDto {
  readonly roundNumber: number;
  readonly categoryId: string;
  readonly categoryName: string;
  readonly difficultyLevel: number;
  readonly prizeAmount: number;
  readonly availableQuestions: number;
}

export interface GameConfigurationDto {
  readonly totalRounds: number;
  readonly questionTimeLimitSeconds: number;
  readonly rounds: readonly RoundConfigurationDto[];
}

export interface StartGameRequestDto {
  readonly playerName: string;
}

export interface SubmitAnswerRequestDto {
  readonly gameId: string;
  readonly answerId: string;
  readonly requestId: string;
}

export interface WithdrawRequestDto {
  readonly gameId: string;
}
