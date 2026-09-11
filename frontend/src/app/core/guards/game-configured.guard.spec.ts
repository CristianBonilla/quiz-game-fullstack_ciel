import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';
import { GAME_REPOSITORY } from '@core/config/injection-tokens';
import { NotificationService } from '@core/error/notification.service';
import { FakeGameRepository } from '@testing/fake-game-repository';
import { FakeNotificationService } from '@testing/fake-notification.service';
import { aGameSettings, aRoundConfiguration } from '@testing/game-settings.builder';
import { beforeEach, describe, expect, it } from 'vitest';
import { gameConfiguredGuard } from './game-configured.guard';

describe('gameConfiguredGuard', () => {
  let repository: FakeGameRepository;

  beforeEach(() => {
    repository = new FakeGameRepository();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        { provide: GAME_REPOSITORY, useValue: repository },
        { provide: NotificationService, useValue: new FakeNotificationService() }
      ]
    });
  });

  it('should allow activation when every round has enough active questions', async () => {
    // Arrange
    repository.settings = aGameSettings({
      totalRounds: 1,
      rounds: [aRoundConfiguration({ availableQuestions: 5 })]
    });

    // Act
    const result = await TestBed.runInInjectionContext(() => gameConfiguredGuard({} as never, {} as never));

    // Assert
    expect(result).toBe(true);
  });

  it('should redirect to /admin when a round has fewer than the minimum questions', async () => {
    // Arrange
    repository.settings = aGameSettings({
      totalRounds: 1,
      rounds: [aRoundConfiguration({ availableQuestions: 4 })]
    });

    // Act
    const result = await TestBed.runInInjectionContext(() => gameConfiguredGuard({} as never, {} as never));

    // Assert
    expect(result).toBeInstanceOf(UrlTree);
    expect((result as UrlTree).toString()).toBe(TestBed.inject(Router).createUrlTree(['/admin']).toString());
  });
});
