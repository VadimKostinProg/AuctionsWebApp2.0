import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home/home.component';
import { SearchComponent } from './search/search.component';
import { CommonSharedModule } from '../common-shared/common-shared.module';
import { AuctionsModule } from '../auctions/auctions.module';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    HomeComponent,
    SearchComponent,
    ConfirmEmailComponent,
  ],
  imports: [
    CommonModule,
    CommonSharedModule,
    AuctionsModule,
    FormsModule,
    ToastrModule.forRoot(),
  ],
  exports: [
    HomeComponent,
    SearchComponent,
    ConfirmEmailComponent,
  ]
})
export class GeneralModule { }
