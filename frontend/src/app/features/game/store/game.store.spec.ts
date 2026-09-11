import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { GAME_GATEWAY, GAME_REPOSITORY } from '@core/config/injection-tokens';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { FakeGameGateway } from '@testing/fake-game-gateway';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { FakeSignalrConnectionService } from '@testing/fake-signalr-connection.service';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { GameStore } from './game.store';

describe('GameStore', () => {
  let repository: FakeGameRepository;
  let gateway: FakeGameGateway;
  let connection: FakeSignalrConnectionService;

  beforeEach(() => {
    repository = new FakeGameRepository();
    gateway = new FakeGameGateway();
    connection = new FakeSignalrConnectionService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: GAME_REPOSITORY, useValue: repository },
        { provide: GAME_GATEWAY, useValue: gateway },
        { provide: SignalrConnectionService, useValue: connection }
      ]
    });
  });

  function createStore(): InstanceType<typeof GameStore> {
    return TestBed.inject(GameStore);
  }

  it('should update currentQuestion and clear selectedAnswerId when RoundStarted is received', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');
    store.selectAnswer('previous-answer');

    // Act
    gateway.emit({
      type: 'RoundStarted',
      gameId: 'game-1',
      roundNumber: 2,
      questionId: 'question-2',
      questionText: 'Next question?',
      answers: [{ id: 'answer-1', text: 'A' }],
      prizeAtStake: 200,
      deadlineUtc: new Date('2026-01-01T10:00:30.000Z')
    });

    // Assert
    expect(store.currentQuestion()?.text).toBe('Next question?');
    expect(store.selectedAnswerId()).toBeNull();
  });

  it('should reflect the status and accumulated prize sent with AnswerEvaluated', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    gateway.emit({
      type: 'AnswerEvaluated',
      gameId: 'game-1',
      roundNumber: 1,
      isCorrect: true,
      correctAnswerId: 'answer-1',
      accumulatedPrize: 500,
      status: 'InProgress'
    });

    // Assert
    expect(store.game()?.status).toBe('InProgress');
    expect(store.accumulatedPrize()).toBe(500);
  });

  it('should increment currentRound and reflect the server accumulated prize when RoundAdvanced is received', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    gateway.emit({
      type: 'RoundAdvanced',
      gameId: 'game-1',
      previousRoundNumber: 1,
      currentRoundNumber: 2,
      accumulatedPrize: 700
    });

    // Assert
    expect(store.game()?.currentRound).toBe(2);
    expect(store.accumulatedPrize()).toBe(700);
  });

  it('should reset the accumulated prize to the server value and mark the game over when GameEnded is Lost', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    gateway.emit({
      type: 'GameEnded',
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'Lost',
      finalPrize: 0,
      endedOnUtc: new Date('2026-01-01T10:10:00.000Z')
    });

    // Assert
    expect(store.accumulatedPrize()).toBe(0);
    expect(store.isGameOver()).toBe(true);
  });

  it('should preserve the accumulated prize when GameEnded is Withdrawn', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    gateway.emit({
      type: 'GameEnded',
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'Withdrawn',
      finalPrize: 900,
      endedOnUtc: new Date('2026-01-01T10:10:00.000Z')
    });

    // Assert
    expect(store.accumulatedPrize()).toBe(900);
    expect(store.isGameOver()).toBe(true);
  });

  it('should not change the state twice when the same event is applied more than once', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');
    const event = {
      type: 'PrizeAccumulated' as const,
      gameId: 'game-1',
      roundNumber: 1,
      prizeWon: 100,
      accumulatedPrize: 100
    };
    gateway.emit(event);
    const stateAfterFirstApplication = store.accumulatedPrize();

    // Act
    gateway.emit(event);

    // Assert
    expect(store.accumulatedPrize()).toBe(stateAfterFirstApplication);
  });

  it('should reflect whatever accumulated prize the server sends without recalculating it', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act: an intentionally incoherent value the client could never have derived on its own.
    gateway.emit({
      type: 'PrizeAccumulated',
      gameId: 'game-1',
      roundNumber: 1,
      prizeWon: 999999,
      accumulatedPrize: -50
    });

    // Assert
    expect(store.accumulatedPrize()).toBe(-50);
  });

  it('should ignore events addressed to a different game', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    gateway.emit({
      type: 'RoundAdvanced',
      gameId: 'another-game',
      previousRoundNumber: 1,
      currentRoundNumber: 2,
      accumulatedPrize: 999
    });

    // Assert
    expect(store.game()?.currentRound).toBe(1);
  });

  it('should rehydrate the game state from the server when reconnecting transitions to connected', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    connection.setState('reconnecting');
    await vi.waitFor(() => expect(store.connectionState()).toBe('reconnecting'));
    connection.setState('connected');

    // Assert
    await vi.waitFor(() => expect(repository.getGameStateCalls).toEqual(['game-1']));
  });

  it('should not rehydrate when the connection goes from disconnected directly to connected', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');

    // Act
    connection.setState('connected');
    await vi.waitFor(() => expect(store.connectionState()).toBe('connected'));

    // Assert
    expect(repository.getGameStateCalls).toEqual([]);
  });

  it('should set status to error and populate error when the gateway fails', async () => {
    // Arrange
    const store = createStore();
    await store.joinGame('game-1');
    gateway.shouldFail = true;

    // Act
    await store.submitAnswer('answer-1');

    // Assert
    expect(store.status()).toBe('error');
    expect(store.error()).not.toBeNull();
  });
});
