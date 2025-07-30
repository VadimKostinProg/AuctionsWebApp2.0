import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../src/environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.identityServerUrl,
  clientId: 'ModeratorUI',
  redirectUri: window.location.origin + '/auth/callback',
  scope: 'openid profile moderatorScope',
  responseType: 'code',
  showDebugInformation: !environment.production,
  requireHttps: false,
  strictDiscoveryDocumentValidation: false
};
