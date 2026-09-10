import { inject, Injectable } from '@angular/core';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { GameGateway, Unsubscribe } from '@domain/contracts/game-gateway';
import { AnswerEvaluation } from '@domain/models/answer-evaluation';
import { Game } from '@domain/models/game';
import { GameEvent, gameEventIdentity } from '@domain/models/game-event';
import { GameSummary } from '@domain/models/game-summary';
import { SubmitAnswerRequest, WithdrawRequest } from '@domain/models/requests';
import { Subject } from 'rxjs';
import { AnswerQuestionDto, GameStateDto, GameSummaryDto } from '../dto/game.dto';
import { HubResponseDto } from '../dto/hub-response.dto';
import {
  AnswerEvaluatedNotificationDto,
  GameEndedNotificationDto,
  GameStartedNotificationDto,
  PrizeAccumulatedNotificationDto,
  RoundAdvancedNotificationDto,
  RoundStartedNotificationDto,
  TimeRemainingNotificationDto
} from '../dto/notification.dto';
import { toAppErrorFromHubResponse } from '../mappers/app-error.mapper';
import {
  toAnswerEvaluatedEvent,
  toGameEndedEvent,
  toGameStartedEvent,
  toPrizeAccumulatedEvent,
  toRoundAdvancedEvent,
  toRoundStartedEvent,
  toTimeRemainingEvent
} from '../mappers/game-event.mapper';
import { toAnswerEvaluation, toGame, toGameSummary, toSubmitAnswerRequestDto, toWithdrawRequestDto } from '../mappers/game.mapper';
import { EventDeduplicator } from './event-deduplicator';
import { HUB_EVENTS, HUB_METHODS } from './hub-contract';

@Injectable()
export class SignalrGameGateway implements GameGateway {
  private readonly connection = inject(SignalrConnectionService);
  private readonly events = new Subject<GameEvent>();
  private readonly deduplicator = new EventDeduplicator();
  private handlersRegistered = false;

  async joinGame(gameId: string): Promise<Game> {
    this.registerHandlers();
    const response = await this.connection.invoke<HubResponseDto<GameStateDto>>(HUB_METHODS.joinGame, gameId);

    return toGame(this.unwrap(response));
  }

  async leaveGame(gameId: string): Promise<void> {
    await this.connection.invoke<void>(HUB_METHODS.leaveGame, gameId);
  }

  async submitAnswer(request: SubmitAnswerRequest): Promise<AnswerEvaluation> {
    const response = await this.connection.invoke<HubResponseDto<AnswerQuestionDto>>(
      HUB_METHODS.submitAnswer,
      toSubmitAnswerRequestDto(request)
    );

    return toAnswerEvaluation(this.unwrap(response));
  }

  async withdraw(request: WithdrawRequest): Promise<GameSummary> {
    const response = await this.connection.invoke<HubResponseDto<GameSummaryDto>>(
      HUB_METHODS.withdraw,
      toWithdrawRequestDto(request)
    );

    return toGameSummary(this.unwrap(response));
  }

  onEvent(handler: (event: GameEvent) => void): Unsubscribe {
    this.registerHandlers();
    const subscription = this.events.subscribe(handler);

    return () => subscription.unsubscribe();
  }

  private registerHandlers(): void {
    if (this.handlersRegistered) {
      return;
    }

    this.handlersRegistered = true;

    this.subscribe(HUB_EVENTS.gameStarted, (payload) =>
      toGameStartedEvent(payload as GameStartedNotificationDto)
    );
    this.subscribe(HUB_EVENTS.roundStarted, (payload) =>
      toRoundStartedEvent(payload as RoundStartedNotificationDto)
    );
    this.subscribe(HUB_EVENTS.answerEvaluated, (payload) =>
      toAnswerEvaluatedEvent(payload as AnswerEvaluatedNotificationDto)
    );
    this.subscribe(HUB_EVENTS.roundAdvanced, (payload) =>
      toRoundAdvancedEvent(payload as RoundAdvancedNotificationDto)
    );
    this.subscribe(HUB_EVENTS.prizeAccumulated, (payload) =>
      toPrizeAccumulatedEvent(payload as PrizeAccumulatedNotificationDto)
    );
    this.subscribe(HUB_EVENTS.gameEnded, (payload) => toGameEndedEvent(payload as GameEndedNotificationDto));
    this.subscribe(HUB_EVENTS.timeRemaining, (payload) =>
      toTimeRemainingEvent(payload as TimeRemainingNotificationDto)
    );
  }

  private subscribe(methodName: string, map: (payload: unknown) => GameEvent): void {
    this.connection.on(methodName, (payload) => {
      const event = map(payload);

      if (this.deduplicator.shouldProcess(gameEventIdentity(event))) {
        this.events.next(event);
      }
    });
  }

  private unwrap<T>(response: HubResponseDto<T>): T {
    if (!response.isSuccess || response.value === null) {
      throw toAppErrorFromHubResponse(response);
    }

    return response.value;
  }
}
