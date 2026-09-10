import { Question } from '../models/question';
import { CreateQuestionRequest, UpdateQuestionRequest } from '../models/requests';

export interface QuestionRepository {
  listByCategory(categoryId: string, onlyActive: boolean): Promise<readonly Question[]>;
  getById(questionId: string): Promise<Question>;
  create(request: CreateQuestionRequest): Promise<Question>;
  update(questionId: string, request: UpdateQuestionRequest): Promise<Question>;
  deactivate(questionId: string): Promise<void>;
}
