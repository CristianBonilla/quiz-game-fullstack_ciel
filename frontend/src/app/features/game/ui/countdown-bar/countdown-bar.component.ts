import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProgressBarModule } from 'primeng/progressbar';

const CRITICAL_THRESHOLD_SECONDS = 5;

@Component({
  selector: 'app-countdown-bar',
  imports: [ProgressBarModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <p-progressbar
      [value]="percent()"
      [showValue]="false"
      [class.critical]="isCritical()"
      [attr.data-critical]="isCritical()"
      [attr.aria-label]="'Tiempo restante: ' + (secondsRemaining() ?? totalSeconds()) + ' segundos'"
    />
  `,
  styles: `
    :host {
      display: block;
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
    return remaining !== null && remaining <= CRITICAL_THRESHOLD_SECONDS;
  });
}
