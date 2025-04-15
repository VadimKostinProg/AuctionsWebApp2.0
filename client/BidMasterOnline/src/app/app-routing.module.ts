import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './general/home/home.component';
import { SearchComponent } from './general/search/search.component';
import { SignInComponent } from './user-accounts/sign-in/sign-in.component';
import { CreateAccountComponent } from './user-accounts/create-account/create-account.component';
import { CategoriesComponent } from './admin/categories/categories.component';
import { CanActivateGuardService } from './services/can-activate-guard.service';
import { StaffManagementComponent } from './admin/staff-management/staff-management.component';
import { ConfirmEmailComponent } from './general/confirm-email/confirm-email.component';
import { ProfileComponent } from './user-accounts/profile/profile.component';
import { CustomersManagementComponent } from './technical-support/customers-management/customers-management.component';
import { CreateAuctionComponent } from './customer/create-auction/create-auction.component';
import { AuctionCreationRequestsListComponent } from './admin/auction-creation-requests-list/auction-creation-requests-list.component';
import { AuctionCreationRequestComponent } from './admin/auction-creation-request/auction-creation-request.component';
import { AuctionDetailsComponent } from './auctions/auction-details/auction-details.component';
import { ComplaintsComponent } from './technical-support/complaints/complaints.component';
import { ComplaintDetailsComponent } from './technical-support/complain-details/complaint-details.component';
import { TechnicalSupportRequestsListComponent } from './technical-support/technical-support-requests-list/technical-support-requests-list.component';
import { TechnicalSupportRequestComponent } from './technical-support/technical-support-request/technical-support-request.component';
import { TechnicalSupportComponent } from './customer/technical-support/technical-support.component';
import { BidsHistoryComponent } from './customer/bids-history/bids-history.component';
import { PaymentDeliveryOptionsComponent } from './technical-support/payment-delivery-options/payment-delivery-options.component';
import { SetPaymentOptionsComponent } from './customer/set-payment-options/set-payment-options.component';
import { SetDeliveryOptionsComponent } from './customer/set-delivery-options/set-delivery-options.component';
import { ApplyDeliveryComponent } from './customer/apply-delivery/apply-delivery.component';
import { ApplyPaymentComponent } from './customer/apply-payment/apply-payment.component';
import { PaymentSuccessComponent } from './customer/payment-success/payment-success.component';
import { PaymentCancelComponent } from './customer/payment-cancel/payment-cancel.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'search',
    component: SearchComponent
  },
  {
    path: 'auction-details',
    component: AuctionDetailsComponent
  },
  {
    path: 'sign-in',
    component: SignInComponent
  },
  {
    path: 'create-account',
    component: CreateAccountComponent
  },
  {
    path: 'confirm-email',
    component: ConfirmEmailComponent,
    canActivate: [CanActivateGuardService],
    data: {
      expectedRoles: [
        'Admin',
        'TechnicalSupportSpecialist',
        'Customer'
      ]
    }
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [CanActivateGuardService],
    data: {
      expectedRoles: [
        'Admin',
        'TechnicalSupportSpecialist',
        'Customer'
      ]
    }
  },
  {
    path: 'staff-management',
    component: StaffManagementComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'categories',
    component: CategoriesComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'customers-management',
    component: CustomersManagementComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'complaints',
    component: ComplaintsComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'complaint-details',
    component: ComplaintDetailsComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'technical-support-requests',
    component: TechnicalSupportRequestsListComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'technical-support-request',
    component: TechnicalSupportRequestComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'payment-delivery-options',
    component: PaymentDeliveryOptionsComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['TechnicalSupportSpecialist'] }
  },
  {
    path: 'create-auction',
    component: CreateAuctionComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'technical-support',
    component: TechnicalSupportComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'bids-history',
    component: BidsHistoryComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'set-payment-options',
    component: SetPaymentOptionsComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'set-delivery-options',
    component: SetDeliveryOptionsComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'apply-delivery',
    component: ApplyDeliveryComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'apply-payment',
    component: ApplyPaymentComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Customer'] }
  },
  {
    path: 'payment-success',
    component: PaymentSuccessComponent,
  },
  {
    path: 'payment-cancel',
    component: PaymentCancelComponent,
  },
  {
    path: 'auction-creation-requests-list',
    component: AuctionCreationRequestsListComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Admin'] }
  },
  {
    path: 'auction-creation-request',
    component: AuctionCreationRequestComponent,
    canActivate: [CanActivateGuardService],
    data: { expectedRoles: ['Admin'] }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
