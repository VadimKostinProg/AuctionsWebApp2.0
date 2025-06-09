import { Component, OnInit, TemplateRef } from "@angular/core";
import { UserProfileService } from "../../services/user-profiles.service";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ActivatedRoute } from "@angular/router";
import { ComplaintsService } from "../../services/complaints.service";
import { UserProfileInfo } from "../../models/user-profiles/userProfileInfo";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth.service";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";
import { PostComplaint } from "../../models/complaints/postComplaint";
import { ResetPasswordModel } from "../../models/user-profiles/resetPasswordModel";
import { UserStatusEnum } from "../../models/users/userStatusEnum";

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  standalone: false
})
export class UserProfileComponent implements OnInit {

  userId: number | undefined;

  userProfileInfo: UserProfileInfo | undefined;

  complaintOnUserForm!: FormGroup;

  changePasswordForm!: FormGroup;

  isOwnProfile: boolean = false;

  UserStatusEnum = UserStatusEnum;

  constructor(private readonly userProfileService: UserProfileService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService,
    private readonly route: ActivatedRoute,
    private readonly authService: AuthService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const userId = params.get('userId');

      if (userId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.userId = parseInt(userId);

      this.isOwnProfile = this.userId == this.authService.user.userId;

      this.reloadUserProfileInfo();

      this.reloadChangePasswordForm();

      this.reloadComplaintForm();
    });
  }

  reloadUserProfileInfo() {
    this.userProfileService.getUserProfile(this.userId!).subscribe({
      next: result => {
        this.userProfileInfo = result.data!;
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  reloadComplaintForm() {
    this.complaintOnUserForm = new FormGroup({
      complaintText: new FormControl(null, [Validators.required])
    });
  }

  reloadChangePasswordForm() {
    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl(null, [Validators.required]),
      newPassword: new FormControl(null, [Validators.required]),
      repeatNewPassword: new FormControl(null, [Validators.required]),
    });
  }

  resetPassword(modal: any) {
    if (!this.changePasswordForm.valid) {
      return;
    }

    if (this.changePasswordForm.value.newPassword !== this.changePasswordForm.value.repeatNewPassword) {
      this.toastrService.error('Confimed password is not the same as new password.', 'Error');

      this.reloadChangePasswordForm();

      return;
    }

    const resetPasswordModel = this.changePasswordForm.value as ResetPasswordModel;

    this.userProfileService.resetPassword(resetPasswordModel).subscribe(
      (response) => {
        if (response.isSuccessfull) {
          this.toastrService.success(response.message!, 'Success');

          this.reloadChangePasswordForm();

          modal.close();
        }
        else
          this.toastrService.error(response.errors[0], 'Error');
      }
    );
  }

  deleteProfile(modal: any) {
    this.userProfileService.deleteProfile().subscribe((response) => {
      if (response.isSuccessfull) {
        modal.close();

        this.authService.logout();
      }
      else
        this.toastrService.error(response.errors[0], 'Error');
    })
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  complainOnUser(modal: any) {
    if (!this.complaintOnUserForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintOnUserForm.value.complaintText;

    this.reloadComplaintForm();

    const complaint = {
      accusedUserId: this.userId,
      type: ComplaintTypeEnum.ComplaintOnUserBehaviour,
      complaintText: complaintText
    } as PostComplaint;

    this.complaintsService.postComplaint(complaint).subscribe({
      next: (response) => {
        if (response.isSuccessfull)
          this.toastrService.success(response.message!, 'Success');
        else
          this.toastrService.error(response.errors[0], 'Error');
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }
}
