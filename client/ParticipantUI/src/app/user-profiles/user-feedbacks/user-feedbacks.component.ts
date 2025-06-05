import { Component, Input, TemplateRef } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../../services/auth.service";
import { UserFeedback } from "../../models/user-profiles/userFeedback";
import { UserFeedbacksService } from "../../services/user-feedbacks.service";
import { ToastrService } from "ngx-toastr";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComplaintsService } from "../../services/complaints.service";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";
import { PostComplaint } from "../../models/complaints/postComplaint";
import { PostUserFeedback } from "../../models/user-profiles/postUserFeedback";
import { UserStatusEnum } from "../../models/users/userStatusEnum";

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

  currentPage: number = 0;
  pageSize: number = 10;
  hasNext: boolean = false;

  UserStatusEnum = UserStatusEnum;

  constructor(private readonly authService: AuthService,
    private readonly userFeedbacksService: UserFeedbacksService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService) {

  }

  get currentUserStatus() {
    return this.authService.userStatus;
  }

  get currentUserId() {
    return this.authService.user.userId;
  }

  async ngOnInit(): Promise<void> {
    this.loadUserFeedbacks();

    this.reloadFeedbackForm();

    this.reloadComplaintForm();
  }

  reloadUserFeedbacks() {
    this.feedbacks = [];
    this.currentPage = 0;
    this.hasNext = false;

    this.loadUserFeedbacks();
  }

  loadUserFeedbacks() {
    this.userFeedbacksService.getUserFeedbacks(this.userId, ++this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.feedbacks = [...this.feedbacks, ...response.data!.items];
        this.currentPage = response.data!.pagination.currentPage;
        this.hasNext = response.data!.pagination.hasNext;
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
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
        this.toastrService.success(response.message!, 'Success');

        this.reloadUserFeedbacks();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
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
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
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
        this.toastrService.success(response.message!, 'Success');
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
