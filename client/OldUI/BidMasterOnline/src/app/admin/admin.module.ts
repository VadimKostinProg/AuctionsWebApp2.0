import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from './categories/categories.component';
import { CommonSharedModule } from '../common-shared/common-shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StaffManagementComponent } from './staff-management/staff-management.component';
import { ToastrModule } from 'ngx-toastr';
import { AuctionCreationRequestsListComponent } from './auction-creation-requests-list/auction-creation-requests-list.component';
import { AuctionCreationRequestComponent } from './auction-creation-request/auction-creation-request.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    CategoriesComponent,
    StaffManagementComponent,
    AuctionCreationRequestsListComponent,
    AuctionCreationRequestComponent,
  ],
  imports: [
    CommonModule,
    CommonSharedModule,
    ReactiveFormsModule,
    NgbModule,
    RouterModule,
    ToastrModule.forRoot(),
  ],
  exports: [
    CategoriesComponent,
    StaffManagementComponent,
    AuctionCreationRequestsListComponent,
    AuctionCreationRequestComponent,
  ]
})
export class AdminModule { }
