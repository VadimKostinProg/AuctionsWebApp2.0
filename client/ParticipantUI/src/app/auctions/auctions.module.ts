import { CommonModule } from "@angular/common";
import { AuctionDetailsComponent } from "./auction-details/auction-details.component";
import { AuctionCardComponent } from "./auctions-card/auction-card.component";
import { CommentsComponent } from "./comments/comments.component";
import { SharedModule } from "../shared/shared.module";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { RouterModule } from "@angular/router";

@NgModule({
  declarations: [
    AuctionDetailsComponent,
    AuctionCardComponent,
    CommentsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    RouterModule
  ],
  exports: [
    AuctionDetailsComponent,
    AuctionCardComponent,
    CommentsComponent
  ]
})
export class AuctionsModule { }
