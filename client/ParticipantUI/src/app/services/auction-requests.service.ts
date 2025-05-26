import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { PostAuctionRequest } from "../models/auction-requests/postAuctionRequest";
import { Observable } from "rxjs";
import { ServiceMessage } from "../models/shared/serviceResult";

@Injectable({
  providedIn: 'root'
})
export class AuctionRequestsService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-requests`;

  constructor(private readonly httpClient: HttpClient) { }

  postAuctionRequest(auctionRequest: PostAuctionRequest): Observable<ServiceMessage> {
    const form = new FormData();

    for (const image of auctionRequest.images)
      form.append('images', image);

    form.append('auctionCategoryId', auctionRequest.auctionCategoryId.toString());
    form.append('auctionTypeId', auctionRequest.auctionTypeId.toString());
    form.append('auctionFinishMethodId', auctionRequest.auctionFinishMethodId.toString());
    form.append('lotTitle', auctionRequest.lotTitle);
    form.append('lotDescription', auctionRequest.lotDescription);
    form.append('requestedAuctionTime', auctionRequest.requestedAuctionTime);
    form.append('startPrice', auctionRequest.startPrice.toString());
    form.append('bidAmountInterval', auctionRequest.bidAmountInterval.toString());

    if (auctionRequest.requestedStartTime) {
      form.append('requestedStartTime', auctionRequest.requestedStartTime.toUTCString());
    }

    if (auctionRequest.finishTimeInterval) {
      form.append('finishTimeInterval', auctionRequest.finishTimeInterval);
    }

    return this.httpClient.post<ServiceMessage>(this.baseUrl, form);
  }
}
