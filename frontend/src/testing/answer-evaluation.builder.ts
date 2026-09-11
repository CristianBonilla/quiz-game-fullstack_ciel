import { AnswerEvaluation } from '@domain/models/answer-evaluation';

export function anAnswerEvaluation(overrides: Partial<AnswerEvaluation> = {}): AnswerEvaluation {
  return {
    gameId: 'game-1',
    answeredRound: 1,
    isCorrect: true,
    correctAnswerId: 'answer-1',
    status: 'InProgress',
    accumulatedPrize: 100,
    currentRound: 2,
    prizeAtStake: 200,
    nextQuestion: null,
    deadlineUtc: null,
    ...overrides
  };
}
