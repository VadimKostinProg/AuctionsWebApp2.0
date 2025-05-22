import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { Observable } from "rxjs";
import { AuctionCategory, AuctionType, AuctionFinishMethod } from "../models/auctions/Auction";

@Injectable({
  providedIn: 'root'
})
export class AuctionResourcesService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}`;

  constructor(private readonly httpClient: HttpClient) { }

  getAuctionCategories(): Observable<AuctionCategory[]> {
    return this.httpClient.get<AuctionCategory[]>(`${this.baseUrl}/auction-categories`);
  }

  getAuctionTypes(): Observable<AuctionCategory[]> {
    return this.httpClient.get<AuctionType[]>(`${this.baseUrl}/auction-types`);
  }

  getAuctionFinishMethods(): Observable<AuctionCategory[]> {
    return this.httpClient.get<AuctionFinishMethod[]>(`${this.baseUrl}/auction-finish-methods`);
  }
}
