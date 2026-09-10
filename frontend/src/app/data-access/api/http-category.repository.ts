import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import { CategoryRepository } from '@domain/contracts/category-repository';
import { Category } from '@domain/models/category';
import { CreateCategoryRequest, UpdateCategoryRequest } from '@domain/models/requests';
import { firstValueFrom } from 'rxjs';
import { CategoryDto } from '../dto/category.dto';
import { toCategory, toCreateCategoryRequestDto, toUpdateCategoryRequestDto } from '../mappers/category.mapper';
import { IDEMPOTENCY_KEY_HEADER } from './idempotency';

@Injectable()
export class HttpCategoryRepository implements CategoryRepository {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(APP_CONFIG).apiBaseUrl}/v1/categories`;

  async list(onlyActive: boolean): Promise<readonly Category[]> {
    const dtos = await firstValueFrom(
      this.http.get<readonly CategoryDto[]>(this.baseUrl, {
        params: new HttpParams().set('onlyActive', onlyActive)
      })
    );

    return dtos.map(toCategory);
  }

  async getById(categoryId: string): Promise<Category> {
    const dto = await firstValueFrom(this.http.get<CategoryDto>(`${this.baseUrl}/${categoryId}`));
    return toCategory(dto);
  }

  async create(request: CreateCategoryRequest): Promise<Category> {
    const dto = await firstValueFrom(
      this.http.post<CategoryDto>(this.baseUrl, toCreateCategoryRequestDto(request), {
        headers: { [IDEMPOTENCY_KEY_HEADER]: crypto.randomUUID() }
      })
    );

    return toCategory(dto);
  }

  async update(categoryId: string, request: UpdateCategoryRequest): Promise<Category> {
    const dto = await firstValueFrom(
      this.http.put<CategoryDto>(`${this.baseUrl}/${categoryId}`, toUpdateCategoryRequestDto(request))
    );

    return toCategory(dto);
  }

  async deactivate(categoryId: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${categoryId}`));
  }
}
