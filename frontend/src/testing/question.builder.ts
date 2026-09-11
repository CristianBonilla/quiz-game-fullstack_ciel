import { PlayableAnswer } from '@domain/models/answer';
import { PlayableQuestion } from '@domain/models/question';

export interface QuestionBuilderOptions {
  readonly id?: string;
  readonly categoryId?: string;
  readonly text?: string;
  readonly answersCount?: number;
}

export function aQuestion(options: QuestionBuilderOptions = {}): PlayableQuestion {
  const answersCount = options.answersCount ?? 4;
  const answers: PlayableAnswer[] = Array.from({ length: answersCount }, (_, index) => ({
    id: `answer-${index + 1}`,
    text: `Answer ${index + 1}`
  }));

  return {
    id: options.id ?? 'question-1',
    categoryId: options.categoryId ?? 'category-1',
    text: options.text ?? 'What is the answer?',
    answers
  };
}
