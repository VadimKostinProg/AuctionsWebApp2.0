import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { PostComplaint } from "../models/complaints/postComplaint";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Injectable } from "@angular/core";
import { PaginatedList } from "../models/shared/paginatedList";
import { ComplaintBasic } from "../models/complaints/complaintBasic";
import { Complaint } from "../models/complaints/complaint";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { ComplaintStatusEnum } from "../models/complaints/complaintStatusEnum";
import { DatePipe } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class ComplaintsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/complaints`;

  constructor(private readonly httpClient: HttpClient,
    private readonly datePipe: DatePipe) { }

  getUserComplaints(pageNumber: number, pageSize: number): Observable<ServiceResult<PaginatedList<ComplaintBasic>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<ComplaintBasic>>>(this.baseUrl, { params });
  }

  getComplaintDetails(complaintId: number): Observable<ServiceResult<Complaint>> {
    return this.httpClient.get<ServiceResult<Complaint>>(`${this.baseUrl}/${complaintId}`);
  }

  postComplaint(complaint: PostComplaint): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(this.baseUrl, complaint);
  }

  getComplaintsListUrl() {
    return this.baseUrl;
  }

  getComplaintsDataTableOptions(): DataTableOptionsModel {
    return {
      id: 'complaints',
      title: 'My complaints',
      resourceName: 'complaint',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'You have not submitted any complaint yet.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: false,
          isLink: true,
          pageLink: '/complaints/$routeParam$',
          linkRouteParamName: 'id'
        },
        {
          title: 'Created At',
          dataPropName: 'createdAt',
          isOrderable: false,
          transformAction: (value) => this.datePipe.transform(value, 'MM-dd-yyyy HH:mm')
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
          transformAction: (status: ComplaintStatusEnum) => {
            switch (status) {
              case ComplaintStatusEnum.Active:
                return 'Active';
              case ComplaintStatusEnum.Pending:
                return 'Pending';
              case ComplaintStatusEnum.Completed:
                return 'Completed';
            }
          }
        },
      ]
    } as DataTableOptionsModel;
  }
}
