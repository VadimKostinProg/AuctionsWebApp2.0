import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuctionCategoriesComponent } from "./auction-categories/auction-categories.component";
import { SharedModule } from "../shared/shared.module";

const routes: Routes = [
  { path: '', component: AuctionCategoriesComponent },
];

@NgModule({
  declarations: [
    AuctionCategoriesComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule
  ],
  exports: [
  ]
})
export class AuctionCategoriesModule { }
