import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SharedModule } from '../shared/shared.module';
import { AuctionsModule } from '../auctions/auctions.module';

const routes: Routes = [
  { path: '', component: HomeComponent },
];

@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    AuctionsModule,
    RouterModule,
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
