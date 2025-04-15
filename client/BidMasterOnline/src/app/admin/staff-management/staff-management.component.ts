import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersService } from 'src/app/services/users.service';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { CreateUserModel } from 'src/app/models/createUserModel';
import { UserRoleEnum } from 'src/app/models/userRoleEnum';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-staff-management',
  templateUrl: './staff-management.component.html',
  styleUrl: './staff-management.component.scss'
})
export class StaffManagementComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dateTable: DataTableComponent;

  options: DataTableOptionsModel;

  placeholder: string = 'Search user...';

  constructor(private readonly usersService: UsersService,
    private readonly toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.options = this.usersService.getStaffDataTableOptions();
  }

  async onSearchClicked() {
    await this.dateTable.reloadDatatable();
  }

  getStaffUsersApiUrl() {
    return this.usersService.getStaffDataTableApiUrl();
  }

  onUserCreate() {
    if (!this.options.createFormOptions.form.valid) {
      return;
    }

    var user = this.options.createFormOptions.form.value;

    var createUser = {
      username: user.username,
      fullName: `${user.surname} ${user.name}`,
      email: user.email,
      dateOfBirth: user.dateOfBirth,
      password: user.password,
      confirmPassword: user.confirmPassword
    } as CreateUserModel;

    var observable: Observable<string>;

    switch (user.role) {
      case UserRoleEnum[UserRoleEnum.Admin]:
        this.usersService.createAdmin(createUser).subscribe(
          async (response) => {
            this.toastrService.success(response.message, 'Success');

            await this.dateTable.reloadDatatable();
          },
          (error) => {
            this.toastrService.success(error.error, 'Success');
          }
        );
        break;
      case UserRoleEnum[UserRoleEnum.TechnicalSupportSpecialist]:
        this.usersService.createTechnicalSupportSpecialist(createUser).subscribe(
          async (response) => {
            this.toastrService.success(response.message, 'Success');

            await this.dateTable.reloadDatatable();
          },
          (error) => {
            this.toastrService.success(error.error, 'Success');
          }
        );
        break;
    }

    this.options = this.usersService.getStaffDataTableOptions();
  }

  onUserDelete(userId: string) {
    this.usersService.deleteUser(userId).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        await this.dateTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.success(error.error, 'Success');
      }
    );
  }
}
