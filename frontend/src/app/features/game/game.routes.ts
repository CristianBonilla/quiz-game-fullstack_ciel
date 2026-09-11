import { Routes } from '@angular/router';

export const gameRoutes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'play' },
  { path: 'play', loadComponent: () => import('./pages/game-play.page').then((m) => m.GamePlayPage) }
];
