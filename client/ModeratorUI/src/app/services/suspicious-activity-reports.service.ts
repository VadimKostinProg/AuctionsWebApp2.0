import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceResult } from "../models/shared/serviceResult";
import { AuctionAnalysis, SuspiciousActivityReport } from "../models/suspicious-activity-reports/suspicious-activity-reports";
import { SuspiciousActivityCheckPeriodEnum } from "../models/suspicious-activity-reports/suspicious-activity-check-period";

@Injectable({
  providedIn: 'root'
})
export class SuspiciousActivityReportsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/suspicious-activity-reports`;

  constructor(private readonly httpClient: HttpClient) { }

  getSuspiciousActivityReport(period: SuspiciousActivityCheckPeriodEnum): Observable<ServiceResult<SuspiciousActivityReport>> {
    const params = new HttpParams()
      .set('period', period);

    return this.httpClient.get<ServiceResult<SuspiciousActivityReport>>(this.baseUrl, { params });
  }

  getAuctionAnalysis(auctionAnalysisId: string): Observable<ServiceResult<AuctionAnalysis>> {
    return this.httpClient.get<ServiceResult<AuctionAnalysis>>(`${this.baseUrl}/auction-analyses/${auctionAnalysisId}`);
  }
}
