import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class SetComplaintModel {
    public accusedUserId: string;
    public auctionId: string;
    public commentId: string | null;
    public complaintType: ComplaintTypeEnum;
    public complaintText: string;
}