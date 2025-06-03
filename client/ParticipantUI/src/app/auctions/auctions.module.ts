import { CommonModule } from "@angular/common";
import { AuctionDetailsComponent } from "./auction-details/auction-details.component";
import { AuctionCardComponent } from "./auctions-card/auction-card.component";
import { CommentsComponent } from "./comments/comments.component";
import { SharedModule } from "../shared/shared.module";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { RouterModule, Routes } from "@angular/router";
import { ToastrModule } from "ngx-toastr";
import { AuctionFiltersComponent } from "./auction-filters/auction-filters.component";

const routes: Routes = [
  {
    path: ':auctionId/details',
    component: AuctionDetailsComponent
  }
];

@NgModule({
  declarations: [
    AuctionDetailsComponent,
    AuctionCardComponent,
    CommentsComponent,
    AuctionFiltersComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    NgbModule,
    RouterModule,
    ToastrModule.forRoot(),
  ],
  exports: [
    AuctionDetailsComponent,
    AuctionCardComponent,
    CommentsComponent,
    AuctionFiltersComponent
  ]
})
export class AuctionsModule { }
