import { UserModel } from "./userModel";

export class ProfileModel extends UserModel {
    public totalAuctions: number;
    public totalWins: number;
}