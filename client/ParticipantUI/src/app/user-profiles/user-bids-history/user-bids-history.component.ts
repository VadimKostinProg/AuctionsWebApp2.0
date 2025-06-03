import { Component, OnInit } from "@angular/core";
import { BidsService } from "../../services/bids.service";

@Component({
  selector: 'app-user-bids-history',
  templateUrl: 'user-bids-history.component.html',
  standalone: false
})
export class UserBidsHistoryComponent {

  constructor(private readonly bidsService: BidsService) { }

  get dataTableApiUrl() {
    return this.bidsService.getBidsHistoryDataTableApiUrl();
  }

  get dataTableOptions() {
    return this.bidsService.getBidsHistoryDataTableOptions();
  }
}
