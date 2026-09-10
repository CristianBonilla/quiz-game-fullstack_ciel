export interface HubResponseDto<T> {
  readonly isSuccess: boolean;
  readonly value: T | null;
  readonly errorCode: string | null;
  readonly errorDescription: string | null;
}
