import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProgressBarModule } from 'primeng/progressbar';

const CRITICAL_THRESHOLD_SECONDS = 5;

@Component({
  selector: 'app-countdown-bar',
  imports: [ProgressBarModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="countdown-bar">
      <p-progressbar
        [value]="percent()"
        [showValue]="false"
        [class.critical]="isCritical()"
        [attr.data-critical]="isCritical()"
        [attr.aria-label]="'Tiempo restante: ' + (secondsRemaining() ?? totalSeconds()) + ' segundos'"
      />
      <span class="countdown-bar__seconds" [class.critical]="isCritical()" aria-hidden="true">
        {{ secondsRemaining() ?? totalSeconds() }}s
      </span>
    </div>
  `,
  styles: `
    :host {
      display: block;
    }

    .countdown-bar {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      min-width: 8rem;
    }

    .countdown-bar p-progressbar {
      flex: 1;
    }

    .countdown-bar__seconds {
      font-variant-numeric: tabular-nums;
      font-size: 0.8125rem;
      font-weight: 600;
      color: var(--p-text-muted-color);
      min-width: 2rem;
      text-align: right;
    }

    .countdown-bar__seconds.critical {
      color: var(--p-red-500, #ef4444);
    }

    :host ::ng-deep .critical .p-progressbar-value {
      animation: countdown-pulse 1s ease-in-out infinite;
    }

    @keyframes countdown-pulse {
      0%,
      100% {
        opacity: 1;
      }
      50% {
        opacity: 0.6;
      }
    }
  `
})
export class CountdownBarComponent {
  readonly secondsRemaining = input<number | null>(null);
  readonly totalSeconds = input.required<number>();

  protected readonly percent = computed(() => {
    const remaining = this.secondsRemaining() ?? this.totalSeconds();
    const total = this.totalSeconds();
    return total <= 0 ? 0 : Math.max(0, Math.min(100, Math.round((remaining / total) * 100)));
  });

  protected readonly isCritical = computed(() => {
    const remaining = this.secondsRemaining();
    return remaining !== null && remaining > 0 && remaining <= CRITICAL_THRESHOLD_SECONDS;
  });
}
