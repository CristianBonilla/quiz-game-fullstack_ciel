import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-withdraw-button',
  imports: [ButtonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <p-button
      label="Retirarme con mi acumulado"
      severity="secondary"
      [text]="true"
      size="small"
      [disabled]="disabled()"
      (onClick)="withdraw.emit()"
    />
  `
})
export class WithdrawButtonComponent {
  readonly disabled = input(false);
  readonly withdraw = output<void>();
}
