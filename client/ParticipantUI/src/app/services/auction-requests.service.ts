import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { PostAuctionRequest } from "../models/auction-requests/postAuctionRequest";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { AuctionRequestStatusEnum } from "../models/auction-requests/auctionRequestStatusEnum";
import { AuctionRequest } from "../models/auction-requests/auctionRequest";

@Injectable({
  providedIn: 'root'
})
export class AuctionRequestsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-requests`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionRequestDetails(id: number): Observable<ServiceResult<AuctionRequest>> {
    return this.httpClient.get<ServiceResult<AuctionRequest>>(`${this.baseUrl}/${id}`);
  }

  postAuctionRequest(auctionRequest: PostAuctionRequest): Observable<ServiceMessage> {
    const form = new FormData();

    for (const image of auctionRequest.images)
      form.append('images', image);

    form.append('auctionCategoryId', auctionRequest.auctionCategoryId.toString());
    form.append('auctionTypeId', auctionRequest.auctionTypeId.toString());
    form.append('auctionFinishMethodId', auctionRequest.auctionFinishMethodId.toString());
    form.append('lotTitle', auctionRequest.lotTitle);
    form.append('lotDescription', auctionRequest.lotDescription);
    form.append('requestedAuctionTime', auctionRequest.requestedAuctionTime);
    form.append('startPrice', auctionRequest.startPrice.toString());
    form.append('bidAmountInterval', auctionRequest.bidAmountInterval.toString());

    if (auctionRequest.requestedStartTime) {
      form.append('requestedStartTime', auctionRequest.requestedStartTime.toUTCString());
    }

    if (auctionRequest.finishTimeInterval) {
      form.append('finishTimeInterval', auctionRequest.finishTimeInterval);
    }

    if (auctionRequest.aimPrice) {
      form.append('aimPrice', auctionRequest.aimPrice.toString());
    }

    return this.httpClient.post<ServiceMessage>(this.baseUrl, form);
  }

  cancelAuctionRequest(id: number): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/${id}/cancel`, null);
  }

  getAuctionRequestsHistoryDataTableApiUrl() {
    return `${this.baseUrl}/own`
  }

  getAuctionRequestsHistoryDataTableOptions() {
    return {
      id: 'auctionRequests',
      title: 'Auction Requests History',
      resourceName: 'auctionRequest',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not any submitted auction request. Create a first one!',
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
