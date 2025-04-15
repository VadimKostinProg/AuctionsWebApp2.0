import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PaymentDeliveryOptionsModel } from '../models/paymentDeliveryOptionsModel';
import { SetDeliveryOptionsModel } from '../models/setDeliveryOptionsModel';

@Injectable({
  providedIn: 'root'
})
export class AuctionsPaymentDeliveryOptionsService {

  baseUrl: string = `${environment.apiUrl}/api/v1/auctions`;

  checkoutUrl: string = `${environment.apiUrl}/api/v1/checkout-session`;

  constructor(private readonly httpClient: HttpClient) { }

  getPaymentDeliveryOptions(auctionId: string): Observable<PaymentDeliveryOptionsModel> {
    return this.httpClient.get<PaymentDeliveryOptionsModel>(`${this.baseUrl}/${auctionId}/payment-delivery-options`);
  }

  setPaymentOptions(auctionId: string, IBAN: string): Observable<any> {
    return this.httpClient.post(`${this.baseUrl}/payment-options`, { auctionId: auctionId, IBAN: IBAN });
  }

  setDeliveryOptions(deliveryOptions: SetDeliveryOptionsModel): Observable<any> {
    return this.httpClient.post(`${this.baseUrl}/delivery-options`, deliveryOptions);
  }

  confirmPaymentOptions(auctionId: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/payment-options?auctionId=${auctionId}`, null);
  }

  confirmDeliveryOptions(auctionId: string, waybill: string): Observable<any> {
    return this.httpClient.put(`${this.baseUrl}/delivery-options`, { auctionId: auctionId, waybill: waybill });
  }

  checkout(auctionId: string): Observable<any> {
    return this.httpClient.post(`${this.checkoutUrl}?auctionId=${auctionId}`, null);
  }
}
