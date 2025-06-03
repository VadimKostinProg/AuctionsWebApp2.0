import { SupportTicketBasic } from "./supportTicketBasic";
import { SupportTicketStatusEnum } from "./supportTicketStatusEnum";

export class SupportTicket extends SupportTicketBasic {
  public text!: string;
  public status!: SupportTicketStatusEnum;
  public moderatorComment?: string | null;
}
