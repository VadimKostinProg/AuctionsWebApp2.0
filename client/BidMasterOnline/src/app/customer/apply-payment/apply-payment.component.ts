import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuctionDetailsModel } from 'src/app/models/auctionDetailsModel';
import { AuctionsPaymentDeliveryOptionsService } from 'src/app/services/auction-payment-delivery-options.service';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { AuctionsService } from 'src/app/services/auctions.service';
import { AuthService } from 'src/app/services/auth.service';
import { environment } from 'src/environments/environment';

declare var Stripe: any;

@Component({
  selector: 'app-apply-payment',
  templateUrl: './apply-payment.component.html'
})
export class ApplyPaymentComponent implements OnInit {

  auctionDetails: AuctionDetailsModel;

  constructor(private readonly toastrService: ToastrService,
    private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService,
    private readonly paymentDeliveryOptionsService: AuctionsPaymentDeliveryOptionsService,
    private readonly auctionsService: AuctionsService,
    private readonly router: Router,
    private readonly authService: AuthService) {

  }

  ngOnInit(): void {
    const auctionId = this.auctionsDeepLinkingService.getAuctionId();

    if (auctionId == null) {
      this.toastrService.error('Invalid query params.', 'Error');

      return;
    }

    this.auctionsService.getAuctionDetailsById(auctionId).subscribe(
      (response) => {
        this.auctionDetails = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    )
  }

  validatePaymentDeliveryOptions(auctionId: string) {
    this.paymentDeliveryOptionsService.getPaymentDeliveryOptions(auctionId).subscribe(
      (response) => {
        if (response.winnerId == null || response.winnerId != this.authService.user.userId) {
          this.toastrService.error('You cannot apply payment for this auction.', 'Error');
        } else if (response.isPaymentConfirmed) {
          this.toastrService.error('PaymentAlready confirmed.', 'Error');
        } else {
          return;
        }

        this.router.navigate(['/']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    )
  }

  checkout() {
    this.paymentDeliveryOptionsService.checkout(this.auctionDetails.id).subscribe(
      (response) => {
        localStorage['paymentToken'] = response.token;

        const stripe = Stripe(environment.stripe.publicKey);
        stripe.redirectToCheckout({
          sessionId: response.sessionId
        });
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
