import { UserStatusEnum } from "../users/userStatusEnum";

export class UserProfileInfo {
  public id!: number;
  public username!: number;
  public fullName!: string;
  public dateOfBirth!: string;
  public email!: string;
  public averageScore?: number | null;
  public status!: UserStatusEnum;
  public imageUrl!: string;
  public totalAuctions!: number;
  public completedAuctions!: number;
  public totalWins!: number;
  public isEmailConfirmed?: string | null;
  public isPaymentMethodAttached?: boolean | null;
  public unblockDateTime?: Date | null;
}
