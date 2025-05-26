import { ComplaintStatusEnum } from "./complaintStatusEnum";

export class ComplaintBasic {
  public id!: number;
  public title!: string;
  public createdAt!: Date;
  public status!: ComplaintStatusEnum;
}
