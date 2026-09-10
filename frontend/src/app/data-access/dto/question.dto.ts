export interface AnswerDto {
  readonly id: string;
  readonly text: string;
  readonly isCorrect: boolean;
}

export interface QuestionDto {
  readonly id: string;
  readonly categoryId: string;
  readonly text: string;
  readonly isActive: boolean;
  readonly answers: readonly AnswerDto[];
}

export interface AnswerDraftRequestDto {
  readonly text: string;
  readonly isCorrect: boolean;
}

export interface CreateQuestionRequestDto {
  readonly categoryId: string;
  readonly text: string;
  readonly answers: readonly AnswerDraftRequestDto[];
}

export interface UpdateQuestionRequestDto {
  readonly text: string;
  readonly answers: readonly AnswerDraftRequestDto[];
}
