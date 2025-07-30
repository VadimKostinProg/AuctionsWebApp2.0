import { Component, Input, TemplateRef } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Auction } from "../../models/auctions/auction";
import { ToastrService } from "ngx-toastr";
import { AuctionCommentsService } from "../../services/auction-comments.service";
import { AuctionComment } from "../../models/auctions/auctionComment";

@Component({
  selector: 'app-auction-comments',
  templateUrl: './auction-comments.component.html',
  standalone: false
})
export class AuctionCommentsComponent {
  @Input() auction: Auction | undefined;

  comments: AuctionComment[] = [];

  choosenComment?: AuctionComment | null;

  currentPage: number = 0;
  pageSize: number = 10;
  hasNext: boolean = false;

  constructor(
    private readonly auctionCommentsService: AuctionCommentsService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit(): Promise<void> {
    this.loadComments();
  }

  reloadComments() {
    this.comments = [];
    this.currentPage = 0;
    this.hasNext = false;

    this.loadComments();
  }

  loadComments() {
    if (this.auction)
      this.auctionCommentsService.getCommentsForAuction(this.auction.id, ++this.currentPage, this.pageSize).subscribe({
        next: (response) => {
          this.comments = [...this.comments, ...response.data!.items];
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

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  openActionModal(content: TemplateRef<any>, comment: AuctionComment) {
    this.choosenComment = comment;

    this.open(content);
  }

  deleteComment(modal: any) {
    modal.close();

    this.auctionCommentsService.deleteComment(this.choosenComment!.id).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadComments();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
