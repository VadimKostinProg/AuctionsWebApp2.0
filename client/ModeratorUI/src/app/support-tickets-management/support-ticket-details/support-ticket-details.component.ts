import { Component, OnInit, TemplateRef } from "@angular/core";
import { SupportTicketsService } from "../../services/support-tickets.service";
import { ToastrService } from "ngx-toastr";
import { AuthService } from "../../services/auth.service";
import { SupportTicket } from "../../models/support-tickets/supportTicket";
import { ActivatedRoute } from "@angular/router";
import { SupportTicketStatusEnum } from "../../models/support-tickets/supportTicketStatusEnum";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { UsersService } from "../../services/users.service";
import { ModeratorInfoBasic } from "../../models/users/moderatorInfoBasic";
import { CompleteSupportTicketModel } from "../../models/support-tickets/completeSupportTicketModel";
import { AssignSupportTicketModel } from "../../models/support-tickets/assignSupportTicketModel";
import { Observable, OperatorFunction, debounceTime, distinctUntilChanged, map } from "rxjs";

@Component({
  selector: 'app-support-ticket-details',
  templateUrl: './support-ticket-details.component.html',
  standalone: false
})
export class SupportTicketDetailsComponent implements OnInit {

  supportTicketId: number | undefined;

  supportTicketDetails: SupportTicket | undefined;

  moderators: ModeratorInfoBasic[] | undefined;

  SupportTicketStatusEnum = SupportTicketStatusEnum;

  // props for modal windows
  assignedModerator: ModeratorInfoBasic | null = null;
  moderatorComment: string | null = null;

  constructor(private readonly supportTicketsService: SupportTicketsService,
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute,
    private readonly modalService: NgbModal,
    private readonly usersService: UsersService) { }

  ngOnInit(): void {
    this.usersService.getAllModerators().subscribe({
      next: result => {
        this.moderators = result.data!;
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });

    this.route.paramMap.subscribe(params => {
      const supportTicketId = params.get('supportTicketId');

      if (supportTicketId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.supportTicketId = parseInt(supportTicketId);

      this.reloadSupportTicketDetails();
    });
  }

  get currentModeratorId() {
    return this.authService.user.userId;
  }

  reloadSupportTicketDetails() {
    this.supportTicketsService.getSupportTicketDetails(this.supportTicketId!).subscribe({
      next: result => {
        this.supportTicketDetails = result.data!;
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  searchModerators: OperatorFunction<string, readonly ModeratorInfoBasic[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.moderators?.filter(mod =>
        (mod.fullName?.toLowerCase().includes(term.toLowerCase()) ||
          mod.username?.toLowerCase().includes(term.toLowerCase()))
        ).slice(0, 10) || [])
    );

  formatter = (mod: ModeratorInfoBasic) => `${mod.fullName} (${mod.username})`;

  assignToMyselfClicked() {
    if (this.moderators) {
      this.assignedModerator = this.moderators.find(m => m.id === this.currentModeratorId) || null;
      if (!this.assignedModerator) {
        this.toastrService.warning('Current moderator not found in the list.', 'Warning');
      }
    } else {
      this.toastrService.warning('Moderators list not loaded yet.', 'Warning');
    }
  }

  assignSupportTicket(modal: any) {
    if (!this.assignedModerator) {
      this.toastrService.error('Please, select moderator to assign this support ticket.', 'Error');
      return;
    }

    modal.close();

    const request = {
      supportTicketId: this.supportTicketId,
      moderatorId: this.assignedModerator.id
    } as AssignSupportTicketModel;

    this.supportTicketsService.assignSupportTicket(request).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.assignedModerator = null;

        this.reloadSupportTicketDetails();
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  completeSupportTicket(modal: any) {
    if (!this.moderatorComment) {
      this.toastrService.error('Please, enter the moderators comment to complete this support ticket.', 'Error');
      return;
    }

    modal.close();

    const request = {
      supportTicketId: this.supportTicketId,
      moderatorComment: this.moderatorComment
    } as CompleteSupportTicketModel;

    this.supportTicketsService.completeSupportTicket(request).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.moderatorComment = null;

        this.reloadSupportTicketDetails();
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
