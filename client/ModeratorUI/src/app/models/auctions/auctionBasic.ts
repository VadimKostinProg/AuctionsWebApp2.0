import { BaseEntityModel } from "../baseEntityModel";

export class AuctionBasic extends BaseEntityModel {
  public category!: string;
  public type!: string;
  public lotTitle!: string;
  public startTime!: Date;
  public finishTime!: Date;
  public startPrice!: number;
  public currentPrice!: number;
}
