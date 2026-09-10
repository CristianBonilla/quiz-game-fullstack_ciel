import { DifficultyLevel } from '../enums/difficulty-level';

export interface StartGameRequest {
  readonly playerName: string;
}

export interface SubmitAnswerRequest {
  readonly gameId: string;
  readonly answerId: string;
  readonly requestId: string;
}

export interface WithdrawRequest {
  readonly gameId: string;
}

export interface CreateCategoryRequest {
  readonly name: string;
  readonly description: string;
  readonly difficultyLevel: DifficultyLevel;
  readonly prizeAmount: number;
}

export type UpdateCategoryRequest = CreateCategoryRequest;

export interface AnswerDraft {
  readonly text: string;
  readonly isCorrect: boolean;
}

export interface CreateQuestionRequest {
  readonly categoryId: string;
  readonly text: string;
  readonly answers: readonly AnswerDraft[];
}

export interface UpdateQuestionRequest {
  readonly text: string;
  readonly answers: readonly AnswerDraft[];
}
