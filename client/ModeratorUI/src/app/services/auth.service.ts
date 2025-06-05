import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from '../../auth.config';
import { UserStatusEnum } from '../models/users/userStatusEnum';
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

  get userStatus(): UserStatusEnum {
    const statusStr: string = this.identityClaims['user_status'];

    return UserStatusEnum[statusStr as keyof typeof UserStatusEnum];
  }

  get user(): UserBasic {
    const claims = this.identityClaims;

    return {
      userId: parseInt(claims['sub']),
      username: claims['preferred_username'],
      email: claims['email']
    }
  }
}
