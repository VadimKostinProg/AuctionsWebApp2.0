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
import { SetDeliveryOptionsModel } from 'src/app/models/setDeliveryOptionsModel';

@Component({
  selector: 'app-set-delivery-options',
  templateUrl: './set-delivery-options.component.html'
})
export class SetDeliveryOptionsComponent {
  deliveryForm: FormGroup;

  allowSetDeliveryOptions: boolean = false;

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

        if (this.auctionDetails.winnerId == user.userId) {
          this.allowSetDeliveryOptions = true;
        }
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.deliveryForm = new FormGroup({
      country: new FormControl(null, [Validators.required]),
      city: new FormControl(null, [Validators.required]),
      zipCode: new FormControl(null, [Validators.required]),
      guarantee: new FormControl(null, [Validators.requiredTrue])
    });
  }

  get country() {
    return this.deliveryForm.get('country');
  }

  get city() {
    return this.deliveryForm.get('city');
  }

  get zipCode() {
    return this.deliveryForm.get('zipCode');
  }

  get guarantee() {
    return this.deliveryForm.get('guarantee');
  }

  onSubmit() {
    if (!this.deliveryForm.valid) {
      return;
    }

    const formValue = this.deliveryForm.value;

    const delivaryOptions = {
      auctionId: this.auctionDetails.id,
      country: formValue.country,
      city: formValue.city,
      zipCode: formValue.zipCode
    } as SetDeliveryOptionsModel;

    this.paymentDeliveryService.setDeliveryOptions(delivaryOptions).subscribe(
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
