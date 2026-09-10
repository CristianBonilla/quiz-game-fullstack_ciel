import { Category } from '../models/category';
import { CreateCategoryRequest, UpdateCategoryRequest } from '../models/requests';

export interface CategoryRepository {
  list(onlyActive: boolean): Promise<readonly Category[]>;
  getById(categoryId: string): Promise<Category>;
  create(request: CreateCategoryRequest): Promise<Category>;
  update(categoryId: string, request: UpdateCategoryRequest): Promise<Category>;
  deactivate(categoryId: string): Promise<void>;
}
