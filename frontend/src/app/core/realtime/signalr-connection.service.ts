import { inject, Injectable, isDevMode, Signal, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { APP_CONFIG } from '../config/app-config.token';
import { ConnectionState } from './connection-state';

/** Owns the transport only: it knows nothing about rounds, prizes or answers. */
@Injectable({ providedIn: 'root' })
export class SignalrConnectionService {
  private readonly config = inject(APP_CONFIG);
  private readonly state = signal<ConnectionState>('disconnected');
  private connection: HubConnection | null = null;

  readonly connectionState: Signal<ConnectionState> = this.state.asReadonly();

  buildConnection(): HubConnection {
    if (this.connection !== null) {
      return this.connection;
    }

    const connection = new HubConnectionBuilder()
      .withUrl(`${this.config.hubUrl}/game`)
      .withAutomaticReconnect([...this.config.reconnectDelays])
      .configureLogging(isDevMode() ? LogLevel.Information : LogLevel.Warning)
      .build();

    connection.onreconnecting(() => this.state.set('reconnecting'));
    connection.onreconnected(() => this.state.set('connected'));
    connection.onclose(() => this.state.set('disconnected'));

    this.connection = connection;
    return connection;
  }

  async start(): Promise<void> {
    const connection = this.buildConnection();
    if (connection.state !== HubConnectionState.Disconnected) {
      return;
    }

    this.state.set('connecting');

    try {
      await connection.start();
      this.state.set('connected');
    } catch (error) {
      this.state.set('disconnected');
      throw error;
    }
  }

  async stop(): Promise<void> {
    if (this.connection === null) {
      return;
    }

    await this.connection.stop();
    this.state.set('disconnected');
  }

  on(methodName: string, handler: (payload: unknown) => void): void {
    this.buildConnection().on(methodName, handler);
  }

  async invoke<TResult>(methodName: string, ...args: readonly unknown[]): Promise<TResult> {
    await this.start();
    return this.buildConnection().invoke<TResult>(methodName, ...args);
  }
}
