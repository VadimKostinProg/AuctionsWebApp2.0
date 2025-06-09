import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule, Routes } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { UserFeedbacksComponent } from "./user-feedbacks/user-feedbacks.component";
import { SharedModule } from "../shared/shared.module";
import { UserAuctionsHistoryComponent } from "./user-auctions-history/user-auctions-history.component";
import { UserBidsHistoryComponent } from "./user-bids-history/user-bids-history.component";
import { PaymentSetupComponent } from "./payment-setup/payment-setup.component";

const routes: Routes = [
  {
    path: 'auctions-history',
    component: UserAuctionsHistoryComponent
  },
  {
    path: 'bids-history',
    component: UserBidsHistoryComponent
  },
  {
    path: 'setup-payment',
    component: PaymentSetupComponent
  },
  {
    path: ':userId',
    component: UserProfileComponent
  }
];

@NgModule({
  declarations: [
    UserProfileComponent,
    UserFeedbacksComponent,
    UserAuctionsHistoryComponent,
    UserBidsHistoryComponent,
    PaymentSetupComponent
  ],
  imports: [
    CommonModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    RouterModule.forChild(routes),
    RouterModule,
    SharedModule
  ],
  exports: [
  ]
})
export class UserProfilesModule { }
