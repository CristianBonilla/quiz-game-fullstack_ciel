import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { GameStore } from '@features/game/store/game.store';
import { ConfirmationService } from 'primeng/api';

/** Voluntary withdrawal expressed as a guard: accepting runs the real use case, not just a prompt. */
export const confirmExitGuard: CanDeactivateFn<unknown> = (): boolean | Promise<boolean> => {
  const store = inject(GameStore);
  const confirmation = inject(ConfirmationService);

  if (!store.canWithdraw()) {
    return true;
  }

  return new Promise<boolean>((resolve) => {
    confirmation.confirm({
      header: 'Leave the game?',
      message: 'You can withdraw now and keep the prize you have accumulated, or stay and keep playing.',
      acceptLabel: 'Withdraw and keep the prize',
      rejectLabel: 'Keep playing',
      accept: () => {
        void store.withdraw().then(() => resolve(store.error() === null));
      },
      reject: () => resolve(false)
    });
  });
};
