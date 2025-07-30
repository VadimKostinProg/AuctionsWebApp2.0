export class AuctionComment {
  public id!: number;
  public userId!: number;
  public auctionId!: number;
  public score!: number;
  public commentText!: string;
  public createdAt!: Date;
  public username!: string;
}
