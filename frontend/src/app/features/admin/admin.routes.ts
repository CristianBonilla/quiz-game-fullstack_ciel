import { Routes } from '@angular/router';
import { CATEGORY_REPOSITORY, QUESTION_REPOSITORY } from '@core/config/injection-tokens';
import { HttpCategoryRepository } from '@data-access/api/http-category.repository';
import { HttpQuestionRepository } from '@data-access/api/http-question.repository';

export const adminRoutes: Routes = [
  {
    path: '',
    providers: [
      { provide: CATEGORY_REPOSITORY, useClass: HttpCategoryRepository },
      { provide: QUESTION_REPOSITORY, useClass: HttpQuestionRepository }
    ],
    loadComponent: () => import('./pages/admin-page.component').then((m) => m.AdminPageComponent)
  }
];
