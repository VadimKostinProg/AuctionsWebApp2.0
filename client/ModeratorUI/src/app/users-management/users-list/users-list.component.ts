import { Component, OnInit, ViewChild } from "@angular/core";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { DataTableComponent } from "../../shared/data-table/data-table.component";
import { UsersService } from "../../services/users.service";

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  standalone: false
})
export class UsersListComponent implements OnInit {
  @ViewChild(DataTableComponent) dateTable?: DataTableComponent | null;

  dataTableApiUrl: string | undefined;
  dataTableOptions: DataTableOptionsModel | undefined;

  showList: boolean = false;

  constructor(private readonly usersService: UsersService) { }

  ngOnInit(): void {
    this.dataTableApiUrl = this.usersService.getDataTableApiUrl();
    this.dataTableOptions = this.usersService.getDataTableOptions();
  }

  async onFiltersChanged() {
    if (this.showList && this.dateTable)
      await this.dateTable.reloadDatatable();
    else
      this.showList = true;
  }
}
