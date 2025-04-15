import { AuctionModel } from "./auctionModel";

export class AuctionDetailsModel extends AuctionModel {
    public startDateAndTime: string;
    public lotDescription: string;
    public score: number;
    public finishTypeDescription: number;
    public status: string;
    public winnerId: string | null;
    public winner: string | null;
}