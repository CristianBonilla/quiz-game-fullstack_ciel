import { GameStatus } from '../enums/game-status';
import { PlayableQuestion } from './question';

export interface AnswerEvaluation {
  readonly gameId: string;
  readonly answeredRound: number;
  readonly isCorrect: boolean;
  readonly correctAnswerId: string;
  readonly status: GameStatus;
  readonly accumulatedPrize: number;
  readonly currentRound: number;
  readonly prizeAtStake: number;
  readonly nextQuestion: PlayableQuestion | null;
  readonly deadlineUtc: Date | null;
}
