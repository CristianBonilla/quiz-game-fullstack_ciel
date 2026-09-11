import { describe, expect, it } from 'vitest';
import { aGame } from '@testing/game.builder';
import { aGameSettings } from '@testing/game-settings.builder';
import {
  canWithdraw,
  isFinalRound,
  isGameConfigured,
  isTerminalStatus,
  MINIMUM_QUESTIONS_PER_CATEGORY,
  prizeIfCorrect,
  progressPercent
} from './game-rules';

describe('canWithdraw', () => {
  it('should return true when the game is in progress', () => {
    // Arrange
    const game = aGame({ status: 'InProgress' });

    // Act
    const result = canWithdraw(game);

    // Assert
    expect(result).toBe(true);
  });

  it('should return false when the game has already ended', () => {
    // Arrange
    const game = aGame({ status: 'Won' });

    // Act
    const result = canWithdraw(game);

    // Assert
    expect(result).toBe(false);
  });
});

describe('isFinalRound', () => {
  it('should return false for the first round', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 5 });

    // Act
    const result = isFinalRound(1, settings);

    // Assert
    expect(result).toBe(false);
  });

  it('should return true for the last round', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 5 });

    // Act
    const result = isFinalRound(5, settings);

    // Assert
    expect(result).toBe(true);
  });

  it('should return true when the round exceeds the total rounds', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 5 });

    // Act
    const result = isFinalRound(6, settings);

    // Assert
    expect(result).toBe(true);
  });
});

describe('progressPercent', () => {
  it('should return 0 when the game is still on round 1', () => {
    // Arrange
    const game = aGame({ currentRound: 1 });
    const settings = aGameSettings({ totalRounds: 5 });

    // Act
    const result = progressPercent(game, settings);

    // Assert
    expect(result).toBe(0);
  });

  it('should return 100 when the game is on the round after the last one', () => {
    // Arrange
    const game = aGame({ currentRound: 6 });
    const settings = aGameSettings({ totalRounds: 5 });

    // Act
    const result = progressPercent(game, settings);

    // Assert
    expect(result).toBe(100);
  });

  it('should return 0 when there are no configured rounds', () => {
    // Arrange
    const game = aGame({ currentRound: 3 });
    const settings = aGameSettings({ totalRounds: 0 });

    // Act
    const result = progressPercent(game, settings);

    // Assert
    expect(result).toBe(0);
  });
});

describe('prizeIfCorrect', () => {
  it('should add the prize at stake to the accumulated prize', () => {
    // Arrange
    const game = aGame({ accumulatedPrize: 600, prizeAtStake: 400 });

    // Act
    const result = prizeIfCorrect(game);

    // Assert
    expect(result).toBe(1000);
  });
});

describe('isTerminalStatus', () => {
  it.each(['Won', 'Lost', 'Withdrawn', 'ForcedEnd'] as const)('should return true for %s', (status) => {
    // Arrange, Act
    const result = isTerminalStatus(status);

    // Assert
    expect(result).toBe(true);
  });

  it.each(['NotStarted', 'InProgress'] as const)('should return false for %s', (status) => {
    // Arrange, Act
    const result = isTerminalStatus(status);

    // Assert
    expect(result).toBe(false);
  });
});

describe('isGameConfigured', () => {
  it('should return true when every round has enough active questions', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 1, rounds: [{ roundNumber: 1, categoryId: 'c1', categoryName: 'Cat', difficultyLevel: 1, prizeAmount: 100, availableQuestions: MINIMUM_QUESTIONS_PER_CATEGORY }] });

    // Act
    const result = isGameConfigured(settings);

    // Assert
    expect(result).toBe(true);
  });

  it('should return false when a round has fewer questions than the minimum', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 1, rounds: [{ roundNumber: 1, categoryId: 'c1', categoryName: 'Cat', difficultyLevel: 1, prizeAmount: 100, availableQuestions: MINIMUM_QUESTIONS_PER_CATEGORY - 1 }] });

    // Act
    const result = isGameConfigured(settings);

    // Assert
    expect(result).toBe(false);
  });

  it('should return false when fewer rounds are configured than required', () => {
    // Arrange
    const settings = aGameSettings({ totalRounds: 5, rounds: [] });

    // Act
    const result = isGameConfigured(settings);

    // Assert
    expect(result).toBe(false);
  });
});
