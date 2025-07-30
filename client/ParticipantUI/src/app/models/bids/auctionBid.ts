import { UserBasic } from "../users/userBasic";

export class AuctionBid {
  public auctionId!: number;
  public amount!: number;
  public time!: string;
  public bidder!: UserBasic;
}
