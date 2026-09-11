import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { FakeMotionPreferenceService } from '@testing/fake-motion-preference.service';
import { beforeEach, describe, expect, it } from 'vitest';
import { PrizeDisplayComponent } from './prize-display.component';

describe('PrizeDisplayComponent', () => {
  let fixture: ComponentFixture<PrizeDisplayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrizeDisplayComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        { provide: MotionPreferenceService, useValue: new FakeMotionPreferenceService() }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PrizeDisplayComponent);
  });

  it('should format the accumulated amount as currency', () => {
    // Arrange
    fixture.componentRef.setInput('amount', 2600);

    // Act
    fixture.detectChanges();
    const value: HTMLElement = fixture.nativeElement.querySelector('[data-testid="prize-value"]');

    // Assert
    expect(value.textContent?.trim()).toBe('$2,600');
  });

  it('should jump directly to the new amount when reduced motion is preferred', () => {
    // Arrange
    fixture.componentRef.setInput('amount', 100);
    fixture.detectChanges();

    // Act
    fixture.componentRef.setInput('amount', 900);
    fixture.detectChanges();
    const value: HTMLElement = fixture.nativeElement.querySelector('[data-testid="prize-value"]');

    // Assert
    expect(value.textContent?.trim()).toBe('$900');
  });
});
