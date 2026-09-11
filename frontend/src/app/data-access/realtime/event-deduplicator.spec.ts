import { describe, expect, it } from 'vitest';
import { EventDeduplicator } from './event-deduplicator';

describe('EventDeduplicator', () => {
  it('should process an identity the first time it is seen', () => {
    // Arrange
    const deduplicator = new EventDeduplicator();

    // Act
    const result = deduplicator.shouldProcess('RoundAdvanced:game-1:1:2');

    // Assert
    expect(result).toBe(true);
  });

  it('should reject a duplicate identity so the event is not applied twice', () => {
    // Arrange
    const deduplicator = new EventDeduplicator();
    deduplicator.shouldProcess('RoundAdvanced:game-1:1:2');

    // Act
    const result = deduplicator.shouldProcess('RoundAdvanced:game-1:1:2');

    // Assert
    expect(result).toBe(false);
  });

  it('should evict the oldest identity once capacity is exceeded', () => {
    // Arrange
    const deduplicator = new EventDeduplicator(1);
    deduplicator.shouldProcess('first');
    deduplicator.shouldProcess('second');

    // Act
    const result = deduplicator.shouldProcess('first');

    // Assert
    expect(result).toBe(true);
  });

  it('should accept every identity again after clear', () => {
    // Arrange
    const deduplicator = new EventDeduplicator();
    deduplicator.shouldProcess('first');
    deduplicator.clear();

    // Act
    const result = deduplicator.shouldProcess('first');

    // Assert
    expect(result).toBe(true);
  });
});
