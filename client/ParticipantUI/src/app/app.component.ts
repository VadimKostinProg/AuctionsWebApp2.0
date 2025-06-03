import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false
})
export class AppComponent {
  title = 'BidMasterOnline';

  currentRoute: string = '';

  constructor(private authService: AuthService) { }

  get isLoggedIn() {
    return this.authService.isLoggedIn;
  }

  get userId() {
    return this.authService.user.userId;
  }

  logout() {
    this.authService.logout();
  }
}
