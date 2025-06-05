import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { QueryParamsService } from "../../services/query-params.service";
import { AuthService } from "../../services/auth.service";
import { ComplaintStatusEnum } from "../../models/complaints/complaintStatusEnum";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";

@Component({
  selector: 'app-complaint-filters',
  templateUrl: './complaint-filters.component.html',
  standalone: false
})
export class ComplaintFiltersComponent implements OnInit {
  checkedStatus: ComplaintStatusEnum = ComplaintStatusEnum.Pending;
  selectedComplaintType: ComplaintTypeEnum | null = null;

  showAssignedOnMeFlag: boolean = false;
  assignedOnMeChecked: boolean = false;
  moderatorId!: number | undefined;

  ComplaintStatusEnum = ComplaintStatusEnum;
  ComplaintTypeEnum = ComplaintTypeEnum;

  complaintTypes = [
    { value: ComplaintTypeEnum.ComplaintOnAuctionContent, name: 'On Auction Content' },
    { value: ComplaintTypeEnum.ComplaintOnAuctionComment, name: 'On Auction Comment' },
    { value: ComplaintTypeEnum.ComplaintOnUserBehaviour, name: 'On User Behaviour' },
    { value: ComplaintTypeEnum.ComplaintOnUserFeedback, name: 'On User Feedback' }
  ];

  @Output() onFiltersChanged = new EventEmitter<void>();

  constructor(private readonly queryParamsService: QueryParamsService,
    private readonly authService: AuthService) { }

  async ngOnInit(): Promise<void> {
    this.moderatorId = this.authService.user.userId;

    const status = await this.queryParamsService.getQueryParam('status');
    if (status) {
      this.checkedStatus = parseInt(status, 10) as ComplaintStatusEnum;
    } else {
      await this.queryParamsService.setQueryParam('status', this.checkedStatus);
    }

    this.showAssignedOnMeFlag = this.checkedStatus !== ComplaintStatusEnum.Pending;

    const moderatorIdParam = await this.queryParamsService.getQueryParam('moderatorId');
    this.assignedOnMeChecked = moderatorIdParam === this.moderatorId?.toString();

    const type = await this.queryParamsService.getQueryParam('type');
    if (type) {
      this.selectedComplaintType = parseInt(type, 10) as ComplaintTypeEnum;
    }

    this.onFiltersChanged.emit();
  }

  async onStatusChanged(status: ComplaintStatusEnum) {
    this.checkedStatus = status;
    await this.queryParamsService.setQueryParam('status', this.checkedStatus);

    if (this.checkedStatus === ComplaintStatusEnum.Pending) {
      this.showAssignedOnMeFlag = false;
      this.assignedOnMeChecked = false;
      await this.queryParamsService.clearQueryParam('moderatorId');
    } else {
      this.showAssignedOnMeFlag = true;
      await this.updateQueryParamsForAssignedOnMeFlag();
    }

    this.onFiltersChanged.emit();
  }

  async onAssignedOnMeChanged() {
    await this.updateQueryParamsForAssignedOnMeFlag();
    this.onFiltersChanged.emit();
  }

  async onTypeChanged() {
    if (this.selectedComplaintType !== null) {
      await this.queryParamsService.setQueryParam('type', this.selectedComplaintType);
    } else {
      await this.queryParamsService.clearQueryParam('type');
    }
    this.onFiltersChanged.emit();
  }

  private async updateQueryParamsForAssignedOnMeFlag() {
    if (this.assignedOnMeChecked) {
      await this.queryParamsService.setQueryParam('moderatorId', this.moderatorId);
    } else {
      await this.queryParamsService.clearQueryParam('moderatorId');
    }
  }
}
