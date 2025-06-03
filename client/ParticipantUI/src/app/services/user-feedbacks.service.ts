import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { PaginatedList } from "../models/shared/paginatedList";
import { UserFeedback } from "../models/user-profiles/userFeedback";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PostUserFeedback } from "../models/user-profiles/postUserFeedback";

@Injectable({
  providedIn: 'root'
})
export class UserFeedbacksService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/users`;

  constructor(private readonly httpClient: HttpClient) { }

  getUserFeedbacks(userId: number, pageNumber: number, pageSize: number): Observable<ServiceResult<PaginatedList<UserFeedback>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<UserFeedback>>>(`${this.baseUrl}/${userId}/feedbacks`, { params });
  }

  postUserFeedback(userFeedback: PostUserFeedback): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}/feedbacks`, userFeedback);
  }

  deleteUserFeedback(userFeedbackId: number): Observable<ServiceMessage> {
    return this.httpClient.delete<ServiceMessage>(`${this.baseUrl}/feedbacks/${userFeedbackId}`);
  }
}
