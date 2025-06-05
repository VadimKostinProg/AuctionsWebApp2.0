import { AuctionComment } from "../auctions/auctionComment";
import { BaseEntityModel } from "../baseEntityModel";
import { UserFeedback } from "../users/userFeedback";
import { ComplaintStatusEnum } from "./complaintStatusEnum";
import { ComplaintTypeEnum } from "./complaintTypeEnum";

export class Complaint extends BaseEntityModel {
  public accusingUserId!: number;
  public accusedUserId!: number;
  public accusedAuctionId?: number | null;
  public accusedCommentId?: number | null;
  public accusedUserFeedbackId?: number | null;
  public moderatorId?: number | null;
  public title!: string;
  public complaintText!: string;
  public type!: ComplaintTypeEnum;
  public status!: ComplaintStatusEnum;
  public moderatorConclusion?: string | null;
  public moderatorName?: string | null;
  public accusingUserName!: string;
  public accusedUsername!: string;
  public accusedAuctionName?: string;
  public accusedComment?: AuctionComment | null;
  public accusedUserFeedback?: UserFeedback | null;
}
