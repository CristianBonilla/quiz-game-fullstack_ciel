import { GameStatus } from '../enums/game-status';
import { Round } from './round';

export interface GameSummary {
  readonly id: string;
  readonly playerName: string;
  readonly status: GameStatus;
  readonly finalPrize: number;
  readonly roundsPlayed: number;
  readonly startedAtUtc: Date;
  readonly endedAtUtc: Date | null;
  readonly rounds: readonly Round[];
}
