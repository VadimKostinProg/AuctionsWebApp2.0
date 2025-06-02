import { Component } from "@angular/core";
import { AuctionRequestsService } from "../../services/auction-requests.service";

@Component({
  selector: 'app-auction-requests',
  templateUrl: './auction-requests-history.component.html',
  standalone: false
})
export class AuctionRequestsHistoryComponent {

  constructor(private readonly auctionRequestsService: AuctionRequestsService) { }

  get dataTableApiUrl() {
    return this.auctionRequestsService.getAuctionRequestsHistoryDataTableApiUrl();
  }

  get dataTableOptions() {
    return this.auctionRequestsService.getAuctionRequestsHistoryDataTableOptions();
  }
}
