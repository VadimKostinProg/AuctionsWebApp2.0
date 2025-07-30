import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../src/environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.identityServerUrl,
  clientId: 'ParticipantUI',
  redirectUri: window.location.origin + '/auth/callback',
  scope: 'openid profile participantScope',
  responseType: 'code',
  showDebugInformation: !environment.production,
  requireHttps: false,
  strictDiscoveryDocumentValidation: false,
};
