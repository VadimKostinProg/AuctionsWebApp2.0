import { ComplaintBasic } from "./complaintBasic";
import { ComplaintStatusEnum } from "./complaintStatusEnum";
import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class Complaint extends ComplaintBasic {
  public accusedUserId!: number;
  public accusedAuctionId?: number | null;
  public accusedCommentId?: number | null;
  public accusedUserFeedbackId?: number | null;
  public type!: ComplaintTypeEnum;
  public status!: ComplaintStatusEnum;
  public moderatorConclusion?: string | null;
}
