import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import { LoaderService } from './services/loader.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit {
  title = 'BidMasterOnline';

  currentRoute: string = '';

  isLoading = false;

  constructor(private readonly authService: AuthService,
    private readonly loaderService: LoaderService) { }

  ngOnInit(): void {
    this.loaderService.isLoading$.subscribe(loading => {
      this.isLoading = loading;
    });
  }

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
