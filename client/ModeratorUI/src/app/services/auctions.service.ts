import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { AuctionStatusEnum } from "../models/auctions/auctionStatusEnum";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Auction } from "../models/auctions/auction";
import { CancelAuctionModel } from "../models/auctions/cancelAuctionModel";

@Injectable({
  providedIn: 'root'
})
export class AuctionsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionDetails(id: number): Observable<ServiceResult<Auction>> {
    return this.httpClient.get<ServiceResult<Auction>>(`${this.baseUrl}/${id}`);
  }

  cancelAuction(cancelAucionRequest: CancelAuctionModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/cancel`, cancelAucionRequest);
  }

  recoverAuction(auctionId: number): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/recover`, { auctionId: auctionId });
  }

  getAuctionsDataTableApiUrl() {
    return this.baseUrl;
  }

  getAuctionsDataTableOptions() {
    return {
      id: 'auctions',
      title: null,
      resourceName: 'auction',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not auctions matching these filters.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: true,
          isLink: true,
          pageLink: '/auctions/$routeParam$',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Lot title',
          dataPropName: 'lotTitle',
          isOrderable: true
        },
        {
          title: 'Category',
          dataPropName: 'category',
          isOrderable: false
        },
        {
          title: 'Type',
          dataPropName: 'type',
          isOrderable: false
        },
        {
          title: 'Start Time',
          dataPropName: 'startTime',
          isOrderable: true
        },
        {
          title: 'Finish Time',
          dataPropName: 'finishTime',
          isOrderable: true
        },
        {
          title: 'Status',
          dataPropName: 'status',
          isOrderable: false,
          transformAction: (value: AuctionStatusEnum) => {
            switch (value) {
              case AuctionStatusEnum.Active:
                return 'Active';
              case AuctionStatusEnum.CancelledByAuctionist:
                return 'Canceled by Auctionist';
              case AuctionStatusEnum.CancelledByModerator:
                return 'Canceled by Moderator';
              case AuctionStatusEnum.Pending:
                return 'Pending';
              case AuctionStatusEnum.Finished:
                return 'Finished';
            }
          }
        }
      ]
    } as DataTableOptionsModel;
  }
}
