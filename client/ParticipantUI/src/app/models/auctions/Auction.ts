import { UserBasic } from "../users/userBasic";
import { AuctionBasic } from "./AuctionBasic";
import { AuctionStatusEnum } from "./auctionStatusEnum";

export class Auction extends AuctionBasic {
  public type!: string;
  public finishMethod!: string;
  public lotDescription!: string;
  public auctionTime!: string;
  public bidAmountInterval!: number;
  public status!: AuctionStatusEnum;
  public finishPrice?: number | null;
  public winner?: UserBasic | null;
}
