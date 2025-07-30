import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { LoaderService } from './services/loader.service copy';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit {
  title = 'BidMasterOnline Moderator Tools';

  currentRoute: string = '';

  isLoading: boolean = false;

  constructor(private authService: AuthService,
    private readonly loaderService: LoaderService) { }

  ngOnInit(): void {
    this.loaderService.isLoading$.subscribe(value => this.isLoading = value);
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
