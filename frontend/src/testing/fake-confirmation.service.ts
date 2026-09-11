export interface FakeConfirmOptions {
  readonly accept?: () => void;
  readonly reject?: () => void;
}

export class FakeConfirmationService {
  respondWith: 'accept' | 'reject' = 'accept';
  readonly confirmCalls: FakeConfirmOptions[] = [];

  confirm(options: FakeConfirmOptions): void {
    this.confirmCalls.push(options);

    if (this.respondWith === 'accept') {
      options.accept?.();
    } else {
      options.reject?.();
    }
  }
}
