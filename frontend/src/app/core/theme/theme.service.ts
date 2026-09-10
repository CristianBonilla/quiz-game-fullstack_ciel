import { DOCUMENT } from '@angular/common';
import { inject, Injectable, Signal, signal } from '@angular/core';
import { APP_CONFIG, ThemePreference } from '../config/app-config.token';

const STORAGE_KEY = 'quiz-game.theme';
const DARK_CLASS = 'app-dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly config = inject(APP_CONFIG);
  private readonly dark = signal(false);

  readonly isDark: Signal<boolean> = this.dark.asReadonly();

  initialize(): void {
    this.apply(this.resolveInitialPreference());
  }

  toggle(): void {
    this.apply(this.dark() ? 'light' : 'dark');
  }

  apply(preference: ThemePreference): void {
    const isDark = preference === 'dark';
    this.dark.set(isDark);
    this.document.documentElement.classList.toggle(DARK_CLASS, isDark);
    this.readStorage()?.setItem(STORAGE_KEY, preference);
  }

  private resolveInitialPreference(): ThemePreference {
    const stored = this.readStorage()?.getItem(STORAGE_KEY);
    if (stored === 'dark' || stored === 'light') {
      return stored;
    }

    const prefersDark = this.matchMedia('(prefers-color-scheme: dark)')?.matches ?? false;
    return prefersDark ? 'dark' : this.config.defaultTheme;
  }

  private matchMedia(query: string): MediaQueryList | null {
    return typeof this.document.defaultView?.matchMedia === 'function'
      ? this.document.defaultView.matchMedia(query)
      : null;
  }

  private readStorage(): Storage | null {
    return this.document.defaultView?.localStorage ?? null;
  }
}
