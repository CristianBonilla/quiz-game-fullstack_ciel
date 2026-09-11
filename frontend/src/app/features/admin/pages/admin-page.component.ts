import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryStore } from '@features/admin/store/category.store';
import { QuestionStore } from '@features/admin/store/question.store';
import { Category } from '@domain/models/category';
import { Question } from '@domain/models/question';
import { DifficultyLevel } from '@domain/enums/difficulty-level';
import { formatPrize } from '@shared/utils/format-prize';
import { DIFFICULTY_LEVEL_OPTIONS } from '@shared/utils/difficulty-level-options';
import { DifficultyLevelLabelPipe } from '@shared/pipes/difficulty-level-label.pipe';
import { ButtonModule } from 'primeng/button';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { InputNumberModule } from 'primeng/inputnumber';

type CategoryDialogMode = { kind: 'create' } | { kind: 'edit'; category: Category };
type QuestionDialogMode = { kind: 'create' } | { kind: 'edit'; question: Question };

@Component({
  selector: 'app-admin-page',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    ConfirmDialogModule,
    DialogModule,
    InputTextModule,
    MessageModule,
    RadioButtonModule,
    SelectModule,
    TableModule,
    TooltipModule,
    InputNumberModule,
    DifficultyLevelLabelPipe
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './admin-page.component.html',
  styleUrl: './admin-page.component.scss'
})
export class AdminPageComponent {
  protected readonly categoryStore = inject(CategoryStore);
  protected readonly questionStore = inject(QuestionStore);
  protected readonly formatPrize = formatPrize;

  private readonly confirmation = inject(ConfirmationService);
  private readonly formBuilder = inject(NonNullableFormBuilder);

  protected readonly difficultyLevelOptions = [...DIFFICULTY_LEVEL_OPTIONS];

  protected readonly categoryDialog = signal<CategoryDialogMode | null>(null);
  protected readonly questionDialog = signal<QuestionDialogMode | null>(null);

  protected readonly categoryForm = this.formBuilder.group({
    name: this.formBuilder.control('', [Validators.required, Validators.maxLength(100)]),
    description: this.formBuilder.control('', [Validators.required, Validators.maxLength(500)]),
    difficultyLevel: this.formBuilder.control<DifficultyLevel | null>(null, [Validators.required]),
    prizeAmount: this.formBuilder.control<number | null>(null, [Validators.required, Validators.min(0)])
  });

  protected readonly questionForm = this.formBuilder.group({
    text: this.formBuilder.control('', [Validators.required, Validators.maxLength(500)]),
    correctAnswerIndex: this.formBuilder.control<number | null>(null, [Validators.required]),
    answers: this.formBuilder.array(
      Array.from({ length: 4 }, () =>
        this.formBuilder.group({
          text: this.formBuilder.control('', [Validators.required, Validators.maxLength(200)])
        })
      )
    )
  });

  protected readonly answerControls = computed(() => this.questionForm.controls.answers.controls);

  protected readonly categoryRows = computed(() => [...this.categoryStore.categories()]);
  protected readonly questionRows = computed(() => [...this.questionStore.questions()]);

  constructor() {
    void this.categoryStore.load(false);
  }

  protected onSelectCategory(category: Category): void {
    this.categoryStore.select(category.id);
    void this.questionStore.loadByCategory(category.id);
  }

  protected openCreateCategory(): void {
    this.categoryForm.reset({ name: '', description: '', difficultyLevel: null, prizeAmount: null });
    this.categoryDialog.set({ kind: 'create' });
  }

  protected openEditCategory(category: Category): void {
    this.categoryForm.reset({
      name: category.name,
      description: category.description,
      difficultyLevel: category.difficultyLevel,
      prizeAmount: category.prizeAmount
    });
    this.categoryDialog.set({ kind: 'edit', category });
  }

  protected async onSubmitCategory(): Promise<void> {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const raw = this.categoryForm.getRawValue();
    const value = {
      name: raw.name,
      description: raw.description,
      difficultyLevel: raw.difficultyLevel as DifficultyLevel,
      prizeAmount: raw.prizeAmount ?? 0
    };
    const mode = this.categoryDialog();

    if (mode?.kind === 'edit') {
      await this.categoryStore.update(mode.category.id, value);
    } else {
      await this.categoryStore.create(value);
    }

    if (this.categoryStore.error() === null) {
      this.categoryDialog.set(null);
    }
  }

  protected confirmDeactivateCategory(category: Category): void {
    this.confirmation.confirm({
      header: 'Desactivar categoría',
      message: `¿Desactivar "${category.name}"? Ya no estará disponible para nuevas partidas.`,
      acceptLabel: 'Desactivar',
      rejectLabel: 'Cancelar',
      accept: () => void this.categoryStore.deactivate(category.id)
    });
  }

  protected openCreateQuestion(): void {
    const categoryId = this.categoryStore.selectedCategoryId();
    if (categoryId === null) {
      return;
    }

    this.resetQuestionForm();
    this.questionDialog.set({ kind: 'create' });
  }

  protected openEditQuestion(question: Question): void {
    const correctAnswerIndex = question.answers.findIndex((answer) => answer.isCorrect);
    this.questionForm.reset({ text: question.text, correctAnswerIndex: correctAnswerIndex === -1 ? null : correctAnswerIndex });
    question.answers.forEach((answer, index) => {
      this.answerControls()[index]?.patchValue({ text: answer.text });
    });
    this.questionDialog.set({ kind: 'edit', question });
  }

  protected async onSubmitQuestion(): Promise<void> {
    this.questionForm.markAllAsTouched();
    if (this.questionForm.invalid) {
      return;
    }

    const categoryId = this.categoryStore.selectedCategoryId();
    const formValue = this.questionForm.getRawValue();
    const answers = formValue.answers.map((answer, index) => ({
      text: answer.text,
      isCorrect: index === formValue.correctAnswerIndex
    }));
    const value = { text: formValue.text, answers };
    const mode = this.questionDialog();

    if (mode?.kind === 'edit') {
      await this.questionStore.update(mode.question.id, value);
    } else if (categoryId !== null) {
      await this.questionStore.create({ categoryId, ...value });
      await this.categoryStore.load(false);
    }

    if (this.questionStore.error() === null) {
      this.questionDialog.set(null);
    }
  }

  protected confirmDeactivateQuestion(question: Question): void {
    this.confirmation.confirm({
      header: 'Desactivar pregunta',
      message: '¿Desactivar esta pregunta? Ya no se usará en nuevas rondas.',
      acceptLabel: 'Desactivar',
      rejectLabel: 'Cancelar',
      accept: () => void this.questionStore.deactivate(question.id)
    });
  }

  private resetQuestionForm(): void {
    this.questionForm.reset({ text: '', correctAnswerIndex: null });
    this.answerControls().forEach((control) => control.patchValue({ text: '' }));
  }
}
