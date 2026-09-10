import { Game } from '../models/game';
import { GameSettings } from '../models/game-settings';
import { GameSummary } from '../models/game-summary';
import { StartGameRequest } from '../models/requests';

export interface GameRepository {
  startGame(request: StartGameRequest): Promise<Game>;
  getGameState(gameId: string): Promise<Game>;
  getGameSummary(gameId: string): Promise<GameSummary>;
  getSettings(): Promise<GameSettings>;
}
