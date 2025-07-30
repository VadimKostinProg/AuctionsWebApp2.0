import { AuctionRequestStatusEnum } from "./auctionRequestStatusEnum";

export class AuctionRequest {
  public id!: number;
  public lotTitle!: string;
  public lotDescription!: string;
  public category!: string;
  public type!: string;
  public finishMethod!: string;
  public requestedAuctionTime!: string;
  public startPrice!: string;
  public status!: AuctionRequestStatusEnum;
  public requestedStartTime?: Date | null;
  public requestedFinishInterval?: string | null;
  public bidAmountInterval!: number;
  public aimPrice?: number | null;
  public reasonDeclined?: string | null;
  public imageUrls!: string[];
}
