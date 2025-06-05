import { Component, Input, TemplateRef } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ToastrService } from "ngx-toastr";
import { UserFeedback } from "../../models/users/userFeedback";
import { UserFeedbacksService } from "../../services/user-feedbacks.service";

@Component({
  selector: 'app-user-feedbacks',
  templateUrl: './user-feedbacks.component.html',
  standalone: false
})
export class UserFeedbacksComponent {
  @Input() userId!: number;

  feedbacks: UserFeedback[] = [];

  choosenFeedback?: UserFeedback | null;

  currentPage: number = 0;
  pageSize: number = 10;
  hasNext: boolean = false;

  constructor(
    private readonly userFeedbacksService: UserFeedbacksService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit(): Promise<void> {
    this.loadFeedbacks();
  }

  reloadFeedbacks() {
    this.feedbacks = [];
    this.currentPage = 0;
    this.hasNext = false;

    this.loadFeedbacks();
  }

  loadFeedbacks() {
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

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  openActionModal(content: TemplateRef<any>, userFeedback: UserFeedback) {
    this.choosenFeedback = userFeedback;

    this.open(content);
  }

  deleteFeedback(modal: any) {
    modal.close();

    this.userFeedbacksService.deleteUserFeedback(this.choosenFeedback!.id).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadFeedbacks();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
