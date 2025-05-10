export class AuctionModel {
    public id: string;
    public name: string;
    public category: string;
    public auctionistId: string;
    public auctionist: string;
    public auctionTime: string;
    public startPrice: number;
    public currentBid: number;
    public finishDateAndTime: Date;
    public imageUrls: string[];
}