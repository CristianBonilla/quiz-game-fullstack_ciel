import { describe, expect, it } from 'vitest';
import { GameConfigurationDto, GameStateDto, GameSummaryDto, AnswerQuestionDto, PlayableQuestionDto } from '../dto/game.dto';
import {
  toAnswerEvaluation,
  toGame,
  toGameSettings,
  toGameSummary,
  toPlayableQuestion,
  toRound,
  toRoundConfiguration,
  toStartGameRequestDto,
  toSubmitAnswerRequestDto,
  toWithdrawRequestDto
} from './game.mapper';

function aGameStateDto(overrides: Partial<GameStateDto> = {}): GameStateDto {
  return {
    gameId: 'game-1',
    playerName: 'Ada',
    status: 'InProgress',
    currentRound: 2,
    totalRounds: 5,
    accumulatedPrize: 300,
    prizeAtStake: 200,
    currentQuestion: null,
    deadlineUtc: null,
    ...overrides
  };
}

describe('toPlayableQuestion', () => {
  it('should map every answer without leaking whether it is correct', () => {
    // Arrange
    const dto: PlayableQuestionDto = {
      id: 'question-1',
      categoryId: 'category-1',
      text: 'What is 2 + 2?',
      answers: [{ id: 'answer-1', text: '4' }, { id: 'answer-2', text: '5' }]
    };
    const dtoWithLeakedField = {
      ...dto,
      answers: dto.answers.map((answer) => ({ ...answer, isCorrect: answer.id === 'answer-1' }))
    } as PlayableQuestionDto;

    // Act
    const result = toPlayableQuestion(dtoWithLeakedField);

    // Assert
    expect(result.answers).toHaveLength(2);
    expect(result.answers[0]).not.toHaveProperty('isCorrect');
    expect(result.answers[1]).not.toHaveProperty('isCorrect');
  });
});

describe('toGame', () => {
  it('should convert the deadline to a UTC date when present', () => {
    // Arrange
    const dto = aGameStateDto({ deadlineUtc: '2026-01-01T10:00:00.000Z' });

    // Act
    const result = toGame(dto);

    // Assert
    expect(result.deadlineUtc).toEqual(new Date('2026-01-01T10:00:00.000Z'));
  });

  it('should map a null deadline to null instead of an invalid date', () => {
    // Arrange
    const dto = aGameStateDto({ deadlineUtc: null });

    // Act
    const result = toGame(dto);

    // Assert
    expect(result.deadlineUtc).toBeNull();
  });

  it('should map a null current question to null', () => {
    // Arrange
    const dto = aGameStateDto({ currentQuestion: null });

    // Act
    const result = toGame(dto);

    // Assert
    expect(result.currentQuestion).toBeNull();
  });

  it('should rename gameId to id', () => {
    // Arrange
    const dto = aGameStateDto({ gameId: 'game-42' });

    // Act
    const result = toGame(dto);

    // Assert
    expect(result.id).toBe('game-42');
  });
});

describe('toAnswerEvaluation', () => {
  it('should map a null next question to null when the game just ended', () => {
    // Arrange
    const dto: AnswerQuestionDto = {
      gameId: 'game-1',
      answeredRound: 5,
      isCorrect: true,
      correctAnswerId: 'answer-1',
      status: 'Won',
      accumulatedPrize: 2600,
      currentRound: 5,
      prizeAtStake: 0,
      nextQuestion: null,
      deadlineUtc: null
    };

    // Act
    const result = toAnswerEvaluation(dto);

    // Assert
    expect(result.nextQuestion).toBeNull();
    expect(result.status).toBe('Won');
  });
});

describe('toGameSummary', () => {
  it('should map an optional endedAtUtc that is present', () => {
    // Arrange
    const dto: GameSummaryDto = {
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'Lost',
      finalPrize: 0,
      roundsPlayed: 3,
      startedAtUtc: '2026-01-01T10:00:00.000Z',
      endedAtUtc: '2026-01-01T10:05:00.000Z',
      rounds: []
    };

    // Act
    const result = toGameSummary(dto);

    // Assert
    expect(result.endedAtUtc).toEqual(new Date('2026-01-01T10:05:00.000Z'));
  });

  it('should map a missing endedAtUtc to null instead of undefined', () => {
    // Arrange
    const dto: GameSummaryDto = {
      gameId: 'game-1',
      playerName: 'Ada',
      status: 'InProgress',
      finalPrize: 0,
      roundsPlayed: 1,
      startedAtUtc: '2026-01-01T10:00:00.000Z',
      endedAtUtc: null,
      rounds: []
    };

    // Act
    const result = toGameSummary(dto);

    // Assert
    expect(result.endedAtUtc).toBeNull();
  });
});

describe('toGameSettings', () => {
  it('should map every configured round', () => {
    // Arrange
    const dto: GameConfigurationDto = {
      totalRounds: 2,
      questionTimeLimitSeconds: 30,
      rounds: [
        { roundNumber: 1, categoryId: 'c1', categoryName: 'Dev', difficultyLevel: 1, prizeAmount: 100, availableQuestions: 5 },
        { roundNumber: 2, categoryId: 'c2', categoryName: 'Arch', difficultyLevel: 2, prizeAmount: 500, availableQuestions: 6 }
      ]
    };

    // Act
    const result = toGameSettings(dto);

    // Assert
    expect(result.rounds).toHaveLength(2);
    expect(result.rounds[1]?.categoryName).toBe('Arch');
  });
});

describe('toRound', () => {
  it('should map an unanswered round with null selection fields', () => {
    // Arrange
    const dto = {
      number: 3,
      questionId: 'question-3',
      outcome: 'Pending',
      prizeAtStake: 400,
      selectedAnswerId: null,
      answeredAtUtc: null
    };

    // Act
    const result = toRound(dto);

    // Assert
    expect(result.selectedAnswerId).toBeNull();
    expect(result.answeredAtUtc).toBeNull();
  });
});

describe('toRoundConfiguration', () => {
  it('should map a valid difficulty level within range', () => {
    // Arrange
    const dto = { roundNumber: 4, categoryId: 'c4', categoryName: 'Data', difficultyLevel: 3, prizeAmount: 1500, availableQuestions: 8 };

    // Act
    const result = toRoundConfiguration(dto);

    // Assert
    expect(result.difficultyLevel).toBe(3);
  });
});

describe('toStartGameRequestDto', () => {
  it('should carry the player name through to the wire format', () => {
    // Arrange, Act
    const result = toStartGameRequestDto({ playerName: 'Grace' });

    // Assert
    expect(result).toEqual({ playerName: 'Grace' });
  });
});

describe('toSubmitAnswerRequestDto', () => {
  it('should map every field required for idempotent submission', () => {
    // Arrange, Act
    const result = toSubmitAnswerRequestDto({ gameId: 'game-1', answerId: 'answer-1', requestId: 'request-1' });

    // Assert
    expect(result).toEqual({ gameId: 'game-1', answerId: 'answer-1', requestId: 'request-1' });
  });
});

describe('toWithdrawRequestDto', () => {
  it('should map the game id', () => {
    // Arrange, Act
    const result = toWithdrawRequestDto({ gameId: 'game-1' });

    // Assert
    expect(result).toEqual({ gameId: 'game-1' });
  });
});
