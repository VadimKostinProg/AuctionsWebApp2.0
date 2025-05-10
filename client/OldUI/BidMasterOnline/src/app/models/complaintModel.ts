import { CommentModel } from "./commentModel";
import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class ComplaintModel {
    public id: string;
    public accusingUserId: string;
    public accusingUsername: string;
    public accusedUserId: string;
    public accusedUsername: string;
    public auctionId: string;
    public auctionName: string;
    public commentId: string | null;
    public comment: CommentModel | null;
    public complaintType: ComplaintTypeEnum;
    public complaintTypeDescription: string;
    public dateAndTime: string;
    public complaintText: string;
}