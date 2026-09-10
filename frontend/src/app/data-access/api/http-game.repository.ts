import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import { GameRepository } from '@domain/contracts/game-repository';
import { Game } from '@domain/models/game';
import { GameSettings } from '@domain/models/game-settings';
import { GameSummary } from '@domain/models/game-summary';
import { StartGameRequest } from '@domain/models/requests';
import { firstValueFrom } from 'rxjs';
import { GameConfigurationDto, GameStateDto, GameSummaryDto } from '../dto/game.dto';
import { toGame, toGameSettings, toGameSummary, toStartGameRequestDto } from '../mappers/game.mapper';
import { IDEMPOTENCY_KEY_HEADER } from './idempotency';

@Injectable()
export class HttpGameRepository implements GameRepository {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(APP_CONFIG).apiBaseUrl}/v1/games`;

  async startGame(request: StartGameRequest): Promise<Game> {
    const dto = await firstValueFrom(
      this.http.post<GameStateDto>(this.baseUrl, toStartGameRequestDto(request), {
        headers: { [IDEMPOTENCY_KEY_HEADER]: crypto.randomUUID() }
      })
    );

    return toGame(dto);
  }

  async getGameState(gameId: string): Promise<Game> {
    const dto = await firstValueFrom(this.http.get<GameStateDto>(`${this.baseUrl}/${gameId}`));
    return toGame(dto);
  }

  async getGameSummary(gameId: string): Promise<GameSummary> {
    const dto = await firstValueFrom(this.http.get<GameSummaryDto>(`${this.baseUrl}/${gameId}/summary`));
    return toGameSummary(dto);
  }

  async getSettings(): Promise<GameSettings> {
    const dto = await firstValueFrom(this.http.get<GameConfigurationDto>(`${this.baseUrl}/settings`));
    return toGameSettings(dto);
  }
}
