import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ServiceResult } from "../models/shared/serviceResult";
import { AuctionType } from "../models/auctions/auctionType";

@Injectable({
  providedIn: 'root'
})
export class AuctionTypesService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-types`;

  constructor(private readonly httpClient: HttpClient) { }

  getAllAuctionTypes(): Observable<ServiceResult<AuctionType[]>> {
    return this.httpClient.get<ServiceResult<AuctionType[]>>(`${this.baseUrl}/all`);
  }
}
