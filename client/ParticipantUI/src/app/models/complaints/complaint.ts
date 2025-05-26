import { ComplaintBasic } from "./complaintBasic";
import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class Complaint extends ComplaintBasic {
  public accusedUserId!: number;
  public accusedAuctionId?: number | null;
  public accusedCommentId?: number | null;
  public accusedUserFeedbackId?: number | null;
  public complaintText!: string;
  public type!: ComplaintTypeEnum;
  public moderatorConclusion?: string | null;
}
