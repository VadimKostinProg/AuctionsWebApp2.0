import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { UserProfileInfo } from "../models/user-profiles/userProfileInfo";
import { ResetPasswordModel } from "../models/user-profiles/resetPasswordModel";

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/users`;

  constructor(private readonly httpClient: HttpClient) { }

  getUserProfile(userId: number): Observable<ServiceResult<UserProfileInfo>> {
    return this.httpClient.get<ServiceResult<UserProfileInfo>>(`${this.baseUrl}/${userId}`);
  }

  resetPassword(request: ResetPasswordModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/password`, request);
  }

  deleteProfile(): Observable<ServiceMessage> {
    return this.httpClient.delete<ServiceMessage>(this.baseUrl);
  }
}
