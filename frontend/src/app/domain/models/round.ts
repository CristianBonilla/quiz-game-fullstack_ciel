import { RoundOutcome } from '../enums/round-outcome';

export interface Round {
  readonly number: number;
  readonly questionId: string;
  readonly outcome: RoundOutcome;
  readonly prizeAtStake: number;
  readonly selectedAnswerId: string | null;
  readonly answeredAtUtc: Date | null;
}
