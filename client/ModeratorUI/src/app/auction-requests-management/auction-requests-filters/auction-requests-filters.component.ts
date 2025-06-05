import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { QueryParamsService } from "../../services/query-params.service";
import { AuctionRequestStatusEnum } from "../../models/auction-requests/auctionRequestStatusEnum";

@Component({
  selector: 'app-auction-requests-filters',
  templateUrl: './auction-requests-filters.component.html',
  standalone: false
})
export class AuctionRequestsFiltersComponent implements OnInit {
  checkedStatus: AuctionRequestStatusEnum = AuctionRequestStatusEnum.Pending;

  AuctionRequestStatusEnum = AuctionRequestStatusEnum;

  @Output() onFiltersChanged = new EventEmitter<void>();

  @Output() onInitialized = new EventEmitter<void>();

  constructor(private readonly queryParamsService: QueryParamsService) { }

  async ngOnInit(): Promise<void> {
    const status = await this.queryParamsService.getQueryParam('status');

    if (status)
      this.checkedStatus = parseInt(status, 10) as AuctionRequestStatusEnum;
    else
      await this.queryParamsService.setQueryParam('status', this.checkedStatus);

    this.onInitialized.emit();
  }

  async onStatusChanged(status: AuctionRequestStatusEnum) {
    this.checkedStatus = status;

    await this.queryParamsService.setQueryParam('status', this.checkedStatus);

    this.onFiltersChanged.emit();
  }
}
