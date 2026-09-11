import { Routes } from '@angular/router';

export const resultsRoutes: Routes = [
  { path: '', loadComponent: () => import('./pages/results-page.component').then((m) => m.ResultsPageComponent) }
];
