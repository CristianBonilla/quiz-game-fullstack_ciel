import { Answer, PlayableAnswer } from './answer';

/** In-game projection: it never carries which answer is correct. */
export interface PlayableQuestion {
  readonly id: string;
  readonly categoryId: string;
  readonly text: string;
  readonly answers: readonly PlayableAnswer[];
}

export interface Question {
  readonly id: string;
  readonly categoryId: string;
  readonly text: string;
  readonly isActive: boolean;
  readonly answers: readonly Answer[];
}
