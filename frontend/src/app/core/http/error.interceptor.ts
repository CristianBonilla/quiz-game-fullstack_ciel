import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { toAppErrorFromHttp } from '@data-access/mappers/app-error.mapper';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../error/notification.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const notifications = inject(NotificationService);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      const appError = toAppErrorFromHttp(error);
      notifications.showError(appError);

      return throwError(() => appError);
    })
  );
};
