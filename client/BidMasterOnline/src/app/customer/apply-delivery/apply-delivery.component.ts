import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuctionDetailsModel } from 'src/app/models/auctionDetailsModel';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { AuctionsService } from 'src/app/services/auctions.service';
import { AuctionsPaymentDeliveryOptionsService } from 'src/app/services/auction-payment-delivery-options.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { PaymentDeliveryOptionsModel } from 'src/app/models/paymentDeliveryOptionsModel';

@Component({
  selector: 'app-apply-delivery',
  templateUrl: './apply-delivery.component.html'
})
export class ApplyDeliveryComponent {

  confirmDeliveryForm: FormGroup;

  allowConfirmDelivery: boolean = false;

  auctionDetails: AuctionDetailsModel;

  paymentDeliveryOptions: PaymentDeliveryOptionsModel;

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
          this.allowConfirmDelivery = true;
        }
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.confirmDeliveryForm = new FormGroup({
      waybill: new FormControl(null, [Validators.required]),
    });
  }

  validatePaymentDelivaryOptions() {
    this.paymentDeliveryService.getPaymentDeliveryOptions(this.auctionDetails.id).subscribe(
      (response) => {
        this.paymentDeliveryOptions = response;

        if (this.paymentDeliveryOptions.areDeliveryOptionsSet && this.paymentDeliveryOptions.arePaymentOptionsSet) {
          this.allowConfirmDelivery = true;
        }
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    )
  }

  get waybill() {
    return this.confirmDeliveryForm.get('waybill');
  }

  onSubmit() {
    if (!this.confirmDeliveryForm.valid) {
      return;
    }

    var waybill = this.confirmDeliveryForm.value.waybill;

    this.paymentDeliveryService.confirmDeliveryOptions(this.auctionDetails.id, waybill).subscribe(
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
