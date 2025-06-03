export class PostAuctionRequest {
  auctionCategoryId!: number;
  auctionTypeId!: number;
  auctionFinishMethodId!: number;
  lotTitle!: string;
  lotDescription!: string;
  requestedAuctionTime!: string;
  startPrice!: number;
  requestedStartTime?: Date | null;
  finishTimeInterval?: string | null;
  bidAmountInterval!: number;
  aimPrice?: number | null;
  images!: File[];
}
