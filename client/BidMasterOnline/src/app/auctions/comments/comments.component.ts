import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { AuthenticationModel } from 'src/app/models/authenticationModel';
import { CommentsService } from 'src/app/services/comments.service';
import { CommentModel } from 'src/app/models/commentModel';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SetCommentModel } from 'src/app/models/setCommentModel';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComplaintsService } from 'src/app/services/complaints.service';
import { ComplaintTypeEnum } from 'src/app/models/complaintTypeEnum';
import { SetComplaintModel } from 'src/app/models/setComplaintModel';

@Component({
  selector: 'comments',
  templateUrl: './comments.component.html'
})
export class CommentsComponent implements OnInit {
  @Input()
  auctionId: string;

  user: AuthenticationModel;

  commentForm: FormGroup;

  complaintForm: FormGroup;

  comments: CommentModel[];

  choosenComment: CommentModel;

  constructor(private readonly authService: AuthService,
    private readonly commentsService: CommentsService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService) {

  }

  ngOnInit(): void {
    this.user = this.authService.user;

    this.reloadComments();

    this.reloadCommentForm();

    this.reloadComplaintForm();
  }

  reloadComments() {
    this.commentsService.getCommentsForAuction(this.auctionId).subscribe(
      (response) => {
        this.comments = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  reloadCommentForm() {
    this.commentForm = new FormGroup({
      commentText: new FormControl(null, [Validators.required])
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

  openActionModal(content: TemplateRef<any>, comment: CommentModel) {
    this.choosenComment = comment;

    this.open(content);
  }

  get commentText() {
    return this.commentForm.get('commentText');
  }

  get complaintText() {
    return this.complaintForm.get('complaintText');
  }

  leaveComment(modal: any) {
    if (!this.commentForm.valid) {
      return;
    }

    modal.close();

    var commentText = this.commentForm.value.commentText;

    this.reloadCommentForm();

    const comment = {
      auctionId: this.auctionId,
      commentText: commentText
    } as SetCommentModel;

    this.commentsService.setNewComment(comment).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadComments();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  deleteComment(modal: any) {
    modal.close();

    switch (this.user.role) {
      case 'Customer':
        this.commentsService.deleteOwnComment(this.choosenComment.id).subscribe(
          (response) => {
            this.toastrService.success(response.message, 'Success');

            this.reloadComments();
          },
          (error) => {
            this.toastrService.error(error.error, 'Error');
          }
        );
        break;
      case 'TechnicalSupportSpecialist':
        this.commentsService.deleteComment(this.choosenComment.id).subscribe(
          (response) => {
            this.toastrService.success(response.message, 'Success');

            this.reloadComments();
          },
          (error) => {
            this.toastrService.error(error.error, 'Error');
          }
        );
        break;
    }
  }

  complainOnComment(modal: any) {
    if (!this.complaintForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintForm.value.complaintText;

    this.reloadCommentForm();

    const complaint = {
      accusedUserId: this.choosenComment.userId,
      auctionId: this.auctionId,
      commentId: this.choosenComment.id,
      complaintType: ComplaintTypeEnum.ComplaintOnUserComment,
      complaintText: complaintText
    } as SetComplaintModel;

    this.complaintsService.setComplaint(complaint).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadComments();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
