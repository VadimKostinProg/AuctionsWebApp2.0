import { UserBasic } from "../users/userBasic";
import { AuctionBasic } from "./auctionBasic";
import { AuctionStatusEnum } from "./auctionStatusEnum";

export class Auction extends AuctionBasic {
  finishMethod!: string;
  lotDescription!: string;
  auctionTime!: string;
  finishTimeInterval?: string;
  bidAmountInterval!: number;
  status!: AuctionStatusEnum;
  cancellationReason?: string | null;
  averageScore?: number | null;
  auctionist!: UserBasic;
  winner?: UserBasic | null;
  imageUrls!: string[];
}
