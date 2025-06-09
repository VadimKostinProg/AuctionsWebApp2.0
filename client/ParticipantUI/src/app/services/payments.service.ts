import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class PaymentsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/payments`;

  constructor(private readonly httpClient: HttpClient) { }

  createSetupIntent(): Observable<ServiceResult<string>> {
    return this.httpClient.post<ServiceResult<string>>(`${this.baseUrl}/setup-intent`, null);
  }

  confirmIntent(setupIntentId: string): Observable<ServiceMessage> {
    const body = {
      setupIntentId: setupIntentId
    };

    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}/setup-intent-confirm`, body);
  }

  chargeAuctionWin(auctionId: number): Observable<ServiceMessage> {
    const body = {
      auctionId: auctionId
    }

    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}/charge-auction-win`, body);
  }
}
