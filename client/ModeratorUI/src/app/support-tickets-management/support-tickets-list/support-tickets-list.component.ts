import { Component, OnInit, ViewChild } from "@angular/core";
import { SupportTicketsService } from "../../services/support-tickets.service";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { DataTableComponent } from "../../shared/data-table/data-table.component";

@Component({
  selector: 'app-support-tickets-list',
  templateUrl: './support-tickets-list.component.html',
  standalone: false
})
export class SupportTicketsListComponent implements OnInit {

  @ViewChild(DataTableComponent) dateTable?: DataTableComponent | null;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  constructor(private readonly supportTicketsService: SupportTicketsService) { }

  ngOnInit(): void {
    this.dataTableApiUrl = this.supportTicketsService.getDataTableApiUrl();
    this.dataTableOptions = this.supportTicketsService.getDataTableOptions();
  }

  async onFiltersChanged() {
    if (this.dateTable)
      await this.dateTable.reloadDatatable();
  }
}
