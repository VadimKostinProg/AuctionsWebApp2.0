import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SetCommentModel } from '../models/setCommentModel';
import { Observable } from 'rxjs';
import { CommentModel } from '../models/commentModel';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {

  baseUrl: string = `${environment.apiUrl}/api/v1/comments`;

  constructor(private readonly httpClient: HttpClient) {

  }

  getCommentsForAuction(auctionId: string): Observable<CommentModel[]> {
    return this.httpClient.get<CommentModel[]>(`${this.baseUrl}?auctionId=${auctionId}`);
  }

  setNewComment(comment: SetCommentModel): Observable<any> {
    return this.httpClient.post(this.baseUrl, comment);
  }

  deleteComment(commentId: string): Observable<any> {
    return this.httpClient.delete<any>(`${this.baseUrl}/${commentId}`);
  }

  deleteOwnComment(commentId: string): Observable<any> {
    return this.httpClient.delete<any>(`${this.baseUrl}/own/${commentId}`);
  }
}
