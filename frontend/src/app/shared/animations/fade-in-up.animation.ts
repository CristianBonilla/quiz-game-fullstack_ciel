import { animate, style, transition, trigger } from '@angular/animations';

export const fadeInUp = trigger('fadeInUp', [
  transition(':enter', [
    style({ opacity: 0, transform: 'translateY(12px)' }),
    animate('{{duration}}ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
  ], { params: { duration: 280 } })
]);
