import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { UserAccountsModule } from './user-accounts/user-accounts.module';
import { CommonSharedModule } from './common-shared/common-shared.module';
import { AuctionsModule } from './auctions/auctions.module';
import { GeneralModule } from './general/general.module';
import { AdminModule } from './admin/admin.module';
import { JwtInterceptorService } from './services/jwt-interceptor.service';
import { JwtModule } from '@auth0/angular-jwt';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TechnicalSupportModule } from './technical-support/technical-support.module';
import { CustomerModule } from './customer/customer.module';
import { AuctionsPaymentDeliveryOptionsService } from './services/auction-payment-delivery-options.service';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    GeneralModule,
    UserAccountsModule,
    CommonSharedModule,
    AuctionsModule,
    AdminModule,
    TechnicalSupportModule,
    CustomerModule,
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FontAwesomeModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => {
          if (localStorage.getItem('authenticatedUser') != null) {
            return JSON.parse(localStorage.getItem('authenticatedUser') as string).token;
          }

          return null;
        }
      }
    }),
    NgbModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptorService,
      multi: true
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }