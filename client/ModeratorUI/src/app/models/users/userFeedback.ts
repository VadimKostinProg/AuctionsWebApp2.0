import { BaseEntityModel } from "../baseEntityModel";

export class UserFeedback extends BaseEntityModel {
  public fromUserId!: number;
  public toUserId!: number;
  public score!: number;
  public comment!: string;
  public fromUsername!: string;
  public toUsername!: string;
}
