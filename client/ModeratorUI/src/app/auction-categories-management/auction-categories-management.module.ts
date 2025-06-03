import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuctionCategoriesListComponent } from "./auction-categories-list/auction-categories-list.component";
import { SharedModule } from "../shared/shared.module";

const routes: Routes = [
  { path: '', component: AuctionCategoriesListComponent },
];

@NgModule({
  declarations: [
    AuctionCategoriesListComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule
  ],
  exports: [
  ]
})
export class AuctionCategoriesManagementModule { }
