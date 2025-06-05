import { Component, OnInit, ViewChild } from "@angular/core";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { AuctionsService } from "../../services/auctions.service";
import { DataTableComponent } from "../../shared/data-table/data-table.component";

@Component({
  selector: 'app-auctions-list',
  templateUrl: './auctions-list.component.html',
  standalone: false
})
export class AuctionsListComponent implements OnInit {
  @ViewChild(DataTableComponent) dateTable?: DataTableComponent | null;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  showList: boolean = false;

  constructor(private readonly auctionsService: AuctionsService) { }

  ngOnInit(): void {
    this.dataTableApiUrl = this.auctionsService.baseUrl;
    this.dataTableOptions = this.auctionsService.getAuctionsDataTableOptions();
  }

  async onFiltersChanged() {
    if (this.showList && this.dateTable)
      await this.dateTable.reloadDatatable();
    else
      this.showList = true;
  }
}
