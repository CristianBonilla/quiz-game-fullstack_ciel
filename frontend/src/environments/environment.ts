import { AppConfig } from '@core/config/app-config.token';

export const environment: AppConfig = {
  apiBaseUrl: 'http://localhost:5000/api',
  hubUrl: 'http://localhost:5000/hubs',
  reconnectDelays: [0, 2000, 5000, 10000, 30000],
  defaultTheme: 'light'
};
