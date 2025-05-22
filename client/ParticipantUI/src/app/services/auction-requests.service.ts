import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { PostAuctionRequest } from "../models/auction-requests/postAuctionRequest";
import { Observable } from "rxjs";
import { ServiceMessage } from "../models/shared/serviceResult";

@Injectable({
  providedIn: 'root'
})
export class AuctionRequestsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-requests`;

  constructor(private readonly httpClient: HttpClient) { }

  postAuctionRequest(auctionRequest: PostAuctionRequest): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(this.baseUrl, auctionRequest);
  }
}
