import { BaseEntityModel } from "../baseEntityModel";

export class AuctionBasic extends BaseEntityModel {
  public category!: string;
  public type!: string;
  public lotTitle!: string;
  public startTime!: string;
  public finishTime!: string;
  public startPrice!: number;
  public currentPrice!: number;
}
