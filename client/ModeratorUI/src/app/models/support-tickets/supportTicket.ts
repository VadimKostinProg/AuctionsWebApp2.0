import { BaseEntityModel } from "../baseEntityModel";
import { SupportTicketStatusEnum } from "./supportTicketStatusEnum";

export class SupportTicket extends BaseEntityModel {
  public userId!: number;
  public moderatorId!: number;
  public title!: number;
  public text!: number;
  public status!: SupportTicketStatusEnum;
  public moderatorComment?: string | null;
  public submitUsername!: string;
  public moderatorName!: string;
}
