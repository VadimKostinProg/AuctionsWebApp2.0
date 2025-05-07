import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ScoresService {

  baseUrl: string = `${environment.apiUrl}/api/v1/auctions`;

  constructor(private readonly httpClient: HttpClient) { }

  setScoreForAuction(auctionId: string, score: number): Observable<any> {
    const model = {
      auctionId: auctionId,
      score: score
    };

    return this.httpClient.post(`${this.baseUrl}/scores`, model);
  }
}
