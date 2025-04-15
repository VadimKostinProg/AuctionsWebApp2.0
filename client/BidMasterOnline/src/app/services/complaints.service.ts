import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SetComplaintModel } from '../models/setComplaintModel';
import { Observable } from 'rxjs';
import { ComplaintTypeEnum } from '../models/complaintTypeEnum';
import { FormControl, FormGroup } from '@angular/forms';
import { DataTableOptionsModel } from '../models/dataTableOptionsModel';
import { ComplaintModel } from '../models/complaintModel';

@Injectable({
  providedIn: 'root'
})
export class ComplaintsService {

  baseUrl: string = `${environment.apiUrl}/api/v1/technical-support/complaints`;

  constructor(private readonly httpClient: HttpClient) { }

  getComplaintById(complaintId: string): Observable<ComplaintModel> {
    return this.httpClient.get<ComplaintModel>(`${this.baseUrl}/${complaintId}`);
  }

  setComplaint(complaint: SetComplaintModel): Observable<any> {
    return this.httpClient.post(this.baseUrl, complaint);
  }

  handleComplaint(complaintId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/${complaintId}`, null);
  }

  getDataTableApiUrl() {
    return `${this.baseUrl}/list`;
  }

  getDataTableOptions() {
    const options = {
      title: 'Complaints',
      resourceName: 'complaint',
      showIndexColumn: true,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: {
        actionName: 'View',
        form: null,
        message: null,
        properties: null,
      },
      emptyListDisplayLabel: 'The list of complaints is empty.',
      columnSettings: [
        {
          title: 'Accusing user',
          dataPropName: 'accusingUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/profile',
          linkQueryParam: 'userId',
          linkQueryDataPropName: 'accusingUserId'
        },
        {
          title: 'Accused user',
          dataPropName: 'accusedUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/profile',
          linkQueryParam: 'userId',
          linkQueryDataPropName: 'accusedUserId'
        },
        {
          title: 'Auction',
          dataPropName: 'auctionName',
          isOrderable: false,
          isLink: true,
          pageLink: '/auction-details',
          linkQueryParam: 'auctionId',
          linkQueryDataPropName: 'auctionId'
        },
        {
          title: 'Date and time',
          dataPropName: 'dateAndTime',
          isOrderable: false
        }
      ]
    } as DataTableOptionsModel;

    return options;
  }
}
