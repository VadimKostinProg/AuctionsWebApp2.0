import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { AuctionsService } from 'src/app/services/auctions.service';
import { AuctionsPaymentDeliveryOptionsService } from 'src/app/services/auction-payment-delivery-options.service';
import { ToastrService } from 'ngx-toastr';
import { AuctionDetailsModel } from 'src/app/models/auctionDetailsModel';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-set-payment-options',
  templateUrl: './set-payment-options.component.html'
})
export class SetPaymentOptionsComponent implements OnInit {

  paymentForm: FormGroup;

  allowSetPaymentOptions: boolean = false;

  auctionDetails: AuctionDetailsModel;

  constructor(private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService,
    private readonly auctionsService: AuctionsService,
    private readonly paymentDeliveryService: AuctionsPaymentDeliveryOptionsService,
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService,
    private readonly router: Router) {

  }

  ngOnInit(): void {
    var auctionId = this.auctionsDeepLinkingService.getAuctionId();

    if (auctionId == null) {
      this.toastrService.error('Invalid query params.', 'Error');
    }

    this.auctionsService.getAuctionDetailsById(auctionId).subscribe(
      (response) => {
        this.auctionDetails = response;

        var user = this.authService.user;

        if (this.auctionDetails.auctionistId == user.userId) {
          this.allowSetPaymentOptions = true;
        }
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.paymentForm = new FormGroup({
      iban: new FormControl(null, [Validators.required]),
      guarantee: new FormControl(null, [Validators.requiredTrue])
    });
  }

  get iban() {
    return this.paymentForm.get('iban');
  }

  get guarantee() {
    return this.paymentForm.get('guarantee');
  }

  onSubmit() {
    if (!this.paymentForm.valid) {
      return;
    }

    var IBAN = this.paymentForm.value.iban;

    this.paymentDeliveryService.setPaymentOptions(this.auctionDetails.id, IBAN).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.router.navigate(['/']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
