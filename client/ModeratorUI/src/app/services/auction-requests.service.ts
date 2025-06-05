import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { AuctionRequest } from "../models/auction-requests/auctionRequest";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { AuctionRequestStatusEnum } from "../models/auction-requests/auctionRequestStatusEnum";

@Injectable({
  providedIn: 'root'
})
export class AuctionRequestsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-requests`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionRequestDetails(id: number): Observable<ServiceResult<AuctionRequest>> {
    return this.httpClient.get<ServiceResult<AuctionRequest>>(`${this.baseUrl}/${id}`);
  }

  approveAuctionRequest(id: number): Observable<ServiceMessage> {
    const body = { auctionRequestId: id };

    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/approve`, body);
  }

  declineAuctionRequest(id: number, reason: string): Observable<ServiceMessage> {
    const body = {
      auctionRequestId: id,
      reason: reason
    };

    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/decline`, body);
  }

  getAuctionRequestsDataTableApiUrl() {
    return this.baseUrl;
  }

  getAuctionReuqestsDataTableOptions() {
    return {
      id: 'auctionRequests',
      title: null,
      resourceName: 'auction request',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not auction request matching these filters.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/auction-requests/$routeParam$',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Lot title',
          dataPropName: 'lotTitle',
          isOrderable: false
        },
        {
          title: 'Requested Auction Time',
          dataPropName: 'requestedAuctionTime',
          isOrderable: false
        },
        {
          title: 'Start Price',
          dataPropName: 'startPrice',
          isOrderable: false,
          transformAction: (value) => `$${value}`
        },
        {
          title: 'Status',
          dataPropName: 'status',
          isOrderable: false,
          transformAction: (value: AuctionRequestStatusEnum) => {
            switch (value) {
              case AuctionRequestStatusEnum.Approved:
                return 'Approved';
              case AuctionRequestStatusEnum.CanceledByUser:
                return 'Canceled by User';
              case AuctionRequestStatusEnum.Declined:
                return 'Declined';
              case AuctionRequestStatusEnum.Pending:
                return 'Pending';
            }
          }
        }
      ]
    } as DataTableOptionsModel;
  }
}
