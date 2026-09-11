import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { NonNullableFormBuilder } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { GameStore } from '@features/game/store/game.store';
import { formatPrize } from '@shared/utils/format-prize';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'app-setup-page',
  imports: [ReactiveFormsModule, RouterLink, ButtonModule, CardModule, InputTextModule, MessageModule, SkeletonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './setup-page.component.html',
  styleUrl: './setup-page.component.scss'
})
export class SetupPageComponent {
  protected readonly store = inject(GameStore);
  protected readonly formatPrize = formatPrize;
  private readonly router = inject(Router);
  private readonly formBuilder = inject(NonNullableFormBuilder);

  protected readonly form = this.formBuilder.group({
    playerName: this.formBuilder.control('', [Validators.required, Validators.minLength(2), Validators.maxLength(50)])
  });

  protected readonly isStarting = signal(false);

  protected get playerNameErrorMessage(): string {
    const control = this.form.controls.playerName;
    if (control.hasError('required')) {
      return 'El nombre del jugador es requerido.';
    }
    if (control.hasError('minlength')) {
      const min = control.getError('minlength')?.requiredLength ?? 2;
      return `El nombre debe tener al menos ${min} caracteres.`;
    }
    if (control.hasError('maxlength')) {
      const max = control.getError('maxlength')?.requiredLength ?? 50;
      return `El nombre no puede tener más de ${max} caracteres.`;
    }
    return 'Nombre inválido.';
  }

  constructor() {
    this.store.reset();
    void this.store.loadSettings();
  }

  protected async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isStarting.set(true);
    try {
      await this.store.startGame(this.form.getRawValue().playerName);

      if (this.store.error() === null) {
        await new Promise((resolve) => setTimeout(resolve, 1000));
        await this.router.navigate(['/game/play']);
      }
    } finally {
      this.isStarting.set(false);
    }
  }
}
