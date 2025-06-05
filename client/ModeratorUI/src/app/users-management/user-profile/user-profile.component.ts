import { Component, OnInit, TemplateRef } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ActivatedRoute } from "@angular/router";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { UserStatusEnum } from "../../models/users/userStatusEnum";
import { UserProfileInfo } from "../../models/users/userProfileInfo";
import { UsersService } from "../../services/users.service";
import { BlockUserModel } from "../../models/users/blockUserModel";

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  standalone: false
})
export class UserProfileComponent implements OnInit {

  userId: number | undefined;

  userProfileInfo: UserProfileInfo | undefined;

  blockUserForm!: FormGroup;

  UserStatusEnum = UserStatusEnum;

  showBlockingPeriodField: boolean = false;

  showUserFeedbacks: boolean = false;

  constructor(private readonly usersService: UsersService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const userId = params.get('userId');

      if (userId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.userId = parseInt(userId);

      this.reloadUserProfileInfo();

      this.reloadBlockUserForm();
    });
  }

  reloadUserProfileInfo() {
    this.showUserFeedbacks = false;

    this.usersService.getUserProfileDetails(this.userId!).subscribe({
      next: result => {
        this.userProfileInfo = result.data!;

        this.showUserFeedbacks = true;
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  reloadBlockUserForm() {
    this.blockUserForm = new FormGroup({
      blockingReason: new FormControl(null, [Validators.required]),
      setBlockingPeriod: new FormControl(false),
      blockingPeriodInDays: new FormControl(null)
    });
  }

  get blockingReason() {
    return this.blockUserForm!.get('blockUserForm');
  }

  get setBlockingPeriod() {
    return this.blockUserForm!.get('setBlockingPeriod');
  }

  get blockingPeriodInDays() {
    return this.blockUserForm!.get('blockingPeriodInDays');
  }

  onSetBlockingPeriodFlagChange() {
    this.showBlockingPeriodField = this.setBlockingPeriod?.value ?? false;
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  blockUserAccount(modal: any) {
    if (!this.blockUserForm.valid) {
      return;
    }

    modal.close();

    const value = this.blockUserForm.value;

    const blockUserRequest = {
      blockingReason: value.blockingReason,
      blockingPeriodInDays: value.setBlockingPeriod
        ? value.blockingPeriodInDays
        : null
    } as BlockUserModel;

    this.usersService.blockUser(this.userId!, blockUserRequest).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.reloadUserProfileInfo();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  unblockUserAccount(modal: any) {
    modal.close();

    this.usersService.unblockUser(this.userId!).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.reloadUserProfileInfo();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
