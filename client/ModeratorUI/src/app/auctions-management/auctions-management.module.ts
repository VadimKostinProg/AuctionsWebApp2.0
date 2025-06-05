import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgxSpinnerModule } from "ngx-spinner";
import { ToastrModule } from "ngx-toastr";
import { AuctionsListComponent } from "./auctions-list/auctions-list.component";
import { AuctionDetailsComponent } from "./auction-details/auction-details.component";
import { AuctionCommentsComponent } from "./auction-comments/auction-comments.component";
import { AuctionFiltersComponent } from "./auction-filters/auction-filters.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

const routes: Routes = [
  { path: '', component: AuctionsListComponent },
  { path: ':auctionId', component: AuctionDetailsComponent },
];

@NgModule({
  declarations: [
    AuctionsListComponent,
    AuctionDetailsComponent,
    AuctionCommentsComponent,
    AuctionFiltersComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    ToastrModule.forRoot(),
    NgxSpinnerModule.forRoot(),
  ],
  exports: [
  ]
})
export class AuctionsManagementModule { }
