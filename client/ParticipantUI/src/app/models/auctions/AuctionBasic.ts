import { UserBasic } from "../users/userBasic";

export class AuctionBasic {
  public id!: number;
  public lotTitle!: string;
  public category!: string;
  public startTime!: Date;
  public finishTime!: Date;
  public currentPrice!: number;
  public averageScore!: number;
  public auctionist!: UserBasic;
  public imageUrls!: string[];
}
