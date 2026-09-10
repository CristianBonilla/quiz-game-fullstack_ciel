import { InjectionToken } from '@angular/core';
import { CategoryRepository } from '@domain/contracts/category-repository';
import { GameGateway } from '@domain/contracts/game-gateway';
import { GameRepository } from '@domain/contracts/game-repository';
import { QuestionRepository } from '@domain/contracts/question-repository';

export const GAME_REPOSITORY = new InjectionToken<GameRepository>('GameRepository');
export const CATEGORY_REPOSITORY = new InjectionToken<CategoryRepository>('CategoryRepository');
export const QUESTION_REPOSITORY = new InjectionToken<QuestionRepository>('QuestionRepository');
export const GAME_GATEWAY = new InjectionToken<GameGateway>('GameGateway');
