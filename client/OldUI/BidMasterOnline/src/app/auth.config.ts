import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.identityServerUrl,
  clientId: 'ParticipantUI',
  redirectUri: window.location.origin + '/callback',
  scope: 'openid profile participantScope',
  responseType: 'code',
  showDebugInformation: !environment.production,
};
