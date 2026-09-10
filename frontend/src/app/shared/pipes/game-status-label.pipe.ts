import { Pipe, PipeTransform } from '@angular/core';
import { GameStatus } from '@domain/enums/game-status';

const LABELS: Record<GameStatus, string> = {
  NotStarted: 'Sin iniciar',
  InProgress: 'En curso',
  Won: 'Ganada',
  Lost: 'Perdida',
  Withdrawn: 'Retirada',
  ForcedEnd: 'Finalizada'
};

@Pipe({ name: 'gameStatusLabel' })
export class GameStatusLabelPipe implements PipeTransform {
  transform(status: GameStatus): string {
    return LABELS[status];
  }
}
