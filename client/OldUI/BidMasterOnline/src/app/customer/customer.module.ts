import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule } from '@angular/router';
import { CreateAuctionComponent } from './create-auction/create-auction.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TechnicalSupportComponent } from './technical-support/technical-support.component';
import { CommonSharedModule } from '../common-shared/common-shared.module';
import { BidsHistoryComponent } from './bids-history/bids-history.component';
import { SetPaymentOptionsComponent } from './set-payment-options/set-payment-options.component';
import { SetDeliveryOptionsComponent } from './set-delivery-options/set-delivery-options.component';
import { ApplyPaymentComponent } from './apply-payment/apply-payment.component';
import { ApplyDeliveryComponent } from './apply-delivery/apply-delivery.component';
import { NgxStripeModule } from 'ngx-stripe';
import { environment } from 'src/environments/environment';
import { PaymentSuccessComponent } from './payment-success/payment-success.component';
import { PaymentCancelComponent } from './payment-cancel/payment-cancel.component';

@NgModule({
  declarations: [
    CreateAuctionComponent,
    TechnicalSupportComponent,
    BidsHistoryComponent,
    SetPaymentOptionsComponent,
    SetDeliveryOptionsComponent,
    ApplyPaymentComponent,
    ApplyDeliveryComponent,
    PaymentSuccessComponent,
    PaymentCancelComponent
  ],
  imports: [
    CommonModule,
    CommonSharedModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    ToastrModule.forRoot(),
    NgxSpinnerModule.forRoot(),
    NgxStripeModule.forRoot(environment.stripe.publicKey)
  ],
  exports: [
    CreateAuctionComponent,
    TechnicalSupportComponent,
    BidsHistoryComponent,
    SetPaymentOptionsComponent,
    SetDeliveryOptionsComponent,
    ApplyPaymentComponent,
    ApplyDeliveryComponent,
    PaymentSuccessComponent,
    PaymentCancelComponent
  ]
})
export class CustomerModule { }
