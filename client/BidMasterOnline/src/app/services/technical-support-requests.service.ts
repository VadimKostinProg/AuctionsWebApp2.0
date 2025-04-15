import { Injectable } from '@angular/core';
import { DataTableOptionsModel } from '../models/dataTableOptionsModel';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { TechnicalSupportRequestModel } from '../models/technicalSupportRequestModel';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TechnicalSupportRequestsService {

  baseUrl: string = `${environment.apiUrl}/api/v1/technical-support/requests`;

  constructor(private readonly httpClient: HttpClient) { }

  getTechnicalSupportRequestById(requestId: string): Observable<TechnicalSupportRequestModel> {
    return this.httpClient.get<TechnicalSupportRequestModel>(`${this.baseUrl}/${requestId}`);
  }

  setTechnicalSupportRequest(requestText: string): Observable<any> {
    return this.httpClient.post(`${this.baseUrl}`, { requestText: requestText });
  }

  handleTechnicalSupportRequest(requestId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/${requestId}`, null);
  }

  getDataTableApiUrl() {
    return `${this.baseUrl}/list`;
  }

  getDataTableOptions() {
    const options = {
      title: 'Technical support requests',
      resourceName: 'technical support request',
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
      emptyListDisplayLabel: 'The list of technical support requests is empty.',
      columnSettings: [
        {
          title: 'User',
          dataPropName: 'username',
          isOrderable: false,
          isLink: true,
          pageLink: '/profile',
          linkQueryParam: 'userId',
          linkQueryDataPropName: 'userId'
        },
        {
          title: 'DateAndTime',
          dataPropName: 'dateAndTime',
          isOrderable: false
        }
      ]
    } as DataTableOptionsModel;

    return options;
  }
}
