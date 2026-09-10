import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, UrlTree } from '@angular/router';
import { GAME_REPOSITORY } from '@core/config/injection-tokens';
import { isTerminalStatus } from '@domain/rules/game-rules';

export const finishedGameGuard: CanActivateFn = async (route: ActivatedRouteSnapshot): Promise<boolean | UrlTree> => {
  const repository = inject(GAME_REPOSITORY);
  const router = inject(Router);
  const gameId = route.paramMap.get('id');

  if (gameId === null) {
    return router.createUrlTree(['/setup']);
  }

  try {
    const summary = await repository.getGameSummary(gameId);
    return isTerminalStatus(summary.status) ? true : router.createUrlTree(['/game/play']);
  } catch {
    return router.createUrlTree(['/setup']);
  }
};
