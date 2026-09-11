export interface PlayableAnswerView {
  readonly id: string;
  readonly text: string;
  readonly index: number;
}

export interface AnswerFeedback {
  readonly selectedAnswerId: string | null;
  readonly correctAnswerId: string;
  readonly isCorrect: boolean;
}
