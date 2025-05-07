import { Component, OnInit } from '@angular/core';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { ToastrService } from 'ngx-toastr';
import { AuctionsPaymentDeliveryOptionsService } from 'src/app/services/auction-payment-delivery-options.service';
import { PaymentDeliveryOptionsModel } from 'src/app/models/paymentDeliveryOptionsModel';

@Component({
  selector: 'app-payment-delivery-options',
  templateUrl: './payment-delivery-options.component.html'
})
export class PaymentDeliveryOptionsComponent implements OnInit {

  paymentDeliveryOptions: PaymentDeliveryOptionsModel;

  constructor(private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService,
    private readonly toastrService: ToastrService,
    private readonly sellDeliveryOptionsService: AuctionsPaymentDeliveryOptionsService) {

  }

  async ngOnInit(): Promise<void> {
    var auctionId = this.auctionsDeepLinkingService.getAuctionId();

    if (auctionId == null) {
      this.toastrService.error('Invalid query params.', 'Error');

      return;
    }

    this.sellDeliveryOptionsService.getPaymentDeliveryOptions(auctionId).subscribe(
      (response) => {
        this.paymentDeliveryOptions = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
