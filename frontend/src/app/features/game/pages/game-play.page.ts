import { ChangeDetectionStrategy, Component, computed, effect, ElementRef, inject, signal, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import { SignalrConnectionService } from '@core/realtime/signalr-connection.service';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import {
  SLIDE_QUESTION_FORWARD_PARAMS,
  SlideDirectionParams
} from '@shared/animations/slide-question.animation';
import { slideQuestion } from '@shared/animations/slide-question.animation';
import { GameStore } from '../store/game.store';
import { ConnectionStatusComponent } from '../ui/connection-status/connection-status.component';
import { CountdownBarComponent } from '../ui/countdown-bar/countdown-bar.component';
import { PrizeDisplayComponent } from '../ui/prize-display/prize-display.component';
import { AnswerFeedback, PlayableAnswerView } from '../ui/question-card/question-card.models';
import { QuestionCardComponent } from '../ui/question-card/question-card.component';
import { RoundProgressComponent } from '../ui/round-progress/round-progress.component';
import { WithdrawButtonComponent } from '../ui/withdraw-button/withdraw-button.component';
import { ConfirmationService } from 'primeng/api';

const FEEDBACK_DISPLAY_MS = 600;

interface QuestionSnapshot {
  readonly id: string;
  readonly text: string;
  readonly answers: readonly PlayableAnswerView[];
}

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

@Component({
  selector: 'app-game-play-page',
  imports: [
    RoundProgressComponent,
    PrizeDisplayComponent,
    CountdownBarComponent,
    ConnectionStatusComponent,
    WithdrawButtonComponent,
    QuestionCardComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [slideQuestion],
  templateUrl: './game-play.page.html',
  styleUrl: './game-play.page.scss'
})
export class GamePlayPage {
  protected readonly store = inject(GameStore);
  private readonly connection = inject(SignalrConnectionService);
  private readonly motion = inject(MotionPreferenceService);
  private readonly router = inject(Router);
  private readonly confirmation = inject(ConfirmationService);

  protected readonly connectionState = this.connection.connectionState;
  protected readonly feedback = signal<{ snapshot: QuestionSnapshot; evaluation: AnswerFeedback } | null>(null);

  protected readonly totalRounds = computed(() => this.store.settings()?.totalRounds ?? this.store.game()?.totalRounds ?? 0);
  protected readonly questionTimeLimit = computed(() => this.store.settings()?.questionTimeLimitSeconds ?? 30);
  protected readonly secondsRemaining = computed(() => {
    if (this.store.isGameOver()) {
      return 0;
    }
    return this.store.timeRemaining() ?? this.questionTimeLimit();
  });

  protected readonly currentQuestions = computed<readonly QuestionSnapshot[]>(() => {
    const question = this.store.currentQuestion();
    return question === null
      ? []
      : [{ id: question.id, text: question.text, answers: question.answers.map((answer, index) => ({ ...answer, index: index + 1 })) }];
  });

  protected readonly slideParams = computed<SlideDirectionParams>(() => ({
    ...SLIDE_QUESTION_FORWARD_PARAMS,
    duration: this.motion.reducedMotion() ? 0 : SLIDE_QUESTION_FORWARD_PARAMS.duration
  }));

  private readonly stage = viewChild<ElementRef<HTMLElement>>('stage');

  constructor() {
    effect(() => {
      if (this.store.isGameOver()) {
        const gameId = this.store.game()?.id;
        if (gameId !== undefined) {
          void this.router.navigate(['/results', gameId]);
        }
      }
    });

    effect(() => {
      const round = this.store.game()?.currentRound;
      if (round !== undefined && this.feedback() === null) {
        this.stage()?.nativeElement.focus({ preventScroll: true });
      }
    });
  }

  protected async onAnswerSelected(answerId: string): Promise<void> {
    const question = this.store.currentQuestion();
    if (question === null || this.feedback() !== null) {
      return;
    }

    const snapshot: QuestionSnapshot = {
      id: question.id,
      text: question.text,
      answers: question.answers.map((answer, index) => ({ ...answer, index: index + 1 }))
    };

    await this.store.submitAnswer(answerId);

    const evaluation = this.store.lastEvaluation();
    if (evaluation === null) {
      return;
    }

    this.feedback.set({
      snapshot,
      evaluation: { selectedAnswerId: answerId, correctAnswerId: evaluation.correctAnswerId, isCorrect: evaluation.isCorrect }
    });

    await delay(this.motion.reducedMotion() ? 0 : FEEDBACK_DISPLAY_MS);
    this.feedback.set(null);
  }

  protected onWithdrawClick(): void {
    this.confirmation.confirm({
      header: 'Retirarte de la partida',
      message: 'Conservarás el acumulado actual y la partida terminará. ¿Confirmas?',
      acceptLabel: 'Retirarme',
      rejectLabel: 'Seguir jugando',
      accept: () => void this.store.withdraw()
    });
  }
}
