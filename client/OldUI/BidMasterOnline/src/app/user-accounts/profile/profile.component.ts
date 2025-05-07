import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { UsersService } from 'src/app/services/users.service';
import { AuthService } from 'src/app/services/auth.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ChangePasswordModel } from 'src/app/models/changePasswordModel';
import { ProfileModel } from 'src/app/models/profileModel';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { AuctionsService } from 'src/app/services/auctions.service';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { AuctionParticipantEnum } from 'src/app/models/auctionParticipantEnum';
import { UsersDeepLinkingService } from 'src/app/services/users-deep-linking.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dataTable: DataTableComponent;

  user: ProfileModel;

  ownProfile: boolean;

  changePasswordForm: FormGroup;

  options: DataTableOptionsModel;

  currentTable: string;

  constructor(private readonly usersService: UsersService,
    private readonly usersDeepLinkingService: UsersDeepLinkingService,
    private readonly authService: AuthService,
    private readonly modalService: NgbModal,
    private readonly router: Router,
    private readonly toastrService: ToastrService,
    private readonly auctionsService: AuctionsService) {

  }

  async ngOnInit() {
    var userId = await this.usersDeepLinkingService.getQueryParam('userId');

    if (userId == null || userId == this.authService.user.userId) {
      const currentAuthOptions = this.authService.user;
      userId = currentAuthOptions.userId;
      this.ownProfile = true;
    }

    this.usersService.getUserProfileById(userId).subscribe(
      async (response) => {
        this.user = response;

        if (this.user.totalAuctions > 0) {
          this.currentTable = 'Users auctions';

          await this.usersDeepLinkingService.setQueryParams([
            { key: 'status', value: 'Finished' },
            { key: 'sortField', value: 'finishDateAndTime' },
            { key: 'auctionistId', value: this.user.id },
          ]);


          this.options = this.auctionsService.getUsersAuctionsDataTableOptions(this.currentTable);
        } else if (this.user.totalWins > 0) {
          this.currentTable = 'Auction victories';

          await this.usersDeepLinkingService.setQueryParams([
            { key: 'status', value: 'Finished' },
            { key: 'sortField', value: 'finishDateAndTime' },
            { key: 'winnerId', value: this.user.id },
          ]);

          this.options = this.auctionsService.getUsersAuctionsDataTableOptions(this.currentTable);
        }
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.refreshChangePasswordForm();
  }

  async updateQueryParamsFor(participant: AuctionParticipantEnum) {
    switch (participant) {
      case AuctionParticipantEnum.Auctionist:
        await this.usersDeepLinkingService.setAuctionist(this.user.id);
        break;
      case AuctionParticipantEnum.Auctioner:
        await this.usersDeepLinkingService.setWinner(this.user.id);
        break;
    }
  }

  getAuctionsDataTableApiUrl() {
    return this.auctionsService.getUsersAuctionsDataTableApiUrl();
  }

  refreshChangePasswordForm() {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl(null, [Validators.required]),
      newPassword: new FormControl(null, [Validators.required]),
      confirmedNewPassword: new FormControl(null, [Validators.required]),
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  async onTableChange(table: string) {
    this.currentTable = table;

    if (this.currentTable == 'Users auctions') {
      await this.updateQueryParamsFor(AuctionParticipantEnum.Auctionist);
    } else {
      await this.updateQueryParamsFor(AuctionParticipantEnum.Auctioner);
    }

    this.options = this.auctionsService.getUsersAuctionsDataTableOptions(this.currentTable);

    await this.dataTable.reloadDatatable();
  }

  onChangePasswordSubmit(modal: any) {
    if (!this.changePasswordForm.valid) {
      return;
    }

    const changePasswordModel = this.changePasswordForm.value as ChangePasswordModel;

    this.usersService.changePassword(changePasswordModel).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.refreshChangePasswordForm();

        modal.close();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  onDeleteAccountSubmit(modal: any) {
    modal.close();

    this.usersService.deleteOwnAccount().subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.authService.logOut();
        this.router.navigate(['/']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
