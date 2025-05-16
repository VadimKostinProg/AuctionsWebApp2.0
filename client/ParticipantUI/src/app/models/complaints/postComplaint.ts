import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class PostComplaint {
  public accusedUserId!: number;
  public accusedAuctionId?: number | null;
  public accusedCommentId?: number | null;
  public accusedUserFeedbackId?: number | null;
  public complaintText!: string;
  public type!: ComplaintTypeEnum;
}
