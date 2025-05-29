import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CommentsService } from '../../services/comments.service';
import { PaginationModel } from '../../models/shared/paginationModel';
import { CommentsQueryParamsService } from '../../services/comments-query-params.service';
import { AuctionComment } from '../../models/auctions/AuctionComment';
import { AuthService } from '../../services/auth.service';
import { PostComment } from '../../models/auctions/PostComment';
import { PostComplaint } from '../../models/complaints/postComplaint';
import { ComplaintsService } from '../../services/complaints.service';
import { ComplaintTypeEnum } from '../../models/complaints/complaintTypeEnum';
import { Auction } from '../../models/auctions/Auction';
import { UserStatusEnum } from '../../models/user-profiles/userStatusEnum';

@Component({
  selector: 'comments',
  templateUrl: './comments.component.html',
  standalone: false
})
export class CommentsComponent implements OnInit {
  @Input() auction: Auction | undefined;

  commentForm!: FormGroup;

  complaintForm!: FormGroup;

  comments: AuctionComment[] = [];

  choosenComment?: AuctionComment | null;

  pagination: PaginationModel | undefined;

  UserStatusEnum = UserStatusEnum;

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

  get userId() {
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

    this.reloadComments();

    this.reloadCommentForm();

    this.reloadComplaintForm();
  }

  reloadComments() {
    if (this.auction)
      this.commentsService.getCommentsForAuction(this.auction.id, this.pagination!.pageNumber!, this.pagination!.pageSize!).subscribe({
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

  onScoreUpdated(newScore: number) {
    if (this.commentForm) {
      this.commentForm.patchValue({ score: newScore });
    }
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
      auctionId: this.auction!.id,
      commentText: commentText
    } as PostComment;

    this.commentsService.postNewComment(comment).subscribe({
      next: (response) => {
        if (response.isSuccessfull) {
          this.toastrService.success(response.message!, 'Success');

          this.reloadComments();
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
