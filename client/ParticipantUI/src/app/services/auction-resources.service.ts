import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { AuctionCategory, AuctionType, AuctionFinishMethod } from "../models/auctions/Auction";
import { ServiceResult } from "../models/shared/serviceResult";

@Injectable({
  providedIn: 'root'
})
export class AuctionResourcesService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionCategories(): Observable<ServiceResult<AuctionCategory[]>> {
    return this.httpClient.get<ServiceResult<AuctionCategory[]>>(`${this.baseUrl}/auction-categories`);
  }

  getAuctionTypes(): Observable<ServiceResult<AuctionType[]>> {
    return this.httpClient.get<ServiceResult<AuctionType[]>>(`${this.baseUrl}/auction-types`);
  }

  getAuctionFinishMethods(): Observable<ServiceResult<AuctionFinishMethod[]>> {
    return this.httpClient.get<ServiceResult<AuctionFinishMethod[]>>(`${this.baseUrl}/auction-finish-methods`);
  }
}
