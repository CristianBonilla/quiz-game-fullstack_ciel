import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { NumberKeyDirective } from '@shared/directives/number-key.directive';

export type AnswerOptionState = 'default' | 'correct' | 'incorrect';

@Component({
  selector: 'app-answer-option',
  imports: [NumberKeyDirective],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './answer-option.component.html',
  styleUrl: './answer-option.component.scss'
})
export class AnswerOptionComponent {
  readonly text = input.required<string>();
  readonly index = input.required<number>();
  readonly selected = input(false);
  readonly disabled = input(false);
  readonly state = input<AnswerOptionState>('default');

  readonly chosen = output<void>();

  protected onNumberKey(): void {
    if (!this.disabled()) {
      this.chosen.emit();
    }
  }
}
