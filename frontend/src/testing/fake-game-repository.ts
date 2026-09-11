import { GameRepository } from '@domain/contracts/game-repository';
import { Game } from '@domain/models/game';
import { GameSettings } from '@domain/models/game-settings';
import { GameSummary } from '@domain/models/game-summary';
import { StartGameRequest } from '@domain/models/requests';
import { aGame } from './game.builder';
import { aGameSettings } from './game-settings.builder';
import { aGameSummary } from './game-summary.builder';

export class FakeGameRepository implements GameRepository {
  settings: GameSettings = aGameSettings();
  gameState: Game = aGame();
  gameSummary: GameSummary = aGameSummary();
  shouldFail = false;

  readonly getGameStateCalls: string[] = [];
  readonly getGameSummaryCalls: string[] = [];
  readonly startGameCalls: StartGameRequest[] = [];

  async startGame(request: StartGameRequest): Promise<Game> {
    this.startGameCalls.push(request);
    if (this.shouldFail) {
      throw new Error('startGame failed');
    }

    return { ...this.gameState, playerName: request.playerName };
  }

  async getGameState(gameId: string): Promise<Game> {
    this.getGameStateCalls.push(gameId);
    if (this.shouldFail) {
      throw new Error('getGameState failed');
    }

    return { ...this.gameState, id: gameId };
  }

  async getGameSummary(gameId: string): Promise<GameSummary> {
    this.getGameSummaryCalls.push(gameId);
    if (this.shouldFail) {
      throw new Error('getGameSummary failed');
    }

    return { ...this.gameSummary, id: gameId };
  }

  async getSettings(): Promise<GameSettings> {
    if (this.shouldFail) {
      throw new Error('getSettings failed');
    }

    return this.settings;
  }
}
