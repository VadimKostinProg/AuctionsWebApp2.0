import { Component, OnInit } from "@angular/core";
import { SupportTicketsService } from "../../services/support-tickets.service";

@Component({
  selector: 'app-support-tickets-history',
  templateUrl: './support-tickets-history.component.html',
  standalone: false
})
export class SupportTicketsHistoryComponent {
  constructor(private readonly supportTicketsService: SupportTicketsService) { }

  get dataTableOptions() {
    return this.supportTicketsService.getSupportTicketsDataTableOptions();
  }

  get dataTableUrl() {
    return this.supportTicketsService.getSupportTicketsListUrl();
  }
}
