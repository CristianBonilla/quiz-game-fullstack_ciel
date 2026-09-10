import { ErrorHandler, inject, Injectable, isDevMode } from '@angular/core';
import { toAppError } from '@domain/models/app-error';
import { NotificationService } from './notification.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private readonly notifications = inject(NotificationService);

  handleError(error: unknown): void {
    if (isDevMode()) {
      console.error(error);
    }

    this.notifications.showError(toAppError(error));
  }
}
