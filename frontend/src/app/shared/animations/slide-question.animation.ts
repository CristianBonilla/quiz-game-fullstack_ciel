import { animate, AnimationTransitionMetadata, group, query, style, transition, trigger } from '@angular/animations';

export interface SlideDirectionParams {
  readonly duration: number;
  readonly enterX: string;
  readonly leaveX: string;
}

export const SLIDE_QUESTION_FORWARD_PARAMS: SlideDirectionParams = { duration: 320, enterX: '100%', leaveX: '-100%' };
export const SLIDE_QUESTION_BACKWARD_PARAMS: SlideDirectionParams = { duration: 320, enterX: '-100%', leaveX: '100%' };

function slideSteps(): AnimationTransitionMetadata['animation'] {
  return [
    style({ position: 'relative' }),
    query(':enter, :leave', [style({ position: 'absolute', inset: 0, width: '100%' })], { optional: true }),
    query(':enter', [style({ transform: 'translateX({{enterX}})', opacity: 0 })], { optional: true }),
    group([
      query(
        ':leave',
        [animate('{{duration}}ms cubic-bezier(0.4, 0, 0.2, 1)', style({ transform: 'translateX({{leaveX}})', opacity: 0 }))],
        { optional: true }
      ),
      query(
        ':enter',
        [animate('{{duration}}ms cubic-bezier(0.4, 0, 0.2, 1)', style({ transform: 'translateX(0)', opacity: 1 }))],
        { optional: true }
      )
    ])
  ];
}

/**
 * The server drives this transition (RoundAdvanced/GameEnded), never the click itself. Forward and
 * backward reuse the same step sequence and differ only through bound params, so the direction is
 * data, not duplicated animation code.
 */
export const slideQuestion = trigger('slideQuestion', [
  transition(':increment', slideSteps(), { params: SLIDE_QUESTION_FORWARD_PARAMS }),
  transition(':decrement', slideSteps(), { params: SLIDE_QUESTION_BACKWARD_PARAMS })
]);
