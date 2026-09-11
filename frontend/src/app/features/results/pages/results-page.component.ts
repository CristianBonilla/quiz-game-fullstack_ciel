import { ChangeDetectionStrategy, Component, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GAME_REPOSITORY } from '@core/config/injection-tokens';
import { GameSummary } from '@domain/models/game-summary';
import { formatPrize } from '@shared/utils/format-prize';
import { GameStatusLabelPipe } from '@shared/pipes/game-status-label.pipe';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { SkeletonModule } from 'primeng/skeleton';
import { TagModule } from 'primeng/tag';

const STATUS_MESSAGES: Record<GameSummary['status'], string> = {
  NotStarted: '',
  InProgress: '',
  Won: '¡Completaste las 5 rondas! Ganaste el premio mayor.',
  Lost: 'Fallaste una respuesta y perdiste el acumulado.',
  Withdrawn: 'Te retiraste y conservaste tu acumulado.',
  ForcedEnd: 'La partida terminó de forma forzada.'
};

@Component({
  selector: 'app-results-page',
  imports: [RouterLink, ButtonModule, CardModule, MessageModule, SkeletonModule, TagModule, GameStatusLabelPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './results-page.component.html',
  styleUrl: './results-page.component.scss'
})
export class ResultsPageComponent implements OnInit {
  readonly id = input.required<string>();

  private readonly repository = inject(GAME_REPOSITORY);

  protected readonly summary = signal<GameSummary | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly formatPrize = formatPrize;

  async ngOnInit(): Promise<void> {
    try {
      const summary = await this.repository.getGameSummary(this.id());
      this.summary.set(summary);
    } catch {
      this.loadError.set('No se pudo cargar el resumen de la partida.');
    }
  }

  protected messageFor(summary: GameSummary): string {
    return STATUS_MESSAGES[summary.status];
  }
}
