import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ReactiveFormsModule, Validators } from '@angular/forms';
import { NonNullableFormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { GameStore } from '@features/game/store/game.store';
import { formatPrize } from '@shared/utils/format-prize';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'app-setup-page',
  imports: [ReactiveFormsModule, ButtonModule, CardModule, InputTextModule, MessageModule, SkeletonModule],
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

  constructor() {
    this.store.reset();
    void this.store.loadSettings();
  }

  protected async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    await this.store.startGame(this.form.getRawValue().playerName);

    if (this.store.error() === null) {
      await this.router.navigate(['/game/play']);
    }
  }
}
