import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SharedModule } from '../shared/shared.module';
import { AuctionsModule } from '../auctions/auctions.module';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'auctions',
    loadChildren: () => import('../auctions/auctions.module').then(m => m.AuctionsModule)
  },
  {
    path: 'auction-requests',
    loadChildren: () => import('../auction-requests/auction-requests.module').then(m => m.AuctionRequestsModule)
  },
];

@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    RouterModule,
    AuctionsModule
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
