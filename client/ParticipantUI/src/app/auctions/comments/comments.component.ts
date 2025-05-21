import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CommentsService } from '../../services/comments.service';
import { PaginationModel } from '../../models/shared/paginationModel';
import { CommentsQueryParamsService } from '../../services/comments-query-params.service';
import { AuctionComment } from '../../models/auctions/AuctionComment';
import { UserBasic } from '../../models/users/userBasic';
import { AuthService } from '../../services/auth.service';
import { PostComment } from '../../models/auctions/PostComment';
import { PostComplaint } from '../../models/complaints/postComplaint';
import { ComplaintsService } from '../../services/complaints.service';
import { ComplaintTypeEnum } from '../../models/complaints/complaintTypeEnum';

@Component({
  selector: 'comments',
  templateUrl: './comments.component.html',
  standalone: false
})
export class CommentsComponent implements OnInit {
  @Input() auctionId!: number;

  currentUser!: UserBasic;

  commentForm!: FormGroup;

  complaintForm!: FormGroup;

  comments: AuctionComment[] = [];

  choosenComment?: AuctionComment | null;

  pagination: PaginationModel | undefined;

  constructor(private readonly authService: AuthService,
    private readonly commentsService: CommentsService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService,
    private readonly queryParamsService: CommentsQueryParamsService) {

  }

  get userStatus() {
    return this.authService.userStatus;
  }

  async ngOnInit(): Promise<void> {
    this.currentUser = this.authService.user;

    this.pagination = await this.queryParamsService.getPaginationParams();

    this.reloadComments();

    this.reloadCommentForm();

    this.reloadComplaintForm();
  }

  reloadComments() {
    this.commentsService.getCommentsForAuction(this.auctionId, this.pagination!.pageNumber!, this.pagination!.pageSize!).subscribe({
      next: (response) => {
        if (response.isSuccessfull)
          this.comments = response.data!.items;
        else
          this.toastrService.error(response.errors[0], 'Error');
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  reloadCommentForm() {
    this.commentForm = new FormGroup({
      commentText: new FormControl(null, [Validators.required]),
      score: new FormControl(null, [Validators.required]),
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

  openActionModal(content: TemplateRef<any>, comment: AuctionComment) {
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
    const score = this.commentForm.value.score;

    this.reloadCommentForm();

    const comment = {
      score: score,
      auctionId: this.auctionId,
      commentText: commentText
    } as PostComment;

    this.commentsService.postNewComment(comment).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadComments();
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  deleteComment(modal: any) {
    modal.close();

    this.commentsService.deleteComment(this.choosenComment!.id).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadComments();
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }

  complainOnComment(modal: any) {
    if (!this.complaintForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintForm.value.complaintText;

    this.reloadCommentForm();

    const complaint = {
      accusedUserId: this.choosenComment!.userId,
      accusedCommentId: this.choosenComment!.id,
      complaintText: complaintText,
      type: ComplaintTypeEnum.ComplaintOnAuctionComment
    } as PostComplaint;

    this.complaintsService.postComplaint(complaint).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadComments();
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }
}
