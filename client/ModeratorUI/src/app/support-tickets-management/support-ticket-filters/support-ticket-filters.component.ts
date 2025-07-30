import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { QueryParamsService } from "../../services/query-params.service";
import { SupportTicketStatusEnum } from "../../models/support-tickets/supportTicketStatusEnum";
import { AuthService } from "../../services/auth.service";

@Component({
  selector: 'app-support-ticket-filters',
  templateUrl: './support-ticket-filters.component.html',
  standalone: false
})
export class SupportTicketFiltersComponent implements OnInit {
  checkedStatus: SupportTicketStatusEnum = SupportTicketStatusEnum.Pending;

  showAssignedOnMeFlag: boolean = false;

  assignedOnMeChecked: boolean = false;

  moderatorId!: number | undefined;

  SupportTicketStatusEnum = SupportTicketStatusEnum;

  @Output() onFiltersChanged = new EventEmitter<void>();

  constructor(private readonly queryParamsService: QueryParamsService,
    private readonly authService: AuthService) { }

  async ngOnInit(): Promise<void> {
    this.moderatorId = this.authService.user.userId;

    const status = await this.queryParamsService.getQueryParam('status');

    if (status)
      this.checkedStatus = parseInt(status, 10) as SupportTicketStatusEnum;
    else
      await this.queryParamsService.setQueryParam('status', this.checkedStatus);
  }

  async onStatusChanged(status: SupportTicketStatusEnum) {
    this.checkedStatus = status;

    await this.queryParamsService.setQueryParam('status', this.checkedStatus);

    if (this.checkedStatus === SupportTicketStatusEnum.Pending) {
      this.showAssignedOnMeFlag = false;
      this.queryParamsService.clearQueryParam('moderatorId');
    }
    else {
      this.showAssignedOnMeFlag = true;
      await this.updateQueryParamsForAssignedOnMeFlag();
    }

    this.onFiltersChanged.emit();
  }

  async onAssignedOnMeChanged() {
    await this.updateQueryParamsForAssignedOnMeFlag();

    this.onFiltersChanged.emit();
  }

  private async updateQueryParamsForAssignedOnMeFlag() {
    if (this.assignedOnMeChecked) {
      await this.queryParamsService.setQueryParam('moderatorId', this.moderatorId);
    }
    else {
      await this.queryParamsService.clearQueryParam('moderatorId');
    }
  }
}
