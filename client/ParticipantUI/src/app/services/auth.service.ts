import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from '../../auth.config';
import { UserBasic } from '../models/users/userBasic';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private oauthService: OAuthService) {
    this.configure();
  }

  private configure() {
    this.oauthService.configure(authConfig);
    this.oauthService.loadDiscoveryDocument();
  }

  login() {
    this.oauthService.initLoginFlow();
  }

  logout() {
    this.oauthService.logOut();
  }

  get identityClaims() {
    return this.oauthService.getIdentityClaims();
  }

  get accessToken() {
    return this.oauthService.getAccessToken();
  }

  get isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  get userStatus(): string {
    return this.identityClaims['status'];
  }

  get user(): UserBasic {
    const claims = this.identityClaims;

    return {
      userId: claims['sub'],
      username: claims[''],
      email: claims['email']
    }
  }
}
