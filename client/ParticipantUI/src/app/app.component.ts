import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false
})
export class AppComponent {
  title = 'ParticipantUI';

  constructor(private authService: AuthService) { }

  login() {
    this.authService.login();
  }

  logout() {
    this.authService.logout();
  }
}
