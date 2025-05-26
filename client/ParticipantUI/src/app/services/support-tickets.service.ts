import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Observable } from "rxjs";
import { PostSupportTicket } from "../models/support-tickets/postSupportTicket";
import { PaginatedList } from "../models/shared/paginatedList";
import { SupportTicketBasic } from "../models/support-tickets/supportTicketBasic";
import { SupportTicket } from "../models/support-tickets/supportTicket";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { SupportTicketStatusEnum } from "../models/support-tickets/supportTicketStatusEnum";

@Injectable({
  providedIn: 'root'
})
export class SupportTicketsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/support-tickets`;

  constructor(private readonly httpClient: HttpClient) { }

  getOwnSupportTickets(pageNumber: number, pageSize: number)
    : Observable<ServiceResult<PaginatedList<SupportTicketBasic>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<SupportTicketBasic>>>(this.baseUrl, { params });
  }

  getSupportTicketDetails(supportTicketId: number): Observable<ServiceResult<SupportTicket>> {
    return this.httpClient.get<ServiceResult<SupportTicket>>(`${this.baseUrl}/${supportTicketId}`);
  }

  postSupportTicket(supportTicket: PostSupportTicket): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(this.baseUrl, supportTicket);
  }

  getSupportTicketsListUrl() {
    return this.baseUrl;
  }

  getSupportTicketsDataTableOptions(): DataTableOptionsModel {
    return {
      id: 'supportTickets',
      title: 'My support tickets',
      resourceName: 'supportTicket',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'You have not submitted any support ticket yet.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/technical-support/support-tickets/$routeParam$',
          linkRouteParamName: 'id'
        },
        {
          title: 'Created At',
          dataPropName: 'createdAt',
          isOrderable: false
        },
        {
          title: 'Title',
          dataPropName: 'title',
          isOrderable: false
        },
        {
          title: 'Status',
          dataPropName: 'status',
          isOrderable: false,
          transformAction: (status: SupportTicketStatusEnum) => {
            switch (status) {
              case SupportTicketStatusEnum.Active:
                return 'Active';
              case SupportTicketStatusEnum.Pending:
                return 'Pending';
              case SupportTicketStatusEnum.Completed:
                return 'Completed';
            }
          }
        },
      ]
    } as DataTableOptionsModel;
  }
}
