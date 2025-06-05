import { Component } from "@angular/core";
import { AuctionsService } from "../../services/auctions.Service";

@Component({
  selector: 'app-user-auctions-history',
  templateUrl: 'user-auctions-history.component.html',
  standalone: false
})
export class UserAuctionsHistoryComponent {

  constructor(private readonly auctionsService: AuctionsService) { }

  get dataTableApiUrl() {
    return this.auctionsService.getAuctionsHistoryDataTableApiUrl();
  }

  get dataTableOptions() {
    return this.auctionsService.getAuctionsHistoryDataTableOptions();
  }
}
