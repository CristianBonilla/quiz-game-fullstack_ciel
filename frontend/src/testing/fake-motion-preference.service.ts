import { signal } from '@angular/core';

export class FakeMotionPreferenceService {
  private readonly reduced = signal(true);

  readonly reducedMotion = this.reduced.asReadonly();

  setReducedMotion(value: boolean): void {
    this.reduced.set(value);
  }
}
