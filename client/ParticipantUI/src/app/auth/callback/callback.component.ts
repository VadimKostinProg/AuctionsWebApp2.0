import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';

@Component({
  selector: 'app-callback',
  template: '<p>Authorization...</p>',
  standalone: false
})
export class CallbackComponent implements OnInit {
  constructor(private oauthService: OAuthService, private router: Router) { }

  async ngOnInit() {
    await this.oauthService.tryLoginCodeFlow();

    this.router.navigate(['/'])
  }
}
