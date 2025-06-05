import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Complaint } from "../models/complaints/complaint";
import { AssignComplaintModel } from "../models/complaints/assignComplaintModel";
import { CompleteComplaintModel } from "../models/complaints/completeComplaintModel";
import { ComplaintStatusEnum } from "../models/complaints/complaintStatusEnum";

@Injectable({
  providedIn: 'root'
})
export class ComplaintsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/complaints`;

  constructor(private readonly httpClient: HttpClient) { }

  getComplaintDetails(complaintId: number): Observable<ServiceResult<Complaint>> {
    return this.httpClient.get<ServiceResult<Complaint>>(`${this.baseUrl}/${complaintId}`);
  }

  assignComplaint(assignComplaint: AssignComplaintModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/assign`, assignComplaint);
  }

  completeComplaint(completeComplaint: CompleteComplaintModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/complete`, completeComplaint);
  }

  getDataTableApiUrl() {
    return this.baseUrl;
  }

  getDataTableOptions() {
    return {
      id: 'complaints',
      title: null,
      resourceName: 'complaint',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not complaints matching your filters.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/complaints/$routeParam$',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Accusing user',
          dataPropName: 'accusingUsername',
          isOrderable: false,
          isLink: true,
          pageLink: '/users/$routeParam$',
          linkRouteParamName: 'accusingUserId',
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
          transformAction: (value: ComplaintStatusEnum) => {
            switch (value) {
              case ComplaintStatusEnum.Pending:
                return 'Pending';
              case ComplaintStatusEnum.Active:
                return 'Active';
              case ComplaintStatusEnum.Completed:
                return 'Completed';
            }
          }
        },
      ]
    } as DataTableOptionsModel;
  }
}
