import { Component } from "@angular/core";
import { AuthService } from "../../services/auth.service";

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  standalone: false
})
export class SignInComponent {
  constructor(private authService: AuthService) { }

  login() {
    this.authService.login();
  }
}
