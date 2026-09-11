import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { convertToParamMap, provideRouter, UrlTree } from '@angular/router';
import { GAME_REPOSITORY } from '@core/config/injection-tokens';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { aGameSummary } from '@testing/game-summary.builder';
import { beforeEach, describe, expect, it } from 'vitest';
import { finishedGameGuard } from './finished-game.guard';

describe('finishedGameGuard', () => {
  let repository: FakeGameRepository;

  beforeEach(() => {
    repository = new FakeGameRepository();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        { provide: GAME_REPOSITORY, useValue: repository }
      ]
    });
  });

  function routeWithId(id: string | null): { paramMap: ReturnType<typeof convertToParamMap> } {
    return { paramMap: convertToParamMap(id === null ? {} : { id }) };
  }

  it('should allow activation when the game already ended', async () => {
    // Arrange
    repository.gameSummary = aGameSummary({ status: 'Won' });

    // Act
    const result = await TestBed.runInInjectionContext(() =>
      finishedGameGuard(routeWithId('game-1') as never, {} as never)
    );

    // Assert
    expect(result).toBe(true);
  });

  it('should redirect to /game/play when the game is still in progress', async () => {
    // Arrange
    repository.gameSummary = aGameSummary({ status: 'InProgress' });

    // Act
    const result = await TestBed.runInInjectionContext(() =>
      finishedGameGuard(routeWithId('game-1') as never, {} as never)
    );

    // Assert
    expect(result).toBeInstanceOf(UrlTree);
  });

  it('should redirect to /setup when the route has no game id', async () => {
    // Arrange, Act
    const result = await TestBed.runInInjectionContext(() =>
      finishedGameGuard(routeWithId(null) as never, {} as never)
    );

    // Assert
    expect(result).toBeInstanceOf(UrlTree);
  });
});
