import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { AuctionRequestsListComponent } from "./auction-requests-list/auction-requests-list.component";
import { AuctionRequestDetailsComponent } from "./auction-request-details/auction-request-details.component";
import { AuctionRequestsFiltersComponent } from "./auction-requests-filters/auction-requests-filters.component";
import { ReactiveFormsModule } from "@angular/forms";
import { NgxSpinnerModule } from "ngx-spinner";
import { ToastrModule } from "ngx-toastr";

const routes: Routes = [
  { path: '', component: AuctionRequestsListComponent },
  { path: ':auctionRequestId', component: AuctionRequestDetailsComponent },
];

@NgModule({
  declarations: [
    AuctionRequestsListComponent,
    AuctionRequestDetailsComponent,
    AuctionRequestsFiltersComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    ToastrModule.forRoot(),
    NgxSpinnerModule.forRoot(),
  ],
  exports: [
  ]
})
export class AuctionRequestsManagementModule { }
