import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink, ButtonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="not-found">
      <h1>404</h1>
      <p>Esta página no existe.</p>
      <p-button label="Ir al inicio" routerLink="/setup" />
    </section>
  `,
  styles: `
    .not-found {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 1rem;
      min-height: 60vh;
      text-align: center;
      color: var(--p-text-color);
    }
  `
})
export default class NotFoundComponent {}
