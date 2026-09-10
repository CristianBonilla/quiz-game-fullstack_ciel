import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { GameStore } from '@features/game/store/game.store';

export const activeGameGuard: CanActivateFn = (): boolean | UrlTree => {
  const store = inject(GameStore);
  const router = inject(Router);
  const game = store.game();

  return game !== null && game.status === 'InProgress' ? true : router.createUrlTree(['/setup']);
};
