import { animate, query, stagger, style, transition, trigger } from '@angular/animations';

// stagger() only accepts a literal/numeric interval; unlike animate(), it does not support the
// {{param}} interpolation syntax, so only the per-item animation duration is parameterized.
export const staggerAnswers = trigger('staggerAnswers', [
  transition('* => *', [
    query(
      '.answer-option',
      [
        style({ opacity: 0, transform: 'translateY(8px)' }),
        stagger(60, animate('{{duration}}ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })))
      ],
      { optional: true }
    )
  ], { params: { duration: 220 } })
]);
