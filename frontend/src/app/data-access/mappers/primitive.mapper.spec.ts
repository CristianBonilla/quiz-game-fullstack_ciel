import { describe, expect, it } from 'vitest';
import { toDifficultyLevel, toGameStatus, toOptionalUtcDate, toRoundOutcome, toUtcDate } from './primitive.mapper';

describe('toUtcDate', () => {
  it('should parse an ISO date that already carries a timezone designator', () => {
    // Arrange
    const value = '2026-01-01T10:00:00.000Z';

    // Act
    const result = toUtcDate(value);

    // Assert
    expect(result.toISOString()).toBe('2026-01-01T10:00:00.000Z');
  });

  it('should treat a timezone-less date as UTC instead of local time', () => {
    // Arrange
    const value = '2026-01-01T10:00:00.000';

    // Act
    const result = toUtcDate(value);

    // Assert
    expect(result.toISOString()).toBe('2026-01-01T10:00:00.000Z');
  });
});

describe('toOptionalUtcDate', () => {
  it('should return null when the value is null', () => {
    // Arrange, Act
    const result = toOptionalUtcDate(null);

    // Assert
    expect(result).toBeNull();
  });

  it('should parse the value when it is not null', () => {
    // Arrange, Act
    const result = toOptionalUtcDate('2026-01-01T10:00:00.000Z');

    // Assert
    expect(result).toEqual(new Date('2026-01-01T10:00:00.000Z'));
  });
});

describe('toGameStatus', () => {
  it('should return the status when it is a known value', () => {
    // Arrange, Act
    const result = toGameStatus('InProgress');

    // Assert
    expect(result).toBe('InProgress');
  });

  it('should throw when the status is unknown', () => {
    // Arrange, Act, Assert
    expect(() => toGameStatus('Unknown')).toThrow();
  });
});

describe('toRoundOutcome', () => {
  it('should return the outcome when it is a known value', () => {
    // Arrange, Act
    const result = toRoundOutcome('Correct');

    // Assert
    expect(result).toBe('Correct');
  });

  it('should throw when the outcome is unknown', () => {
    // Arrange, Act, Assert
    expect(() => toRoundOutcome('Unknown')).toThrow();
  });
});

describe('toDifficultyLevel', () => {
  it('should return the level when it is within range', () => {
    // Arrange, Act
    const result = toDifficultyLevel(3);

    // Assert
    expect(result).toBe(3);
  });

  it('should throw when the level is out of range', () => {
    // Arrange, Act, Assert
    expect(() => toDifficultyLevel(6)).toThrow();
  });

  it('should throw when the level is not an integer', () => {
    // Arrange, Act, Assert
    expect(() => toDifficultyLevel(2.5)).toThrow();
  });
});
