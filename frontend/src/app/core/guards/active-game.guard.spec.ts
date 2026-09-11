import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, UrlTree } from '@angular/router';
import { GAME_GATEWAY, GAME_REPOSITORY } from '@core/config/injection-tokens';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { GameStore } from '@features/game/store/game.store';
import { FakeGameGateway } from '@testing/fake-game-gateway';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { FakeSignalrConnectionService } from '@testing/fake-signalr-connection.service';
import { beforeEach, describe, expect, it } from 'vitest';
import { activeGameGuard } from './active-game.guard';

describe('activeGameGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        { provide: GAME_REPOSITORY, useValue: new FakeGameRepository() },
        { provide: GAME_GATEWAY, useValue: new FakeGameGateway() },
        { provide: SignalrConnectionService, useValue: new FakeSignalrConnectionService() }
      ]
    });
  });

  it('should allow activation when a game is in progress', async () => {
    // Arrange
    await TestBed.inject(GameStore).joinGame('game-1');

    // Act
    const result = TestBed.runInInjectionContext(() => activeGameGuard({} as never, {} as never));

    // Assert
    expect(result).toBe(true);
  });

  it('should redirect to /setup when there is no game in progress', () => {
    // Arrange, Act
    const result = TestBed.runInInjectionContext(() => activeGameGuard({} as never, {} as never));

    // Assert
    expect(result).toBeInstanceOf(UrlTree);
  });
});
