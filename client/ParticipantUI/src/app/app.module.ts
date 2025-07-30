import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { OAuthModule, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { RouterModule, RouterOutlet, Routes } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AppComponent } from './app.component';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { DatePipe } from '@angular/common';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
];

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    RouterOutlet,
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
      }
    }),
    RouterModule.forRoot(routes)
  ],
  providers: [
    AuthGuard,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
    provideToastr(),
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    DatePipe,
    { provide: OAuthStorage, useValue: localStorage },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
