import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { PostComplaint } from "../models/complaints/postComplaint";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Injectable } from "@angular/core";
import { PaginatedList } from "../models/shared/paginatedList";
import { ComplaintBasic } from "../models/complaints/complaintBasic";
import { Complaint } from "../models/complaints/complaint";

@Injectable({
  providedIn: 'root'
})
export class ComplaintsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/complaints`;

  constructor(private readonly httpClient: HttpClient) { }

  getUserComplaints(pageNumber: number, pageSize: number): Observable<ServiceResult<PaginatedList<ComplaintBasic>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.httpClient.get<ServiceResult<PaginatedList<ComplaintBasic>>>(this.baseUrl, { params });
  }

  getComplaintById(complaintId: number): Observable<ServiceResult<Complaint>> {
    return this.httpClient.get<ServiceResult<Complaint>>(`${this.baseUrl}/${complaintId}`);
  }

  postComplaint(complaint: PostComplaint): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(this.baseUrl, complaint);
  }
}
