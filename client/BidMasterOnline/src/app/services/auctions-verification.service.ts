import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DataTableOptionsModel } from '../models/dataTableOptionsModel';
import { FormInputTypeEnum } from '../models/formInputTypeEnum';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuctionDetailsModel } from '../models/auctionDetailsModel';
import { Observable } from 'rxjs';
import { RejectAuctionModel } from '../models/rejectAuctionModel';

@Injectable({
  providedIn: 'root'
})
export class AuctionsVerificationService {

  baseUrl: string = `${environment.apiUrl}/api/v1/auctions/not-approved`;

  constructor(private readonly httpClient: HttpClient) {

  }

  getNotApporvedAuctionDetailsById(auctionId: string): Observable<AuctionDetailsModel> {
    return this.httpClient.get<AuctionDetailsModel>(`${this.baseUrl}/${auctionId}/details`);
  }

  approveAuction(auctionId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/${auctionId}/approve`, null);
  }

  rejectAuction(model: RejectAuctionModel): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/reject`, model);
  }

  getNotApporovedAuctionsDataTableApiUrl() {
    return `${this.baseUrl}/list`;
  }

  getNotApporovedAuctionsDataTableOptions() {
    var options = {
      title: 'Auction creation requests',
      resourceName: 'auction',
      showIndexColumn: true,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: {
        actionName: 'Details',
        form: null,
        message: null,
        properties: null,
      },
      emptyListDisplayLabel: 'The list of auction creation requests is empty.',
      columnSettings: [
        {
          title: 'Name',
          dataPropName: 'name',
          isOrderable: false
        },
        {
          title: 'Category',
          dataPropName: 'category',
          isOrderable: false
        },
        {
          title: 'Auctionist',
          dataPropName: 'auctionist',
          isOrderable: false,
          isLink: true,
          pageLink: '/profile',
          linkQueryParam: 'userId',
          linkQueryDataPropName: 'auctionistId'
        },
        {
          title: 'Auction time',
          dataPropName: 'auctionTime',
          isOrderable: false
        },
        {
          title: 'Start price',
          dataPropName: 'startPrice',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;

    return options;
  }
}
