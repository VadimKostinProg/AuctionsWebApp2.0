import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'auction-categories',
    loadChildren: () => import('../auction-categories-management/auction-categories-management.module').then(m => m.AuctionCategoriesManagementModule)
  },
  {
    path: 'auctions',
    loadChildren: () => import('../auctions-management/auctions-management.module').then(m => m.AuctionsManagementModule)
  },
  {
    path: 'auction-requests',
    loadChildren: () => import('../auction-requests-management/auction-requests-management.module').then(m => m.AuctionRequestsManagementModule)
  },
  {
    path: 'users',
    loadChildren: () => import('../users-management/users-management.module').then(m => m.UsersManagementModule)
  },
  {
    path: 'complaints',
    loadChildren: () => import('../complaints-management/complaints-management.module').then(m => m.ComplaintsManagementModule)
  },
  {
    path: 'support-tickets',
    loadChildren: () => import('../support-tickets-management/support-tickets-management.module').then(m => m.SupportTicketsManagementModule)
  },
];

@NgModule({
  declarations: [
    HomeComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ],
  exports: [
    HomeComponent
  ]
})
export class HomeModule { }
