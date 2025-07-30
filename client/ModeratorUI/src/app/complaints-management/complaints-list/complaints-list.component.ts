import { Component, OnInit, ViewChild } from "@angular/core";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { DataTableComponent } from "../../shared/data-table/data-table.component";
import { ComplaintsService } from "../../services/complaints.service";

@Component({
  selector: 'app-complaints-list',
  templateUrl: './complaints-list.component.html',
  standalone: false
})
export class ComplaintsListComponent implements OnInit {

  @ViewChild(DataTableComponent) dateTable?: DataTableComponent | null;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  constructor(private readonly complaintsService: ComplaintsService) { }

  ngOnInit(): void {
    this.dataTableApiUrl = this.complaintsService.getDataTableApiUrl();
    this.dataTableOptions = this.complaintsService.getDataTableOptions();
  }

  async onFiltersChanged() {
    if (this.dateTable)
      await this.dateTable.reloadDatatable();
  }
}
