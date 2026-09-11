import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { beforeEach, describe, expect, it } from 'vitest';
import { WithdrawButtonComponent } from './withdraw-button.component';

describe('WithdrawButtonComponent', () => {
  let fixture: ComponentFixture<WithdrawButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WithdrawButtonComponent],
      providers: [provideZonelessChangeDetection(), provideNoopAnimations()]
    }).compileComponents();

    fixture = TestBed.createComponent(WithdrawButtonComponent);
  });

  it('should disable the button when disabled is true', () => {
    // Arrange
    fixture.componentRef.setInput('disabled', true);

    // Act
    fixture.detectChanges();
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button');

    // Assert
    expect(button.disabled).toBe(true);
  });

  it('should emit withdraw when clicked while enabled', () => {
    // Arrange
    fixture.componentRef.setInput('disabled', false);
    fixture.detectChanges();
    let emitted = false;
    fixture.componentInstance.withdraw.subscribe(() => {
      emitted = true;
    });

    // Act
    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    button.click();

    // Assert
    expect(emitted).toBe(true);
  });
});
