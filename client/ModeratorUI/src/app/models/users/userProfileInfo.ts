import { BaseEntityModel } from "../baseEntityModel";
import { UserStatusEnum } from "./userStatusEnum";

export class UserProfileInfo extends BaseEntityModel {
  public username!: string;
  public fullName!: string;
  public email!: string;
  public status!: UserStatusEnum;
  public role!: string;
  public dateOfBirth!: Date
  public averageScore?: number | null
  public imageUrl!: string;
  public totalAuctions!: number;
  public completedAuctions!: number;
  public totalWins!: number;
  public isEmailConfirmed!: boolean;
  public isPaymentMethodAttached!: boolean;
  public blockingReason?: string | null;
  public unblockDateTime?: Date | null;
}
