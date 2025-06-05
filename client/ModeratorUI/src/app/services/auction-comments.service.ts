import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { PaginatedList } from "../models/shared/paginatedList";
import { HttpClient, HttpParams } from "@angular/common/http";
import { AuctionComment } from "../models/auctions/auctionComment";

@Injectable({
  providedIn: 'root'
})
export class AuctionCommentsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getCommentsForAuction(auctionId: number, pageNumber: number, pageSize: number)
    : Observable<ServiceResult<PaginatedList<AuctionComment>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionComment>>>(`${this.baseUrl}/${auctionId}/comments`, { params });
  }

  deleteComment(commentId: number): Observable<ServiceMessage> {
    return this.httpClient.delete<ServiceMessage>(`${this.baseUrl}/comments/${commentId}`);
  }
}
