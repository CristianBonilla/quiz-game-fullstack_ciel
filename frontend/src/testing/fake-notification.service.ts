export class FakeNotificationService {
  readonly errors: string[] = [];
  readonly warnings: string[] = [];

  showError(error: { message: string }): void {
    this.errors.push(error.message);
  }

  showWarning(detail: string): void {
    this.warnings.push(detail);
  }

  showSuccess(detail: string): void {
    this.warnings.push(detail);
  }

  showInfo(detail: string): void {
    this.warnings.push(detail);
  }
}
