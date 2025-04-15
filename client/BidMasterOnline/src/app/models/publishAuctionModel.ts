export class PublishAuctionModel {
    public images: File[];
    public name: string;
    public categoryId: string;
    public lotDescription: string;
    public finishType: string;
    public auctionTime: string;
    public finishTimeInterval: string | null;
    public startPrice: number;
}