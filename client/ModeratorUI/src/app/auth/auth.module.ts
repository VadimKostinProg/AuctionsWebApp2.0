import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CallbackComponent } from './callback/callback.component';
import { RouterModule, Routes } from '@angular/router';
import { OAuthModule } from 'angular-oauth2-oidc';

const routes: Routes = [
  { path: 'callback', component: CallbackComponent },
];

@NgModule({
  declarations: [
    CallbackComponent
  ],
  imports: [
    CommonModule,
    OAuthModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    CallbackComponent
  ]
})
export class AuthModule { }
