import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_CONFIG } from '@core/config/app-config.token';
import { QuestionRepository } from '@domain/contracts/question-repository';
import { Question } from '@domain/models/question';
import { CreateQuestionRequest, UpdateQuestionRequest } from '@domain/models/requests';
import { firstValueFrom } from 'rxjs';
import { QuestionDto } from '../dto/question.dto';
import { toCreateQuestionRequestDto, toQuestion, toUpdateQuestionRequestDto } from '../mappers/question.mapper';
import { IDEMPOTENCY_KEY_HEADER } from './idempotency';

@Injectable()
export class HttpQuestionRepository implements QuestionRepository {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(APP_CONFIG).apiBaseUrl;
  private readonly baseUrl = `${this.apiBaseUrl}/v1/questions`;

  async listByCategory(categoryId: string, onlyActive: boolean): Promise<readonly Question[]> {
    const dtos = await firstValueFrom(
      this.http.get<readonly QuestionDto[]>(`${this.apiBaseUrl}/v1/categories/${categoryId}/questions`, {
        params: new HttpParams().set('onlyActive', onlyActive)
      })
    );

    return dtos.map(toQuestion);
  }

  async getById(questionId: string): Promise<Question> {
    const dto = await firstValueFrom(this.http.get<QuestionDto>(`${this.baseUrl}/${questionId}`));
    return toQuestion(dto);
  }

  async create(request: CreateQuestionRequest): Promise<Question> {
    const dto = await firstValueFrom(
      this.http.post<QuestionDto>(this.baseUrl, toCreateQuestionRequestDto(request), {
        headers: { [IDEMPOTENCY_KEY_HEADER]: crypto.randomUUID() }
      })
    );

    return toQuestion(dto);
  }

  async update(questionId: string, request: UpdateQuestionRequest): Promise<Question> {
    const dto = await firstValueFrom(
      this.http.put<QuestionDto>(`${this.baseUrl}/${questionId}`, toUpdateQuestionRequestDto(request))
    );

    return toQuestion(dto);
  }

  async deactivate(questionId: string): Promise<void> {
    await firstValueFrom(this.http.delete<void>(`${this.baseUrl}/${questionId}`));
  }
}
