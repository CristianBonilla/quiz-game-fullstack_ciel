import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { fadeInUp } from '@shared/animations/fade-in-up.animation';

@Component({
  selector: 'app-round-progress',
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [fadeInUp],
  template: `
    <div class="round-progress" [@fadeInUp]="{ value: true, params: { duration: motionDuration() } }" role="img" [attr.aria-label]="label()">
      @for (segment of segments(); track $index) {
        <span class="round-progress__dot" [class.filled]="$index < currentRound()" [attr.data-filled]="$index < currentRound()"></span>
      }
    </div>
  `,
  styles: `
    .round-progress {
      display: flex;
      gap: 0.375rem;
    }

    .round-progress__dot {
      width: 0.5rem;
      height: 0.5rem;
      border-radius: 50%;
      background: var(--p-surface-200);
    }

    .round-progress__dot.filled {
      background: var(--p-primary-color);
    }
  `
})
export class RoundProgressComponent {
  readonly currentRound = input.required<number>();
  readonly totalRounds = input.required<number>();

  private readonly motion = inject(MotionPreferenceService);
  protected readonly motionDuration = computed(() => (this.motion.reducedMotion() ? 0 : 280));

  protected readonly segments = computed(() => Array.from({ length: this.totalRounds() }));
  protected readonly label = computed(() => `Ronda ${this.currentRound()} de ${this.totalRounds()}`);
}
