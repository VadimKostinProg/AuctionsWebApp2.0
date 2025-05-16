import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuctionBasic } from "../models/auctions/AuctionBasic";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { PaginatedList } from "../models/shared/paginatedList";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Auction } from "../models/auctions/Auction";
import { CancelAuction } from "../models/auctions/CancelAuction";

@Injectable({
  providedIn: 'root'
})
export class AuctionsService {

  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getPopularAuctions(): Observable<ServiceResult<PaginatedList<AuctionBasic>>> {
    // todo: add query params

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionBasic>>>(this.baseUrl);
  }

  getAuctionDetailsById(auctionId: number): Observable<ServiceResult<Auction>> {
    return this.httpClient.get<ServiceResult<Auction>>(`${this.baseUrl}/${auctionId}`);
  }

  cancelAuction(request: CancelAuction): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(this.baseUrl, request);
  }
}
