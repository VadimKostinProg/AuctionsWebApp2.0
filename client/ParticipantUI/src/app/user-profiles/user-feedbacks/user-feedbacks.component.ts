import { Component, Input, TemplateRef } from "@angular/core";
import { UserBasic } from "../../models/users/userBasic";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth.service";
import { PaginationModel } from "../../models/shared/paginationModel";
import { AuctionComment } from "../../models/auctions/AuctionComment";
import { UserFeedback } from "../../models/user-profiles/userFeedback";
import { UserFeedbacksService } from "../../services/user-feedbacks.service";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComplaintsService } from "../../services/complaints.service";
import { UserFeedbacksQueryParamsService } from "../../services/user-feedbacks-query-params.service";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";
import { PostComplaint } from "../../models/complaints/postComplaint";
import { PostUserFeedback } from "../../models/user-profiles/postUserFeedback";
import { UserStatusEnum } from "../../models/user-profiles/userStatusEnum";

@Component({
  selector: 'app-user-feedbacks',
  templateUrl: './user-feedbacks.component.html',
  standalone: false
})
export class UserFeedbacksComponent {
  @Input() userId!: number;

  feedbackForm!: FormGroup;

  complaintForm!: FormGroup;

  feedbacks: UserFeedback[] = [];

  choosenFeedback?: UserFeedback | null;

  pagination: PaginationModel | undefined;

  UserStatusEnum = UserStatusEnum;

  constructor(private readonly authService: AuthService,
    private readonly userFeedbacksService: UserFeedbacksService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService,
    private readonly queryParamsService: UserFeedbacksQueryParamsService) {

  }

  get currentUserStatus() {
    return this.authService.userStatus;
  }

  get currentUserId() {
    return this.authService.user.userId;
  }

  async ngOnInit(): Promise<void> {
    this.pagination = await this.queryParamsService.getPaginationParams();

    if (!this.pagination.pageNumber || !this.pagination.pageSize) {
      this.pagination = {
        pageNumber: 1,
        pageSize: 15
      }

      await this.queryParamsService.setPaginationParams(this.pagination);
    }

    this.reloadUserFeedbacks();

    this.reloadFeedbackForm();

    this.reloadComplaintForm();
  }

  reloadUserFeedbacks() {
    this.userFeedbacksService.getUserFeedbacks(this.userId, this.pagination!.pageNumber!, this.pagination!.pageSize!).subscribe({
      next: (response) => {
        if (response.isSuccessfull)
          this.feedbacks = response.data!.items;
        else
          this.toastrService.error(response.errors[0], 'Error');
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  onScoreUpdated(newScore: number) {
    if (this.feedbackForm) {
      this.feedbackForm.patchValue({ score: newScore });
    }
  }

  reloadFeedbackForm() {
    this.feedbackForm = new FormGroup({
      score: new FormControl(null, [Validators.required]),
      comment: new FormControl(null),
    });
  }

  reloadComplaintForm() {
    this.complaintForm = new FormGroup({
      complaintText: new FormControl(null, [Validators.required])
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  openActionModal(content: TemplateRef<any>, feedback: UserFeedback) {
    this.choosenFeedback = feedback;

    this.open(content);
  }

  get comment() {
    return this.feedbackForm.get('comment');
  }

  get complaintText() {
    return this.complaintForm.get('complaintText');
  }

  leaveFeedback(modal: any) {
    if (!this.feedbackForm.valid) {
      return;
    }

    modal.close();

    const commentText = this.feedbackForm.value.comment;
    const score = this.feedbackForm.value.score;

    this.reloadFeedbackForm();

    const userFeedback = {
      score: score,
      toUserId: this.userId,
      comment: commentText
    } as PostUserFeedback;

    this.userFeedbacksService.postUserFeedback(userFeedback).subscribe({
      next: (response) => {
        if (response.isSuccessfull) {
          this.toastrService.success(response.message!, 'Success');

          this.reloadUserFeedbacks();
        }
        else {
          this.toastrService.error(response.errors[0], 'Error');
        }
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  deleteFeedback(modal: any) {
    modal.close();

    this.userFeedbacksService.deleteUserFeedback(this.choosenFeedback!.id).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadUserFeedbacks();
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  complainOnFeedback(modal: any) {
    if (!this.complaintForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintForm.value.complaintText;

    this.reloadFeedbackForm();

    const complaint = {
      accusedUserId: this.choosenFeedback!.fromUserId,
      accusedUserFeedbackId: this.choosenFeedback!.id,
      complaintText: complaintText,
      type: ComplaintTypeEnum.ComplaintOnUserFeedback
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
