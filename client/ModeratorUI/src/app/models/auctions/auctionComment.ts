import { BaseEntityModel } from "../baseEntityModel";

export class AuctionComment extends BaseEntityModel {
  public userId!: number;
  public auctionId!: number;
  public score!: number;
  public commentText!: string;
  public username!: string;
}
