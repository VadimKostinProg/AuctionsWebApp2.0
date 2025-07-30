import { UserBasic } from "../users/userBasic";
import { AuctionStatusEnum } from "./auctionStatusEnum";

export class AuctionBasic {
  public id!: number;
  public lotTitle!: string;
  public category!: string;
  public startTime!: Date;
  public finishTime!: Date;
  public startPrice!: number;
  public currentPrice!: number;
  public averageScore!: number | null;
  public auctioneer!: UserBasic;
  public status!: AuctionStatusEnum;
  public imageUrls!: string[];
}
