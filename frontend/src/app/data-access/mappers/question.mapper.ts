import { Question } from '@domain/models/question';
import { CreateQuestionRequest, UpdateQuestionRequest } from '@domain/models/requests';
import { CreateQuestionRequestDto, QuestionDto, UpdateQuestionRequestDto } from '../dto/question.dto';

export function toQuestion(dto: QuestionDto): Question {
  return {
    id: dto.id,
    categoryId: dto.categoryId,
    text: dto.text,
    isActive: dto.isActive,
    answers: dto.answers.map((answer) => ({ id: answer.id, text: answer.text, isCorrect: answer.isCorrect }))
  };
}

export function toCreateQuestionRequestDto(request: CreateQuestionRequest): CreateQuestionRequestDto {
  return {
    categoryId: request.categoryId,
    text: request.text,
    answers: request.answers.map((answer) => ({ text: answer.text, isCorrect: answer.isCorrect }))
  };
}

export function toUpdateQuestionRequestDto(request: UpdateQuestionRequest): UpdateQuestionRequestDto {
  return {
    text: request.text,
    answers: request.answers.map((answer) => ({ text: answer.text, isCorrect: answer.isCorrect }))
  };
}
