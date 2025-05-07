import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.scss'
})
export class SignInComponent implements OnInit {

  signInForm: FormGroup;

  error: string | undefined;

  constructor(private readonly authService: AuthService,
    private readonly router: Router) { }

  ngOnInit(): void {
    this.signInForm = new FormGroup({
      username: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
    });
  }

  get username() {
    return this.signInForm.get('username');
  }

  get password() {
    return this.signInForm.get('password');
  }

  onSubmit() {
    if (!this.signInForm.valid) {
      return;
    }

    this.error = undefined;

    const signInModel = this.signInForm.value;

    this.authService.signIn(signInModel)
      .subscribe(
        (response) => {
          console.log('Authenticated successfully.');

          this.router.navigate(['/']);
        },
        (error) => {
          console.error('Authentication failed:', error);
          this.error = error.error;
        }
      );
  }
}
