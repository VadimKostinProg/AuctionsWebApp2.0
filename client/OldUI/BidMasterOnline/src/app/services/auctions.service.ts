import { Injectable } from '@angular/core';
import { AuctionModel } from '../models/auctionModel';
import { Params } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { PublishAuctionModel } from '../models/publishAuctionModel';
import { ListModel } from '../models/listModel';
import { AuctionDetailsModel } from '../models/auctionDetailsModel';
import { DataTableOptionsModel } from '../models/dataTableOptionsModel';
import { SetBidModel } from '../models/setBidModel';
import { CancelAuctionModel } from '../models/cancelAuctionModel';
import { AuctionParticipantEnum } from '../models/auctionParticipantEnum';

@Injectable({
  providedIn: 'root'
})
export class AuctionsService {

  baseUrl: string = `${environment.apiUrl}/api/v1/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionsList(specifications: Params): Observable<ListModel<AuctionModel>> {
    const params = new HttpParams({ fromObject: specifications });

    return this.httpClient.get<ListModel<AuctionModel>>(`${this.baseUrl}/list`, { params });
  }

  getAuctionDetailsById(auctionId: string): Observable<AuctionDetailsModel> {
    return this.httpClient.get<AuctionDetailsModel>(`${this.baseUrl}/${auctionId}/details`);
  }

  getPopularAuctions(): Observable<ListModel<AuctionModel>> {
    return this.httpClient.get<ListModel<AuctionModel>>(`${this.baseUrl}/list?status=Active&pageSize=10&pageNumber=1&sortField=popularity`);
  }

  getFinishingAuctions(): Observable<ListModel<AuctionModel>> {
    return this.httpClient.get<ListModel<AuctionModel>>(`${this.baseUrl}/list?status=Active&pageSize=10&pageNumber=1&sortField=dateAndTime`);
  }

  getFinishedAuctionsWithNotConfirmedOptions(participant: AuctionParticipantEnum): Observable<AuctionModel[]> {
    return this.httpClient.get<AuctionModel[]>(`${this.baseUrl}/not-confirmed?participant=${AuctionParticipantEnum[participant]}`);
  }

  setBidOnAuction(bid: SetBidModel): Observable<any> {
    return this.httpClient.post(`${this.baseUrl}/bids`, bid);
  }

  getAuctionBidsApiUrl(auctionId: string) {
    return `${this.baseUrl}/${auctionId}/bids`;
  }

  cancelLastBidOfAuction(auctionId: string): Observable<any> {
    return this.httpClient.delete(`${this.baseUrl}/${auctionId}/bids`);
  }

  getAuctionBidsDataTableOptions() {
    var options = {
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
          linkQueryParam: 'userId',
          linkQueryDataPropName: 'bidderId'
        },
        {
          title: 'Date and time',
          dataPropName: 'dateAndTime',
          isOrderable: false
        },
        {
          title: 'Amount',
          dataPropName: 'amount',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;

    return options;
  }

  publishAuction(auction: PublishAuctionModel): Observable<any> {
    var form = new FormData();

    for (const image of auction.images) {
      form.append(`images`, image);
    }

    form.append('name', auction.name);
    form.append('categoryId', auction.categoryId);
    form.append('lotDescription', auction.lotDescription);
    form.append('finishType', auction.finishType);
    form.append('auctionTime', auction.auctionTime);
    if (auction.finishTimeInterval) {
      form.append('finishTimeInterval', auction.finishTimeInterval);
    }
    form.append('startPrice', auction.startPrice.toString());

    return this.httpClient.post(`${this.baseUrl}`, form);
  }

  cancelAuction(model: CancelAuctionModel): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/cancel`, model);
  }

  cancelOwnAuction(auctionId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/own/${auctionId}/cancel`, null);
  }

  recoverAuction(auctionId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/${auctionId}/recover`, null);
  }

  getUsersAuctionsDataTableApiUrl() {
    return `${this.baseUrl}/list`;
  }

  getUsersAuctionsDataTableOptions(title: string) {
    var options = {
      title: title,
      resourceName: 'auction',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: '',
      columnSettings: [
        {
          title: 'Name',
          dataPropName: 'name',
          isOrderable: false,
          isLink: true,
          pageLink: '/auction-details',
          linkQueryParam: 'auctionId',
          linkQueryDataPropName: 'id'
        },
        {
          title: 'Date and time',
          dataPropName: 'finishDateAndTime',
          isOrderable: false
        },
        {
          title: 'Sell price',
          dataPropName: 'currentBid',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;

    return options;
  }
}