import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { staggerAnswers } from '@shared/animations/stagger-answers.animation';
import { AnswerOptionComponent, AnswerOptionState } from '../answer-option/answer-option.component';
import { AnswerFeedback, PlayableAnswerView } from './question-card.models';

@Component({
  selector: 'app-question-card',
  imports: [AnswerOptionComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [staggerAnswers],
  templateUrl: './question-card.component.html',
  styleUrl: './question-card.component.scss'
})
export class QuestionCardComponent {
  readonly questionText = input.required<string>();
  readonly answers = input.required<readonly PlayableAnswerView[]>();
  readonly disabled = input(false);
  readonly feedback = input<AnswerFeedback | null>(null);
  readonly animateStagger = input(true);

  readonly answerSelected = output<string>();

  private readonly motion = inject(MotionPreferenceService);

  protected stateFor(answerId: string): AnswerOptionState {
    const feedback = this.feedback();
    if (feedback === null) {
      return 'default';
    }

    if (answerId === feedback.correctAnswerId) {
      return 'correct';
    }

    return answerId === feedback.selectedAnswerId ? 'incorrect' : 'default';
  }

  protected isSelected(answerId: string): boolean {
    return this.feedback()?.selectedAnswerId === answerId;
  }

  protected readonly staggerKey = computed(() => {
    if (!this.animateStagger()) {
      return { value: 'disabled', params: { duration: 0 } };
    }
    return {
      value: `${this.questionText()}-${this.answers().length}`,
      params: { duration: this.motion.reducedMotion() ? 0 : 220 }
    };
  });
}
