import {
  ApplicationConfig,
  ErrorHandler,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection
} from '@angular/core';
import { provideRouter, withComponentInputBinding, withViewTransitions } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ConfirmationService, MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';

import { routes } from './app.routes';
import { APP_CONFIG } from './core/config/app-config.token';
import { GAME_GATEWAY, GAME_REPOSITORY } from './core/config/injection-tokens';
import { GlobalErrorHandler } from './core/error/global-error-handler';
import { correlationIdInterceptor } from './core/http/correlation-id.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';
import { loadingInterceptor } from './core/http/loading.interceptor';
import { HttpGameRepository } from './data-access/api/http-game.repository';
import { SignalrGameGateway } from './data-access/realtime/signalr-game.gateway';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    // Zoneless: SignalR callbacks arrive outside Angular's zone; signals notify the framework directly without NgZone.run.
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding(), withViewTransitions()),
    provideHttpClient(
      withInterceptors([correlationIdInterceptor, loadingInterceptor, errorInterceptor]),
      withFetch()
    ),
    provideAnimations(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.app-dark',
          cssLayer: { name: 'primeng', order: 'theme, base, primeng' }
        }
      }
    }),
    MessageService,
    ConfirmationService,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: GAME_REPOSITORY, useClass: HttpGameRepository },
    { provide: GAME_GATEWAY, useClass: SignalrGameGateway },
    { provide: APP_CONFIG, useValue: environment }
  ]
};

