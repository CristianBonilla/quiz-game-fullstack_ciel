import { Category } from '@domain/models/category';
import { CreateCategoryRequest, UpdateCategoryRequest } from '@domain/models/requests';
import { CategoryDto, CreateCategoryRequestDto, UpdateCategoryRequestDto } from '../dto/category.dto';
import { toDifficultyLevel } from './primitive.mapper';

export function toCategory(dto: CategoryDto): Category {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description,
    difficultyLevel: toDifficultyLevel(dto.difficultyLevel),
    prizeAmount: dto.prizeAmount,
    isActive: dto.isActive,
    questionCount: dto.questionCount
  };
}

export function toCreateCategoryRequestDto(request: CreateCategoryRequest): CreateCategoryRequestDto {
  return {
    name: request.name,
    description: request.description,
    difficultyLevel: request.difficultyLevel,
    prizeAmount: request.prizeAmount
  };
}

export function toUpdateCategoryRequestDto(request: UpdateCategoryRequest): UpdateCategoryRequestDto {
  return toCreateCategoryRequestDto(request);
}
