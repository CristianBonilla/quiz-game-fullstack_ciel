import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { GAME_GATEWAY, GAME_REPOSITORY } from '@core/config/injection-tokens';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { GameStore } from '@features/game/store/game.store';
import { FakeConfirmationService } from '@testing/fake-confirmation.service';
import { FakeGameGateway } from '@testing/fake-game-gateway';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { FakeSignalrConnectionService } from '@testing/fake-signalr-connection.service';
import { ConfirmationService } from 'primeng/api';
import { beforeEach, describe, expect, it } from 'vitest';
import { confirmExitGuard } from './confirm-exit.guard';

describe('confirmExitGuard', () => {
  let gateway: FakeGameGateway;
  let confirmation: FakeConfirmationService;

  beforeEach(() => {
    gateway = new FakeGameGateway();
    confirmation = new FakeConfirmationService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        { provide: GAME_REPOSITORY, useValue: new FakeGameRepository() },
        { provide: GAME_GATEWAY, useValue: gateway },
        { provide: SignalrConnectionService, useValue: new FakeSignalrConnectionService() },
        { provide: ConfirmationService, useValue: confirmation }
      ]
    });
  });

  it('should allow deactivation without confirming when there is no game to withdraw from', async () => {
    // Arrange, Act
    const result = await TestBed.runInInjectionContext(() => confirmExitGuard({} as never, {} as never, {} as never, {} as never));

    // Assert
    expect(result).toBe(true);
    expect(confirmation.confirmCalls).toHaveLength(0);
  });

  it('should invoke the real withdraw use case when the player accepts leaving', async () => {
    // Arrange
    await TestBed.inject(GameStore).joinGame('game-1');
    confirmation.respondWith = 'accept';

    // Act
    const result = await TestBed.runInInjectionContext(() => confirmExitGuard({} as never, {} as never, {} as never, {} as never));

    // Assert
    expect(result).toBe(true);
    expect(gateway.withdrawCalls).toEqual([{ gameId: 'game-1' }]);
  });

  it('should block deactivation and not withdraw when the player rejects leaving', async () => {
    // Arrange
    await TestBed.inject(GameStore).joinGame('game-1');
    confirmation.respondWith = 'reject';

    // Act
    const result = await TestBed.runInInjectionContext(() => confirmExitGuard({} as never, {} as never, {} as never, {} as never));

    // Assert
    expect(result).toBe(false);
    expect(gateway.withdrawCalls).toHaveLength(0);
  });
});
