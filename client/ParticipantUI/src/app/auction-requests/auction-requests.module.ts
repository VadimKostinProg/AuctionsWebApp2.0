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

const routes: Routes = [
  {
    path: 'new',
    component: NewAuctionRequestComponent
  }
];

@NgModule({
  declarations: [
    NewAuctionRequestComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes),
    NgbModule,
    NgSelectModule,
    RouterModule,
    ToastrModule.forRoot(),
    NgxSpinnerModule.forRoot(),
  ],
  exports: [
    NewAuctionRequestComponent
  ]
})
export class AuctionRequestsModule { }
