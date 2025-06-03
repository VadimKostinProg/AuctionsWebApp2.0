import { Component, OnInit, ViewChild } from "@angular/core";
import { AuctionRequestsService } from "../../services/auction-requests.service";
import { DataTableComponent } from "../../shared/data-table/data-table.component";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";

@Component({
  selector: 'app-auction-requests-list',
  templateUrl: './auction-requests-list.component.html',
  standalone: false
})
export class AuctionRequestsListComponent implements OnInit {

  @ViewChild(DataTableComponent) dateTable?: DataTableComponent | null;

  placeholder: string = 'Search auction request...';

  showList: boolean = false;

  dataTableApiUrl: string | undefined;

  dataTableOptions: DataTableOptionsModel | undefined;

  constructor(private readonly auctionRequestsService: AuctionRequestsService) { }

  ngOnInit(): void {
    this.dataTableApiUrl = this.auctionRequestsService.getAuctionRequestsDataTableApiUrl();
    this.dataTableOptions = this.auctionRequestsService.getAuctionReuqestsDataTableOptions();
  }

  async onSearchClicked() {
    if (this.dateTable)
      await this.dateTable.reloadDatatable();
  }

  async onFiltersChanged() {
    if (this.dateTable)
      await this.dateTable.reloadDatatable();
  }
}
