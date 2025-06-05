import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { QueryParamsService } from "../../services/query-params.service";
import { UserStatusEnum } from "../../models/users/userStatusEnum";

@Component({
  selector: 'app-user-filters',
  templateUrl: './user-filters.component.html',
  standalone: false,
})
export class UserFiltersComponent implements OnInit {
  userId: number | null = null;
  searchTerm: string | null = null;
  selectedStatus: UserStatusEnum | null = null;

  UserStatusEnum = UserStatusEnum;

  statusOptions = Object.keys(UserStatusEnum)
    .filter(key => isNaN(Number(key)))
    .map(key => ({
      key: key,
      value: UserStatusEnum[key as keyof typeof UserStatusEnum]
    }));

  @Output() onFiltersChanged = new EventEmitter<void>();

  constructor(
    private readonly queryParamsService: QueryParamsService
  ) { }

  async ngOnInit(): Promise<void> {
    const userIdParam = this.queryParamsService.getQueryParam('userId') || null;
    if (userIdParam) {
      this.userId = parseInt(userIdParam);
    } else {
      this.userId = null;
    }

    this.searchTerm = this.queryParamsService.getQueryParam('searchTerm') || null;

    const statusParam = this.queryParamsService.getQueryParam('status');
    if (statusParam !== null && statusParam !== undefined) {
      this.selectedStatus = parseInt(statusParam, 10) as UserStatusEnum;
    } else {
      this.selectedStatus = null;
    }
  }

  async onSearchClicked(): Promise<void> {
    await this.updateQueryParams();
    this.onFiltersChanged.emit();
  }

  async updateQueryParams(): Promise<void> {
    await this.queryParamsService.setQueryParam('userId', this.userId);
    await this.queryParamsService.setQueryParam('searchTerm', this.searchTerm);
    await this.queryParamsService.setQueryParam('status', this.selectedStatus);
  }

  async clearFilters(): Promise<void> {
    this.userId = null;
    this.searchTerm = null;
    this.selectedStatus = null;
    await this.onSearchClicked();
  }

  getUserStatusText(statusValue: UserStatusEnum): string {
    switch (statusValue) {
      case UserStatusEnum.Active: return 'Active';
      case UserStatusEnum.Blocked: return 'Blocked';
      case UserStatusEnum.Deleted: return 'Deleted';
      default: return 'Unknown';
    }
  }
}
