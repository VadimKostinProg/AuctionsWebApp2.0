import { Component, OnInit } from "@angular/core";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AuctionsService } from "../../services/auctions.service";

@Component({
  selector: 'app-user-auctions-history',
  templateUrl: 'user-auctions-history.component.html',
  standalone: false
})
export class UserAuctionsHistoryComponent implements OnInit {

  userId: number | undefined;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  constructor(private readonly auctionsService: AuctionsService,
    private readonly toastrService: ToastrService,
    private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const userId = params.get('userId');

      if (userId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.userId = parseInt(userId);

      this.dataTableApiUrl = this.auctionsService.getAuctionsHistoryDataTableApiUrl(this.userId);
      this.dataTableOptions = this.auctionsService.getAuctionsHistoryDataTableOptions();
    });
  }
}
