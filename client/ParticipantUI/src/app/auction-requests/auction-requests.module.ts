import { CommonModule } from "@angular/common";
import { SharedModule } from "../shared/shared.module";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { RouterModule, Routes } from "@angular/router";
import { NewAuctionRequestComponent } from "./new-auction-request/new-auction-request.component";
import { ToastrModule } from "ngx-toastr";
import { NgxSpinnerModule } from "ngx-spinner";
import { NgSelectModule } from "@ng-select/ng-select";
import { AuctionRequestsHistoryComponent } from "./auction-requests-history/auction-requests-history.component";
import { AuctionRequestDetailsComponent } from "./auction-request-details/auction-request-details.component";

const routes: Routes = [
  {
    path: '',
    component: AuctionRequestsHistoryComponent
  },
  {
    path: 'new',
    component: NewAuctionRequestComponent
  },
  {
    path: ':auctionRequestId',
    component: AuctionRequestDetailsComponent
  },
];

@NgModule({
  declarations: [
    NewAuctionRequestComponent,
    AuctionRequestsHistoryComponent,
    AuctionRequestDetailsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    NgbModule,
    NgSelectModule,
    ToastrModule.forRoot(),
    NgxSpinnerModule.forRoot(),
  ],
  exports: []
})
export class AuctionRequestsModule { }
