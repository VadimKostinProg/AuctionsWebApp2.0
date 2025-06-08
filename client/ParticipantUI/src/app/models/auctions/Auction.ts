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
  public isPaymentPerformed!: boolean;
  public isDeliveryPerformed!: boolean;
}

abstract class AuctionResource {
  public id!: number;
  public name!: string;
  public description!: string;
}

export class AuctionCategory extends AuctionResource { }
export class AuctionType extends AuctionResource { }
export class AuctionFinishMethod extends AuctionResource { }
