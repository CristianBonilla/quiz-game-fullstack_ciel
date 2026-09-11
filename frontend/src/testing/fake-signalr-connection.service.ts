import { signal } from '@angular/core';
import { ConnectionState } from '@core/realtime/connection-state';

export class FakeSignalrConnectionService {
  private readonly state = signal<ConnectionState>('disconnected');

  readonly connectionState = this.state.asReadonly();

  setState(value: ConnectionState): void {
    this.state.set(value);
  }
}
