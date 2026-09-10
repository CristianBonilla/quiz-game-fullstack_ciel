export interface ProblemDetailsDto {
  readonly type?: string;
  readonly title?: string;
  readonly status?: number;
  readonly detail?: string;
  readonly instance?: string;
  readonly code?: string;
  readonly errors?: Readonly<Record<string, readonly string[]>>;
}
