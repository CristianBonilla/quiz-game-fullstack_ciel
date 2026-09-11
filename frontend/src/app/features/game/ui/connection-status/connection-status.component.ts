import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ConnectionState } from '@core/realtime/connection-state';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-connection-status',
  imports: [TagModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (state() !== 'connected') {
      <p-tag [severity]="severity()" [value]="label()" aria-live="polite" />
    }
  `
})
export class ConnectionStatusComponent {
  readonly state = input.required<ConnectionState>();

  protected readonly label = computed(() => {
    switch (this.state()) {
      case 'connecting':
        return 'Conectando…';
      case 'reconnecting':
        return 'Reconectando…';
      case 'disconnected':
        return 'Sin conexión';
      default:
        return '';
    }
  });

  protected readonly severity = computed(() => (this.state() === 'disconnected' ? 'danger' : 'warn'));
}
