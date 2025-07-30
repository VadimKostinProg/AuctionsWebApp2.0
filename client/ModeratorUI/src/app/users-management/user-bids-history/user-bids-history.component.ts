import { Component, OnInit } from "@angular/core";
import { BidsService } from "../../services/bids.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute } from "@angular/router";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";

@Component({
  selector: 'app-user-bids-history',
  templateUrl: 'user-bids-history.component.html',
  standalone: false
})
export class UserBidsHistoryComponent {

  userId: number | undefined;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  constructor(private readonly bidsService: BidsService,
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

      this.dataTableApiUrl = this.bidsService.getUserBidsHistoryDataTableApiUrl(this.userId);
      this.dataTableOptions = this.bidsService.getUserBidsHistoryDataTableOptions();
    });
  }
}
