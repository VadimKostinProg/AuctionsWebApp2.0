import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuctionsPaymentDeliveryOptionsService } from 'src/app/services/auction-payment-delivery-options.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';

@Component({
  selector: 'app-payment-success',
  templateUrl: './payment-success.component.html'
})
export class PaymentSuccessComponent implements OnInit {

  constructor(private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService,
    private readonly router: Router,
    private readonly toastrService: ToastrService,
    private readonly paymentDeliveryOptionsService: AuctionsPaymentDeliveryOptionsService) {

  }

  async ngOnInit() {
    var auctionId = this.auctionsDeepLinkingService.getAuctionId();

    var queryToken = this.auctionsDeepLinkingService.getQueryParam('token');

    var actualToken = localStorage['paymentToken'];

    console.log(auctionId);
    console.log(queryToken);
    console.log(actualToken);

    if (auctionId == null || queryToken == null || actualToken == null || queryToken != actualToken) {
      this.toastrService.error('Invalid params.', 'Error');
    } else {
      this.paymentDeliveryOptionsService.confirmPaymentOptions(auctionId).subscribe(
        (response) => {
          this.toastrService.success(response.message, 'Success');
        },
        (error) => {
          this.toastrService.error(error.error, 'Error');
        }
      )
    }

    await this.router.navigate(['/']);
  }
}
