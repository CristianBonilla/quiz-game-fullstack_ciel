export interface PlayableAnswer {
  readonly id: string;
  readonly text: string;
}

export interface Answer extends PlayableAnswer {
  readonly isCorrect: boolean;
}
