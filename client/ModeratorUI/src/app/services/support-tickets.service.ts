import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { SupportTicketStatusEnum } from "../models/support-tickets/supportTicketStatusEnum";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { SupportTicket } from "../models/support-tickets/supportTicket";
import { AssignSupportTicketModel } from "../models/support-tickets/assignSupportTicketModel";
import { CompleteSupportTicketModel } from "../models/support-tickets/completeSupportTicketModel";

@Injectable({
  providedIn: 'root'
})
export class SupportTicketsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/support-tickets`;

  constructor(private readonly httpClient: HttpClient) { }

  getSupportTicketDetails(supportTicketId: number): Observable<ServiceResult<SupportTicket>> {
    return this.httpClient.get<ServiceResult<SupportTicket>>(`${this.baseUrl}/${supportTicketId}`);
  }

  assignSupportTicket(assignSupportTicket: AssignSupportTicketModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/assign`, assignSupportTicket);
  }

  completeSupportTicket(completeSupportTicket: CompleteSupportTicketModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/complete`, completeSupportTicket);
  }

  getDataTableApiUrl() {
    return this.baseUrl;
  }

  getDataTableOptions() {
    return {
      id: 'supportTickets',
      title: null,
      resourceName: 'supportTicket',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not support tickets matching your filters.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/support-tickets/$routeParam$',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Submitted user',
          dataPropName: 'submittedUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/users/$routeParam$',
          linkRouteParamName: 'userId',
        },
        {
          title: 'Title',
          dataPropName: 'title',
          isOrderable: false
        },
        {
          title: 'Submitted time',
          dataPropName: 'submittedTime',
          isOrderable: false
        },
        {
          title: 'Assigned on',
          dataPropName: 'moderatorName',
          isOrderable: false,
          isLink: true,
          pageLink: '/users/$routeParam$',
          linkRouteParamName: 'moderatorId',
        },
        {
          title: 'Status',
          dataPropName: 'status',
          isOrderable: false,
          transformAction: (value: SupportTicketStatusEnum) => {
            switch (value) {
              case SupportTicketStatusEnum.Pending:
                return 'Pending';
              case SupportTicketStatusEnum.Active:
                return 'Active';
              case SupportTicketStatusEnum.Completed:
                return 'Completed';
            }
          }
        },
      ]
    } as DataTableOptionsModel;
  }
}
