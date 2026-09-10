import { AnswerEvaluation } from '../models/answer-evaluation';
import { Game } from '../models/game';
import { GameEvent } from '../models/game-event';
import { GameSummary } from '../models/game-summary';
import { SubmitAnswerRequest, WithdrawRequest } from '../models/requests';

export type Unsubscribe = () => void;

/**
 * Real-time port. Subscription is callback based instead of Observable so this layer stays free of
 * any framework dependency; the adapter is free to back it with a Subject.
 */
export interface GameGateway {
  joinGame(gameId: string): Promise<Game>;
  leaveGame(gameId: string): Promise<void>;
  submitAnswer(request: SubmitAnswerRequest): Promise<AnswerEvaluation>;
  withdraw(request: WithdrawRequest): Promise<GameSummary>;
  onEvent(handler: (event: GameEvent) => void): Unsubscribe;
}
