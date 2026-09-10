import { computed, inject } from '@angular/core';
import { QUESTION_REPOSITORY } from '@core/config/injection-tokens';
import { AppError, toAppError } from '@domain/models/app-error';
import { CreateQuestionRequest, UpdateQuestionRequest } from '@domain/models/requests';
import { Question } from '@domain/models/question';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';

export type QuestionStoreStatus = 'idle' | 'loading' | 'ready' | 'error';

export interface QuestionState {
  readonly categoryId: string | null;
  readonly questions: readonly Question[];
  readonly status: QuestionStoreStatus;
  readonly error: AppError | null;
}

const initialState: QuestionState = {
  categoryId: null,
  questions: [],
  status: 'idle',
  error: null
};

export const QuestionStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ questions }) => ({
    activeQuestions: computed(() => questions().filter((question) => question.isActive))
  })),
  withMethods((store, repository = inject(QUESTION_REPOSITORY)) => {
    function fail(error: unknown): void {
      patchState(store, { status: 'error', error: toAppError(error) });
    }

    async function loadByCategory(categoryId: string): Promise<void> {
      patchState(store, { status: 'loading', error: null, categoryId });

      try {
        patchState(store, { questions: await repository.listByCategory(categoryId, false), status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    async function create(request: CreateQuestionRequest): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        const question = await repository.create(request);
        patchState(store, { questions: [...store.questions(), question], status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    async function update(questionId: string, request: UpdateQuestionRequest): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        const updated = await repository.update(questionId, request);
        patchState(store, {
          questions: store.questions().map((question) => (question.id === questionId ? updated : question)),
          status: 'ready'
        });
      } catch (error) {
        fail(error);
      }
    }

    async function deactivate(questionId: string): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        await repository.deactivate(questionId);
        patchState(store, {
          questions: store.questions().map((question) =>
            question.id === questionId ? { ...question, isActive: false } : question
          ),
          status: 'ready'
        });
      } catch (error) {
        fail(error);
      }
    }

    function reset(): void {
      patchState(store, initialState);
    }

    return { loadByCategory, create, update, deactivate, reset };
  })
);
