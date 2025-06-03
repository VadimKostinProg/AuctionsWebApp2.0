export class UserAuction {
  public id!: number;
  public lotTitle!: string;
  public category!: string;
  public startTime!: Date;
  public finishTime!: Date;
  public startPrice!: number;
  public currentPrice!: number;
  public averageScore!: number | null;
}
