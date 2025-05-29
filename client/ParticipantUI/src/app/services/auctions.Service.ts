import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuctionBasic } from "../models/auctions/AuctionBasic";
import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { PaginatedList } from "../models/shared/paginatedList";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Auction } from "../models/auctions/Auction";
import { CancelAuction } from "../models/auctions/CancelAuction";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { UserAuction } from "../models/auctions/userAuction";

@Injectable({
  providedIn: 'root'
})
export class AuctionsService {

  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getPopularAuctions(): Observable<ServiceResult<PaginatedList<AuctionBasic>>> {
    const params = new HttpParams()
      .set('sortBy', 'popularity')
      .set('status', 'active')
      .set('pageNumber', 1)
      .set('pageSize', 15);

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionBasic>>>(this.baseUrl, { params });
  }

  getFinishingAuctions(): Observable<ServiceResult<PaginatedList<AuctionBasic>>> {
    const params = new HttpParams()
      .set('sortBy', 'finishTime')
      .set('status', 'active')
      .set('pageNumber', 1)
      .set('pageSize', 15);

    return this.httpClient.get<ServiceResult<PaginatedList<AuctionBasic>>>(this.baseUrl, { params });
  }

  getUserAuctions(pageNumber: number, pageSize: number): Observable<ServiceResult<PaginatedList<UserAuction>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<UserAuction>>>(`${this.baseUrl}/own`, { params });
  }

  getAuctionDetailsById(auctionId: number): Observable<ServiceResult<Auction>> {
    return this.httpClient.get<ServiceResult<Auction>>(`${this.baseUrl}/${auctionId}`);
  }

  cancelAuction(request: CancelAuction): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(this.baseUrl, request);
  }

  getAuctionsHistoryDataTableApiUrl() {
    return `${this.baseUrl}/own`;
  }

  getAuctionsHistoryDataTableOptions() {
    return {
      id: 'auctions',
      title: 'Auctions History',
      resourceName: 'auction',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'You have not had any auction. Submit an auction request to start one!',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/auctions/$routeParam$/details',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Name',
          dataPropName: 'lotTitle',
          isOrderable: false
        },
        {
          title: 'Category',
          dataPropName: 'category',
          isOrderable: false
        },
        {
          title: 'Start time',
          dataPropName: 'startTime',
          isOrderable: false
        },
        {
          title: 'Current/Completed price',
          dataPropName: 'currentPrice',
          isOrderable: false
        },
        {
          title: 'Score',
          dataPropName: 'averageScore',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;
  }
}
