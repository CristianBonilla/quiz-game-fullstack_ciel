import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ThemeService } from '@core/theme/theme.service';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ToggleSwitchModule } from 'primeng/toggleswitch';

@Component({
  selector: 'app-layout',
  imports: [RouterLink, RouterOutlet, FormsModule, ToggleSwitchModule, ToastModule, ConfirmDialogModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss'
})
export class AppLayoutComponent {
  protected readonly theme = inject(ThemeService);

  constructor() {
    this.theme.initialize();
  }

  protected onThemeToggle(isDark: boolean): void {
    this.theme.apply(isDark ? 'dark' : 'light');
  }
}
