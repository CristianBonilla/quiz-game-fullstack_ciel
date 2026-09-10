export interface CategoryDto {
  readonly id: string;
  readonly name: string;
  readonly description: string;
  readonly difficultyLevel: number;
  readonly prizeAmount: number;
  readonly isActive: boolean;
  readonly questionCount: number;
}

export interface CreateCategoryRequestDto {
  readonly name: string;
  readonly description: string;
  readonly difficultyLevel: number;
  readonly prizeAmount: number;
}

export type UpdateCategoryRequestDto = CreateCategoryRequestDto;
