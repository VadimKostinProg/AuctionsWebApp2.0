import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuctionBasic } from "../models/auctions/AuctionBasic";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class AuctionsService {

  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  getPopularAuctions(): Observable<AuctionBasic[]> {
    // todo: add query params

    return this.httpClient.get<AuctionBasic[]>(this.baseUrl);
  }
}
