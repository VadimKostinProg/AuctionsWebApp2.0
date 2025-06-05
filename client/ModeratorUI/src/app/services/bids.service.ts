import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";

@Injectable({
  providedIn: 'root'
})
export class BidsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}`;

  getDataTableApiUrl(auctionId: number) {
    return `${this.baseUrl}/auctions/${auctionId}/bids`;
  }

  getDataTableOptions() {
    return {
      id: 'bids',
      title: 'Auction bids',
      resourceName: 'bid',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not placed bids on this auction.',
      columnSettings: [
        {
          title: 'User',
          dataPropName: 'bidderUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/users/$routeParam$',
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
          isOrderable: false,
          transformAction: (value) => `$${value}`
        },
      ]
    } as DataTableOptionsModel;
  }

  getUserBidsHistoryDataTableApiUrl(userId: number) {
    return `${this.baseUrl}/users/${userId}/bids`;
  }

  getUserBidsHistoryDataTableOptions() {
    return {
      id: 'bids',
      title: 'Bids History',
      resourceName: 'bid',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not bids placed by this user.',
      columnSettings: [
        {
          title: 'Auction Id',
          dataPropName: 'auctionId',
          isOrderable: false,
          isLink: true,
          pageLink: '/auctions/$routeParam$',
          linkRouteParamName: 'auctionId',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Auction name',
          dataPropName: 'auctionName',
          isOrderable: false,
        },
        {
          title: 'Date and time',
          dataPropName: 'time',
          isOrderable: false
        },
        {
          title: 'Amount',
          dataPropName: 'amount',
          isOrderable: false,
          transformAction: (value) => `$${value}`
        },
      ]
    } as DataTableOptionsModel;
  }
}
