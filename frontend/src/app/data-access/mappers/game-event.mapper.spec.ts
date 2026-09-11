import { describe, expect, it } from 'vitest';
import {
  toAnswerEvaluatedEvent,
  toGameEndedEvent,
  toGameStartedEvent,
  toPrizeAccumulatedEvent,
  toRoundAdvancedEvent,
  toRoundStartedEvent,
  toTimeRemainingEvent
} from './game-event.mapper';

describe('toGameStartedEvent', () => {
  it('should map the notification into a typed GameStarted event', () => {
    // Arrange
    const dto = { gameId: 'game-1', playerName: 'Ada', totalRounds: 5, startedOnUtc: '2026-01-01T10:00:00.000Z' };

    // Act
    const result = toGameStartedEvent(dto);

    // Assert
    expect(result).toEqual({
      type: 'GameStarted',
      gameId: 'game-1',
      playerName: 'Ada',
      totalRounds: 5,
      startedOnUtc: new Date('2026-01-01T10:00:00.000Z')
    });
  });
});

describe('toRoundStartedEvent', () => {
  it('should not leak which answer is correct', () => {
    // Arrange
    const dto = {
      gameId: 'game-1',
      roundNumber: 2,
      questionId: 'question-1',
      questionText: 'What is 2 + 2?',
      answers: [{ id: 'answer-1', text: '4' }],
      prizeAtStake: 200,
      deadlineUtc: '2026-01-01T10:00:00.000Z'
    };

    // Act
    const result = toRoundStartedEvent(dto);

    // Assert
    expect(result.answers[0]).not.toHaveProperty('isCorrect');
  });
});

describe('toAnswerEvaluatedEvent', () => {
  it('should map the outcome status verbatim from the server', () => {
    // Arrange
    const dto = {
      gameId: 'game-1',
      roundNumber: 3,
      isCorrect: false,
      correctAnswerId: 'answer-2',
      accumulatedPrize: 0,
      status: 'Lost'
    };

    // Act
    const result = toAnswerEvaluatedEvent(dto);

    // Assert
    expect(result.status).toBe('Lost');
    expect(result.isCorrect).toBe(false);
  });
});

describe('toRoundAdvancedEvent', () => {
  it('should map the previous and current round numbers', () => {
    // Arrange
    const dto = { gameId: 'game-1', previousRoundNumber: 2, currentRoundNumber: 3, accumulatedPrize: 600 };

    // Act
    const result = toRoundAdvancedEvent(dto);

    // Assert
    expect(result.previousRoundNumber).toBe(2);
    expect(result.currentRoundNumber).toBe(3);
  });
});

describe('toPrizeAccumulatedEvent', () => {
  it('should map the prize won and the running total as sent by the server', () => {
    // Arrange
    const dto = { gameId: 'game-1', roundNumber: 3, prizeWon: 500, accumulatedPrize: 600 };

    // Act
    const result = toPrizeAccumulatedEvent(dto);

    // Assert
    expect(result.prizeWon).toBe(500);
    expect(result.accumulatedPrize).toBe(600);
  });
});

describe('toGameEndedEvent', () => {
  it('should map the ended timestamp to a UTC date', () => {
    // Arrange
    const dto = {
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'Won',
      finalPrize: 2600,
      endedOnUtc: '2026-01-01T10:00:00.000Z'
    };

    // Act
    const result = toGameEndedEvent(dto);

    // Assert
    expect(result.endedOnUtc).toEqual(new Date('2026-01-01T10:00:00.000Z'));
  });
});

describe('toTimeRemainingEvent', () => {
  it('should map the seconds remaining verbatim', () => {
    // Arrange
    const dto = { gameId: 'game-1', roundNumber: 1, secondsRemaining: 7 };

    // Act
    const result = toTimeRemainingEvent(dto);

    // Assert
    expect(result.secondsRemaining).toBe(7);
  });
});
