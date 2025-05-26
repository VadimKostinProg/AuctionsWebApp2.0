import { Component, OnInit } from "@angular/core";
import { ComplaintsService } from "../../services/complaints.service";

@Component({
  selector: 'app-complaints-history',
  templateUrl: './complaints-history.component.html',
  standalone: false
})
export class ComplaintsHistoryComponent {
  constructor(private readonly complaintsService: ComplaintsService) { }

  get dataTableOptions() {
    return this.complaintsService.getComplaintsDataTableOptions();
  }

  get dataTableUrl() {
    return this.complaintsService.getComplaintsListUrl();
  }
}
