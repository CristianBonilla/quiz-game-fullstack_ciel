import { GameStatus } from '../enums/game-status';
import { PlayableQuestion } from './question';

export interface Game {
  readonly id: string;
  readonly playerName: string;
  readonly status: GameStatus;
  readonly currentRound: number;
  readonly totalRounds: number;
  readonly accumulatedPrize: number;
  readonly prizeAtStake: number;
  readonly currentQuestion: PlayableQuestion | null;
  readonly deadlineUtc: Date | null;
}
