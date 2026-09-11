import { Pipe, PipeTransform } from '@angular/core';
import { DifficultyLevel } from '@domain/enums/difficulty-level';
import { difficultyLevelLabel } from '@shared/utils/difficulty-level-options';

@Pipe({ name: 'difficultyLevelLabel' })
export class DifficultyLevelLabelPipe implements PipeTransform {
  transform(level: DifficultyLevel): string {
    return difficultyLevelLabel(level);
  }
}
