import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ToastrModule } from "ngx-toastr";
import { UserFiltersComponent } from "./user-filters/user-filters.component";
import { UsersListComponent } from "./users-list/users-list.component";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { UserFeedbacksComponent } from "./user-feedbacks/user-feedbacks.component";
import { UserAuctionsHistoryComponent } from "./user-auctions-history/user-auctions-history.component";
import { UserBidsHistoryComponent } from "./user-bids-history/user-bids-history.component";

const routes: Routes = [
  { path: '', component: UsersListComponent },
  { path: ':userId', component: UserProfileComponent },
  { path: ':userId/auctions-history', component: UserAuctionsHistoryComponent },
  { path: ':userId/bids-history', component: UserBidsHistoryComponent },
];

@NgModule({
  declarations: [
    UsersListComponent,
    UserProfileComponent,
    UserFeedbacksComponent,
    UserFiltersComponent,
    UserAuctionsHistoryComponent,
    UserBidsHistoryComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    ToastrModule.forRoot(),
  ],
  exports: [
  ]
})
export class UsersManagementModule { }
