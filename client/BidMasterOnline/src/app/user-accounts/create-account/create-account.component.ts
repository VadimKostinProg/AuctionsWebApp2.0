import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../../services/users.service';
import { CreateUserModel } from '../../models/createUserModel';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html'
})
export class CreateAccountComponent implements OnInit {
  createAccountForm: FormGroup;

  image: File;

  error: string | undefined;

  showAllErrors: boolean = false;

  constructor(private readonly usersService: UsersService,
    private readonly router: Router,
    private readonly toastrService: ToastrService) { }

  ngOnInit(): void {
    this.createAccountForm = new FormGroup({
      username: new FormControl(null, [Validators.required]),
      name: new FormControl(null, [Validators.required]),
      surname: new FormControl(null, [Validators.required]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      dateOfBirth: new FormControl(null, [Validators.required]),
      password: new FormControl(null, [Validators.required]),
      confirmPassword: new FormControl(null, [Validators.required]),
    });
  }

  get username() {
    return this.createAccountForm.get('username');
  }

  get name() {
    return this.createAccountForm.get('name');
  }

  get surname() {
    return this.createAccountForm.get('surname');
  }

  get email() {
    return this.createAccountForm.get('email');
  }

  get dateOfBirth() {
    return this.createAccountForm.get('dateOfBirth');
  }

  get password() {
    return this.createAccountForm.get('password');
  }

  get confirmPassword() {
    return this.createAccountForm.get('confirmPassword');
  }

  onImageChange(file: any) {
    this.image = file.target.files[0];
  }

  onSubmit() {
    if (!this.createAccountForm.valid) {
      this.showAllErrors = true;

      return;
    }

    this.showAllErrors = false;

    this.error = undefined;

    const formModel = this.createAccountForm.value;

    const user = {
      image: this.image,
      username: formModel.username,
      fullName: `${formModel.surname} ${formModel.name}`,
      email: formModel.email,
      dateOfBirth: formModel.dateOfBirth,
      password: formModel.password,
      confirmPassword: formModel.confirmPassword
    } as CreateUserModel;

    this.usersService.createCustomer(user)
      .subscribe(
        (response) => {
          this.toastrService.success(response.message);

          this.router.navigate(['/sign-in']);
        },
        (error) => {
          this.error = error.error;
        }
      );
  }
}
