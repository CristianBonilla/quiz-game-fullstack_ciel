import { Routes } from '@angular/router';
import { activeGameGuard } from '@core/guards/active-game.guard';
import { confirmExitGuard } from '@core/guards/confirm-exit.guard';
import { finishedGameGuard } from '@core/guards/finished-game.guard';
import { gameConfiguredGuard } from '@core/guards/game-configured.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'setup', pathMatch: 'full' },
  {
    path: 'setup',
    loadChildren: () => import('@features/setup/setup.routes').then((m) => m.setupRoutes),
    canActivate: [gameConfiguredGuard]
  },
  {
    path: 'game',
    loadChildren: () => import('@features/game/game.routes').then((m) => m.gameRoutes),
    canActivate: [activeGameGuard],
    canDeactivate: [confirmExitGuard]
  },
  {
    path: 'admin',
    loadChildren: () => import('@features/admin/admin.routes').then((m) => m.adminRoutes)
  },
  {
    path: 'results/:id',
    loadChildren: () => import('@features/results/results.routes').then((m) => m.resultsRoutes),
    canActivate: [finishedGameGuard]
  },
  { path: '**', loadComponent: () => import('@shared/ui/not-found/not-found.component') }
];
