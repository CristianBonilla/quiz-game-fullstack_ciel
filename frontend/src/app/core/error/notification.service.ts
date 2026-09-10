import { inject, Injectable } from '@angular/core';
import { AppError } from '@domain/models/app-error';
import { MessageService } from 'primeng/api';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly messages = inject(MessageService);

  showError(error: AppError): void {
    this.messages.add({ severity: 'error', summary: error.code, detail: error.message, life: 6000 });
  }

  showWarning(detail: string, summary = 'Warning'): void {
    this.messages.add({ severity: 'warn', summary, detail, life: 5000 });
  }

  showSuccess(detail: string, summary = 'Success'): void {
    this.messages.add({ severity: 'success', summary, detail, life: 4000 });
  }

  showInfo(detail: string, summary = 'Info'): void {
    this.messages.add({ severity: 'info', summary, detail, life: 4000 });
  }
}
