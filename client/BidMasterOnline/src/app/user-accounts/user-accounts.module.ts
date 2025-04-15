import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInComponent } from './sign-in/sign-in.component';
import { CreateAccountComponent } from './create-account/create-account.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonSharedModule } from '../common-shared/common-shared.module';

@NgModule({
  declarations: [
    SignInComponent,
    CreateAccountComponent,
    ProfileComponent,
  ],
  imports: [
    CommonModule,
    CommonSharedModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    FontAwesomeModule,
  ],
  exports: [
    SignInComponent,
    CreateAccountComponent,
    ProfileComponent,
  ]
})
export class UserAccountsModule { }
