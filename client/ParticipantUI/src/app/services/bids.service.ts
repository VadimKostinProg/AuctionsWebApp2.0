import { Injectable } from "@angular/core";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { PaginatedList } from "../models/shared/paginatedList";
import { UserBid } from "../models/bids/userBid";
import { HttpClient, HttpParams } from "@angular/common/http";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { AuctionBid } from "../models/bids/auctionBid";
import { PostBid } from "../models/bids/postBid";

@Injectable({
  providedIn: 'root'
})
export class BidsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getUserBids(pageNumber: number, pageSize: number): Observable<ServiceResult<PaginatedList<UserBid>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<UserBid>>>(`${this.baseUrl}/bids`, { params });
  }

  getAuctionBids(auctionId: number, pageNumber: number, pageSize: number):
    Observable<ServiceResult<PaginatedList<AuctionBid>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionBid>>>(`${this.baseUrl}/${auctionId}/bids`, { params });
  }

  postBid(bid: PostBid): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}/bids`, bid);
  }

  getAuctionBidsApiUrl(auctionId: number) {
    return `${this.baseUrl}/${auctionId}/bids`;
  }

  getAuctionBidsDataTableOptions(): DataTableOptionsModel {
    return {
      id: 'bids',
      title: 'Bids',
      resourceName: 'bid',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not placed bids at this auction.',
      columnSettings: [
        {
          title: 'User',
          dataPropName: 'bidderUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/profile',
          linkRouteParamName: 'bidderId'
        },
        {
          title: 'Date and time',
          dataPropName: 'time',
          isOrderable: false
        },
        {
          title: 'Amount',
          dataPropName: 'amount',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;
  }
}
