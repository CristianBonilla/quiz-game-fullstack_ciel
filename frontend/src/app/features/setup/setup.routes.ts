import { Routes } from '@angular/router';

export const setupRoutes: Routes = [
  { path: '', loadComponent: () => import('./pages/setup-page.component').then((m) => m.SetupPageComponent) }
];
