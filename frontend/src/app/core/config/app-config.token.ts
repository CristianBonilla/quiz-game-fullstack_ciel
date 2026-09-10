import { InjectionToken } from '@angular/core';

export type ThemePreference = 'light' | 'dark';

export interface AppConfig {
  readonly apiBaseUrl: string;
  readonly hubUrl: string;
  readonly reconnectDelays: readonly number[];
  readonly defaultTheme: ThemePreference;
}

export const APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG');
