import { ChangeDetectionStrategy, Component, effect, inject, input, signal } from '@angular/core';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { formatPrize } from '@shared/utils/format-prize';

const ANIMATION_DURATION_MS = 500;

@Component({
  selector: 'app-prize-display',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="prize-display">
      <span class="prize-display__label">Acumulado</span>
      <span class="prize-display__value" data-testid="prize-value" aria-live="polite">{{ formattedValue() }}</span>
    </div>
  `,
  styles: `
    .prize-display {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      line-height: 1.1;
    }

    .prize-display__label {
      font-size: 0.75rem;
      color: var(--p-text-muted-color);
    }

    .prize-display__value {
      font-size: 1.25rem;
      font-weight: 700;
      color: var(--p-primary-color);
    }
  `
})
export class PrizeDisplayComponent {
  readonly amount = input.required<number>();

  private readonly motion = inject(MotionPreferenceService);
  private readonly displayed = signal(0);
  protected readonly formattedValue = signal(formatPrize(0));

  private animationFrame: number | null = null;

  constructor() {
    effect((onCleanup) => {
      const target = this.amount();

      if (this.motion.reducedMotion()) {
        this.displayed.set(target);
        this.formattedValue.set(formatPrize(target));
        return;
      }

      const start = this.displayed();
      const startedAt = performance.now();

      const step = (now: number): void => {
        const progress = Math.min((now - startedAt) / ANIMATION_DURATION_MS, 1);
        const current = Math.round(start + (target - start) * progress);
        this.displayed.set(current);
        this.formattedValue.set(formatPrize(current));

        if (progress < 1) {
          this.animationFrame = requestAnimationFrame(step);
        }
      };

      this.animationFrame = requestAnimationFrame(step);

      onCleanup(() => {
        if (this.animationFrame !== null) {
          cancelAnimationFrame(this.animationFrame);
        }
      });
    });
  }
}
