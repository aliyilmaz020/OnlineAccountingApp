export class ApiError extends Error {
  errorCode: string | null;
  errors: Record<string, string[]> | null;
  status: number;

  constructor(
    message: string,
    errorCode: string | null,
    errors: Record<string, string[]> | null,
    status: number,
  ) {
    super(message);
    this.name = "ApiError";
    this.errorCode = errorCode;
    this.errors = errors;
    this.status = status;
  }
}
