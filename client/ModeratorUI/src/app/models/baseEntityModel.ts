export class BaseEntityModel {
  public id!: number;
  public deleted!: boolean;
  public createdAt!: Date;
  public createdBy!: string;
  public modifiedAt?: Date | null;
  public modifiedBy?: string | null;
}
