import { Directive, HostListener, input, output } from '@angular/core';

/** Lets any focusable option respond to its own digit shortcut (1-9) without a page-level coordinator. */
@Directive({ selector: '[appNumberKey]' })
export class NumberKeyDirective {
  readonly appNumberKey = input.required<number>();
  readonly numberKeyPressed = output<void>();

  @HostListener('document:keydown', ['$event'])
  protected onKeydown(event: KeyboardEvent): void {
    if (event.key === String(this.appNumberKey())) {
      this.numberKeyPressed.emit();
    }
  }
}
