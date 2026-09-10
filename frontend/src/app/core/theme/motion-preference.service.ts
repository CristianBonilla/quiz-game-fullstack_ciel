import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class MotionPreferenceService {
  private readonly document = inject(DOCUMENT);
  private readonly reduced = signal(this.matchMedia()?.matches ?? false);

  readonly reducedMotion = this.reduced.asReadonly();

  constructor() {
    this.matchMedia()?.addEventListener('change', (event) => this.reduced.set(event.matches));
  }

  private matchMedia(): MediaQueryList | undefined {
    return typeof this.document.defaultView?.matchMedia === 'function'
      ? this.document.defaultView.matchMedia('(prefers-reduced-motion: reduce)')
      : undefined;
  }
}
