import { describe, expect, it } from 'vitest';
import { isDifficultyLevel } from './difficulty-level';

describe('isDifficultyLevel', () => {
  it('should return true for a value within the valid range', () => {
    // Arrange, Act
    const result = isDifficultyLevel(3);

    // Assert
    expect(result).toBe(true);
  });

  it('should return false for a value below the minimum', () => {
    // Arrange, Act
    const result = isDifficultyLevel(0);

    // Assert
    expect(result).toBe(false);
  });

  it('should return false for a non-integer value', () => {
    // Arrange, Act
    const result = isDifficultyLevel(2.5);

    // Assert
    expect(result).toBe(false);
  });
});
