import { UserBasic } from "../users/userBasic";

export class AuctionBasic {
  public id!: number;
  public lotTitle!: string;
  public category!: string;
  public startTime!: Date;
  public finishTime!: Date;
  public startPrice!: number;
  public currentPrice!: number;
  public averageScore!: number | null;
  public auctionist!: UserBasic;
  public imageUrls!: string[];
}
