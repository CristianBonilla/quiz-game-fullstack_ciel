import { Component } from '@angular/core';
import { ChangeDetectionStrategy } from '@angular/core';
import { By } from '@angular/platform-browser';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { MotionPreferenceService } from '@core/theme/motion-preference.service';
import { FakeMotionPreferenceService } from '@testing/fake-motion-preference.service';
import { beforeEach, describe, expect, it } from 'vitest';
import { QuestionCardComponent } from './question-card.component';
import { PlayableAnswerView } from './question-card.models';

@Component({
  imports: [QuestionCardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <app-question-card
      [questionText]="questionText"
      [answers]="answers"
      [disabled]="disabled"
      [feedback]="feedback"
      (answerSelected)="selected = $event"
    />
  `
})
class HostComponent {
  questionText = 'What is 2 + 2?';
  answers: readonly PlayableAnswerView[] = [
    { id: 'answer-1', text: '3', index: 1 },
    { id: 'answer-2', text: '4', index: 2 }
  ];
  disabled = false;
  feedback: { selectedAnswerId: string | null; correctAnswerId: string; isCorrect: boolean } | null = null;
  selected: string | null = null;
}

describe('QuestionCardComponent', () => {
  let fixture: ComponentFixture<HostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HostComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
        { provide: MotionPreferenceService, useValue: new FakeMotionPreferenceService() }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HostComponent);
  });

  it('should render one button per answer', () => {
    // Arrange, Act
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('button'));

    // Assert
    expect(buttons).toHaveLength(2);
  });

  it('should emit answerSelected with the id of the clicked answer', () => {
    // Arrange
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('button'));

    // Act
    buttons[1]?.nativeElement.click();

    // Assert
    expect(fixture.componentInstance.selected).toBe('answer-2');
  });

  it('should disable every answer button when disabled is true', () => {
    // Arrange
    fixture.componentInstance.disabled = true;

    // Act
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('button'));

    // Assert
    expect(buttons.every((button) => button.nativeElement.disabled)).toBe(true);
  });

  it('should mark the correct answer as aria-pressed when it matches the selected answer', () => {
    // Arrange
    fixture.componentInstance.feedback = { selectedAnswerId: 'answer-1', correctAnswerId: 'answer-2', isCorrect: false };

    // Act
    fixture.detectChanges();
    const buttons = fixture.debugElement.queryAll(By.css('button'));

    // Assert
    expect(buttons[0]?.nativeElement.getAttribute('aria-pressed')).toBe('true');
    expect(buttons[1]?.nativeElement.getAttribute('aria-pressed')).toBe('false');
  });
});
