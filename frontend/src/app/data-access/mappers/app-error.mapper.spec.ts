import { HttpErrorResponse } from '@angular/common/http';
import { describe, expect, it } from 'vitest';
import { HubResponseDto } from '../dto/hub-response.dto';
import { toAppErrorFromHttp, toAppErrorFromHubResponse } from './app-error.mapper';

describe('toAppErrorFromHttp', () => {
  it('should classify a status 0 as a network error', () => {
    // Arrange
    const error = new HttpErrorResponse({ status: 0, url: '/api/v1/games' });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.kind).toBe('network');
  });

  it('should classify a 400 response as a validation error', () => {
    // Arrange
    const error = new HttpErrorResponse({ status: 400, error: { title: 'Bad Request' } });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.kind).toBe('validation');
  });

  it('should classify a 404 response as a not-found error', () => {
    // Arrange
    const error = new HttpErrorResponse({ status: 404, error: { title: 'Not Found' } });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.kind).toBe('notFound');
  });

  it('should classify a 409 response as a conflict error', () => {
    // Arrange
    const error = new HttpErrorResponse({ status: 409, error: { title: 'Conflict' } });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.kind).toBe('conflict');
  });

  it('should classify a 500 response as a server error', () => {
    // Arrange
    const error = new HttpErrorResponse({ status: 500, error: { title: 'Server Error' } });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.kind).toBe('server');
  });

  it('should join validation messages from ProblemDetails.errors', () => {
    // Arrange
    const error = new HttpErrorResponse({
      status: 400,
      error: { title: 'Bad Request', errors: { playerName: ['Player name is required.'] } }
    });

    // Act
    const result = toAppErrorFromHttp(error);

    // Assert
    expect(result.message).toBe('Player name is required.');
  });
});

describe('toAppErrorFromHubResponse', () => {
  it('should map the error code and description from a failed hub response', () => {
    // Arrange
    const response: HubResponseDto<unknown> = {
      isSuccess: false,
      value: null,
      errorCode: 'Game.RoundClosed',
      errorDescription: 'The round is no longer open.'
    };

    // Act
    const result = toAppErrorFromHubResponse(response);

    // Assert
    expect(result.code).toBe('Game.RoundClosed');
    expect(result.message).toBe('The round is no longer open.');
  });
});
