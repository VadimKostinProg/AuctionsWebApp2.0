import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuctionComment } from '../models/auctions/AuctionComment';
import { PostComment } from '../models/auctions/PostComment';
import { environment } from '../../environments/environment';
import { PaginatedList } from '../models/shared/paginatedList';
import { ServiceMessage, ServiceResult } from '../models/shared/serviceResult';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {

  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) {

  }

  getCommentsForAuction(auctionId: number, pageNumber: number, pageSize: number)
    : Observable<ServiceResult<PaginatedList<AuctionComment>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionComment>>>(`${this.baseUrl}/${auctionId}/comments`, { params });
  }

  postNewComment(comment: PostComment): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}/comments`, comment);
  }

  deleteComment(commentId: number): Observable<ServiceMessage> {
    return this.httpClient.delete<ServiceMessage>(`${this.baseUrl}/comments/${commentId}`);
  }
}
