import { Component, OnInit, TemplateRef } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { AuthService } from "../../services/auth.service";
import { ActivatedRoute } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { UsersService } from "../../services/users.service";
import { ModeratorInfoBasic } from "../../models/users/moderatorInfoBasic";
import { Observable, OperatorFunction, debounceTime, distinctUntilChanged, map } from "rxjs";
import { Complaint } from "../../models/complaints/complaint";
import { ComplaintStatusEnum } from "../../models/complaints/complaintStatusEnum";
import { ComplaintsService } from "../../services/complaints.service";
import { AssignComplaintModel } from "../../models/complaints/assignComplaintModel";
import { CompleteComplaintModel } from "../../models/complaints/completeComplaintModel";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";

@Component({
  selector: 'app-complaint-details',
  templateUrl: './complaint-details.component.html',
  standalone: false
})
export class ComplaintDetailsComponent implements OnInit {

  complaintId: number | undefined;

  complaintDetails: Complaint | undefined;

  moderators: ModeratorInfoBasic[] | undefined;

  ComplaintStatusEnum = ComplaintStatusEnum;
  ComplaintTypeEnum = ComplaintTypeEnum;

  // props for modal windows
  assignedModerator: ModeratorInfoBasic | null = null;
  moderatorConclusion: string | null = null;

  constructor(private readonly complaintsService: ComplaintsService,
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
      const complaintId = params.get('complaintId');

      if (complaintId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.complaintId = parseInt(complaintId);

      this.reloadComplaintDetails();
    });
  }

  get currentModeratorId() {
    return this.authService.user.userId;
  }

  reloadComplaintDetails() {
    this.complaintsService.getComplaintDetails(this.complaintId!).subscribe({
      next: result => {
        this.complaintDetails = result.data!;
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

  assignComplaint(modal: any) {
    if (!this.assignedModerator) {
      this.toastrService.error('Please, select moderator to assign this complaint.', 'Error');
      return;
    }

    modal.close();

    const request = {
      complaintId: this.complaintId,
      moderatorId: this.assignedModerator.id
    } as AssignComplaintModel;

    this.complaintsService.assignComplaint(request).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.assignedModerator = null;

        this.reloadComplaintDetails();
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  completeComplaint(modal: any) {
    if (!this.moderatorConclusion) {
      this.toastrService.error('Please, enter the moderators conclusion to complete this complaint.', 'Error');
      return;
    }

    modal.close();

    const request = {
      complaintId: this.complaintId,
      moderatorConclusion: this.moderatorConclusion
    } as CompleteComplaintModel;

    this.complaintsService.completeComplaint(request).subscribe({
      next: result => {
        this.toastrService.success(result.message!, 'Success');

        this.moderatorConclusion = null;

        this.reloadComplaintDetails();
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  getComplaintTypeText(type: ComplaintTypeEnum): string {
    switch (type) {
      case ComplaintTypeEnum.ComplaintOnAuctionContent:
        return 'On Auction Content';
      case ComplaintTypeEnum.ComplaintOnAuctionComment:
        return 'On Auction Comment';
      case ComplaintTypeEnum.ComplaintOnUserBehaviour:
        return 'On User Behaviour';
      case ComplaintTypeEnum.ComplaintOnUserFeedback:
        return 'On User Feedback';
      default:
        return 'Unknown Type';
    }
  }
}
