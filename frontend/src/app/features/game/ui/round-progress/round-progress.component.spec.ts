import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { FakeMotionPreferenceService } from '@testing/fake-motion-preference.service';
import { beforeEach, describe, expect, it } from 'vitest';
import { RoundProgressComponent } from './round-progress.component';

describe('RoundProgressComponent', () => {
  let fixture: ComponentFixture<RoundProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoundProgressComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        { provide: MotionPreferenceService, useValue: new FakeMotionPreferenceService() }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoundProgressComponent);
    fixture.componentRef.setInput('currentRound', 3);
    fixture.componentRef.setInput('totalRounds', 5);
    fixture.detectChanges();
  });

  it('should render one segment per configured round', () => {
    // Arrange, Act
    const segments = fixture.nativeElement.querySelectorAll('[role="img"] > span');

    // Assert
    expect(segments).toHaveLength(5);
  });

  it('should mark as filled only the segments up to the current round', () => {
    // Arrange, Act
    const filledSegments = fixture.nativeElement.querySelectorAll('[role="img"] > span[data-filled="true"]');

    // Assert
    expect(filledSegments).toHaveLength(3);
  });

  it('should describe the round with an accessible label', () => {
    // Arrange, Act
    const region: HTMLElement = fixture.nativeElement.querySelector('[role="img"]');

    // Assert
    expect(region.getAttribute('aria-label')).toBe('Ronda 3 de 5');
  });
});
