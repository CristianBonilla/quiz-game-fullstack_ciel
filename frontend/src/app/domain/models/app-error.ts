export type AppErrorKind = 'validation' | 'notFound' | 'conflict' | 'network' | 'server' | 'unknown';

export interface AppError {
  readonly code: string;
  readonly message: string;
  readonly kind: AppErrorKind;
  readonly status: number | null;
}

export function isAppError(value: unknown): value is AppError {
  if (typeof value !== 'object' || value === null) {
    return false;
  }

  const candidate = value as Partial<AppError>;
  return typeof candidate.code === 'string' && typeof candidate.message === 'string';
}

export function toAppError(error: unknown, fallbackCode = 'Client.UnexpectedError'): AppError {
  if (isAppError(error)) {
    return error;
  }

  return {
    code: fallbackCode,
    message: error instanceof Error ? error.message : 'An unexpected error occurred.',
    kind: 'unknown',
    status: null
  };
}
