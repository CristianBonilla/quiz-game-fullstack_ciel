import { GameGateway, Unsubscribe } from '@domain/contracts/game-gateway';
import { AnswerEvaluation } from '@domain/models/answer-evaluation';
import { Game } from '@domain/models/game';
import { GameEvent } from '@domain/models/game-event';
import { GameSummary } from '@domain/models/game-summary';
import { SubmitAnswerRequest, WithdrawRequest } from '@domain/models/requests';
import { aGame } from './game.builder';
import { aGameSummary } from './game-summary.builder';
import { anAnswerEvaluation } from './answer-evaluation.builder';

export class FakeGameGateway implements GameGateway {
  joinGameResult: Game = aGame();
  submitAnswerResult: AnswerEvaluation = anAnswerEvaluation();
  withdrawResult: GameSummary = aGameSummary();
  shouldFail = false;

  readonly joinGameCalls: string[] = [];
  readonly leaveGameCalls: string[] = [];
  readonly submitAnswerCalls: SubmitAnswerRequest[] = [];
  readonly withdrawCalls: WithdrawRequest[] = [];

  private readonly handlers: Array<(event: GameEvent) => void> = [];

  async joinGame(gameId: string): Promise<Game> {
    this.joinGameCalls.push(gameId);
    if (this.shouldFail) {
      throw new Error('joinGame failed');
    }

    return { ...this.joinGameResult, id: gameId };
  }

  async leaveGame(gameId: string): Promise<void> {
    this.leaveGameCalls.push(gameId);
  }

  async submitAnswer(request: SubmitAnswerRequest): Promise<AnswerEvaluation> {
    this.submitAnswerCalls.push(request);
    if (this.shouldFail) {
      throw new Error('submitAnswer failed');
    }

    return this.submitAnswerResult;
  }

  async withdraw(request: WithdrawRequest): Promise<GameSummary> {
    this.withdrawCalls.push(request);
    if (this.shouldFail) {
      throw new Error('withdraw failed');
    }

    return this.withdrawResult;
  }

  onEvent(handler: (event: GameEvent) => void): Unsubscribe {
    this.handlers.push(handler);
    return () => {
      const index = this.handlers.indexOf(handler);
      if (index !== -1) {
        this.handlers.splice(index, 1);
      }
    };
  }

  emit(event: GameEvent): void {
    for (const handler of this.handlers) {
      handler(event);
    }
  }
}
