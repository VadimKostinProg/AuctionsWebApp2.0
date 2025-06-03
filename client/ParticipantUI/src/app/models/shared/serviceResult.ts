export class ServiceResult<T> {
  public data?: T | null;
  public isSuccessfull!: boolean;
  public errors: string[] = [];
}

export class ServiceMessage {
  public message?: string | null;
  public isSuccessfull!: boolean;
  public errors: string[] = [];
}
