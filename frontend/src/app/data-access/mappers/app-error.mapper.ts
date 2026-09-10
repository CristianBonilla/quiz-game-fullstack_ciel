import { HttpErrorResponse } from '@angular/common/http';
import { AppError, AppErrorKind } from '@domain/models/app-error';
import { HubResponseDto } from '../dto/hub-response.dto';
import { ProblemDetailsDto } from '../dto/problem-details.dto';

export function toAppErrorFromHttp(error: HttpErrorResponse): AppError {
  if (error.status === 0) {
    return {
      code: 'Network.Unreachable',
      message: 'The server could not be reached. Check your connection and try again.',
      kind: 'network',
      status: 0
    };
  }

  const problem = asProblemDetails(error.error);

  return {
    code: problem?.code ?? problem?.title ?? `Http.${error.status}`,
    message: describe(problem) ?? error.message,
    kind: toKind(error.status),
    status: error.status
  };
}

export function toAppErrorFromHubResponse(response: HubResponseDto<unknown>): AppError {
  return {
    code: response.errorCode ?? 'Hub.UnknownError',
    message: response.errorDescription ?? 'The operation was rejected by the server.',
    kind: 'conflict',
    status: null
  };
}

function asProblemDetails(payload: unknown): ProblemDetailsDto | null {
  return typeof payload === 'object' && payload !== null ? (payload as ProblemDetailsDto) : null;
}

function describe(problem: ProblemDetailsDto | null): string | null {
  if (problem === null) {
    return null;
  }

  const validationMessages = Object.values(problem.errors ?? {}).flat();
  if (validationMessages.length > 0) {
    return validationMessages.join(' ');
  }

  return problem.detail ?? problem.title ?? null;
}

function toKind(status: number): AppErrorKind {
  switch (status) {
    case 400:
    case 422:
      return 'validation';
    case 404:
      return 'notFound';
    case 409:
      return 'conflict';
    default:
      return status >= 500 ? 'server' : 'unknown';
  }
}
