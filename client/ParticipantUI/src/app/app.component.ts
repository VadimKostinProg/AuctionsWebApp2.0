import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false
})
export class AppComponent implements OnInit {
  title = 'ParticipantUI';

  constructor(private authService: AuthService,
    private router: Router) { }

  ngOnInit() {
    if (this.authService.isLoggedIn) {
      return;
    }

    const urlParams = new URLSearchParams(window.location.search);
    const hasCode = urlParams.has('code');

    if (hasCode) {
      return;
    }
    else {
      this.authService.login();
    }
  }
}
