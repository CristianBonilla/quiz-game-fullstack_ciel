import { computed, inject } from '@angular/core';
import { CATEGORY_REPOSITORY } from '@core/config/injection-tokens';
import { AppError, toAppError } from '@domain/models/app-error';
import { Category } from '@domain/models/category';
import { CreateCategoryRequest, UpdateCategoryRequest } from '@domain/models/requests';
import { MINIMUM_QUESTIONS_PER_CATEGORY } from '@domain/rules/game-rules';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';

export type CategoryStoreStatus = 'idle' | 'loading' | 'ready' | 'error';

export interface CategoryState {
  readonly categories: readonly Category[];
  readonly selectedCategoryId: string | null;
  readonly status: CategoryStoreStatus;
  readonly error: AppError | null;
}

const initialState: CategoryState = {
  categories: [],
  selectedCategoryId: null,
  status: 'idle',
  error: null
};

export const CategoryStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ categories, selectedCategoryId }) => ({
    activeCategories: computed(() => categories().filter((category) => category.isActive)),
    playableCategories: computed(() =>
      categories().filter(
        (category) => category.isActive && category.questionCount >= MINIMUM_QUESTIONS_PER_CATEGORY
      )
    ),
    selectedCategory: computed(() => categories().find((category) => category.id === selectedCategoryId()) ?? null)
  })),
  withMethods((store, repository = inject(CATEGORY_REPOSITORY)) => {
    function fail(error: unknown): void {
      patchState(store, { status: 'error', error: toAppError(error) });
    }

    async function load(onlyActive = false): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        patchState(store, { categories: await repository.list(onlyActive), status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    async function create(request: CreateCategoryRequest): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        const category = await repository.create(request);
        patchState(store, { categories: [...store.categories(), category], status: 'ready' });
      } catch (error) {
        fail(error);
      }
    }

    async function update(categoryId: string, request: UpdateCategoryRequest): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        const updated = await repository.update(categoryId, request);
        patchState(store, {
          categories: store.categories().map((category) => (category.id === categoryId ? updated : category)),
          status: 'ready'
        });
      } catch (error) {
        fail(error);
      }
    }

    async function deactivate(categoryId: string): Promise<void> {
      patchState(store, { status: 'loading', error: null });

      try {
        await repository.deactivate(categoryId);
        patchState(store, {
          categories: store.categories().map((category) =>
            category.id === categoryId ? { ...category, isActive: false } : category
          ),
          status: 'ready'
        });
      } catch (error) {
        fail(error);
      }
    }

    function select(categoryId: string | null): void {
      patchState(store, { selectedCategoryId: categoryId });
    }

    return { load, create, update, deactivate, select };
  })
);
