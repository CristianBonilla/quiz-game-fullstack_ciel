import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { GAME_REPOSITORY } from '@core/config/injection-tokens';
import { NotificationService } from '@core/error/notification.service';
import { isGameConfigured, MINIMUM_QUESTIONS_PER_CATEGORY } from '@domain/rules/game-rules';

export const gameConfiguredGuard: CanActivateFn = async (): Promise<boolean | UrlTree> => {
  const repository = inject(GAME_REPOSITORY);
  const notifications = inject(NotificationService);
  const router = inject(Router);

  try {
    const settings = await repository.getSettings();
    if (isGameConfigured(settings)) {
      return true;
    }

    notifications.showWarning(
      `Every round needs an active category with at least ${MINIMUM_QUESTIONS_PER_CATEGORY} questions.`,
      'Game not configured'
    );
  } catch {
    notifications.showWarning('The game configuration could not be loaded.', 'Game not configured');
  }

  return router.createUrlTree(['/admin']);
};
