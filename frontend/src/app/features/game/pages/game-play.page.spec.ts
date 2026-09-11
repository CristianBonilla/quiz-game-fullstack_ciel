import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { provideRouter, Router } from '@angular/router';
import { GAME_GATEWAY, GAME_REPOSITORY } from '@core/config/injection-tokens';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { FakeConfirmationService } from '@testing/fake-confirmation.service';
import { FakeGameGateway } from '@testing/fake-game-gateway';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { FakeMotionPreferenceService } from '@testing/fake-motion-preference.service';
import { FakeSignalrConnectionService } from '@testing/fake-signalr-connection.service';
import { ConfirmationService } from 'primeng/api';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { GameStore } from '../store/game.store';
import { GamePlayPage } from './game-play.page';

describe('GamePlayPage', () => {
  let fixture: ComponentFixture<GamePlayPage>;
  let gateway: FakeGameGateway;
  let confirmation: FakeConfirmationService;
  let router: Router;

  beforeEach(async () => {
    gateway = new FakeGameGateway();
    confirmation = new FakeConfirmationService();

    await TestBed.configureTestingModule({
      imports: [GamePlayPage],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        provideRouter([]),
        { provide: GAME_REPOSITORY, useValue: new FakeGameRepository() },
        { provide: GAME_GATEWAY, useValue: gateway },
        { provide: SignalrConnectionService, useValue: new FakeSignalrConnectionService() },
        { provide: MotionPreferenceService, useValue: new FakeMotionPreferenceService() },
        { provide: ConfirmationService, useValue: confirmation }
      ]
    }).compileComponents();

    await TestBed.inject(GameStore).joinGame('game-1');
    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(GamePlayPage);
    fixture.detectChanges();
  });

  it('should invoke the withdraw use case through the gateway when the confirmation is accepted', () => {
    // Arrange
    confirmation.respondWith = 'accept';

    // Act
    fixture.componentInstance['onWithdrawClick']();

    // Assert
    expect(gateway.withdrawCalls).toEqual([{ gameId: 'game-1' }]);
  });

  it('should not withdraw when the confirmation is rejected', () => {
    // Arrange
    confirmation.respondWith = 'reject';

    // Act
    fixture.componentInstance['onWithdrawClick']();

    // Assert
    expect(gateway.withdrawCalls).toHaveLength(0);
  });

  it('should navigate to the results page once the game is over', async () => {
    // Arrange
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    // Act
    gateway.emit({
      type: 'GameEnded',
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'Won',
      finalPrize: 2600,
      endedOnUtc: new Date('2026-01-01T10:10:00.000Z')
    });
    fixture.detectChanges();

    // Assert
    expect(navigateSpy).toHaveBeenCalledWith(['/results', 'game-1']);
  });
});
