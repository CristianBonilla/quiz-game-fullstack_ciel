import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { beforeEach, describe, expect, it } from 'vitest';
import { CountdownBarComponent } from './countdown-bar.component';

describe('CountdownBarComponent', () => {
  let fixture: ComponentFixture<CountdownBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CountdownBarComponent],
      providers: [provideZonelessChangeDetection(), provideNoopAnimations()]
    }).compileComponents();

    fixture = TestBed.createComponent(CountdownBarComponent);
    fixture.componentRef.setInput('totalSeconds', 30);
  });

  it('should not apply the critical style when above the threshold', () => {
    // Arrange
    fixture.componentRef.setInput('secondsRemaining', 10);

    // Act
    fixture.detectChanges();
    const bar: HTMLElement = fixture.nativeElement.querySelector('p-progressbar');

    // Assert
    expect(bar.getAttribute('data-critical')).toBe('false');
  });

  it('should apply the critical style at or below the threshold', () => {
    // Arrange
    fixture.componentRef.setInput('secondsRemaining', 5);

    // Act
    fixture.detectChanges();
    const bar: HTMLElement = fixture.nativeElement.querySelector('p-progressbar');

    // Assert
    expect(bar.getAttribute('data-critical')).toBe('true');
  });

  it('should describe the remaining seconds for assistive technology', () => {
    // Arrange
    fixture.componentRef.setInput('secondsRemaining', 12);

    // Act
    fixture.detectChanges();
    const bar: HTMLElement = fixture.nativeElement.querySelector('p-progressbar');

    // Assert
    expect(bar.getAttribute('aria-label')).toBe('Tiempo restante: 12 segundos');
  });
});
