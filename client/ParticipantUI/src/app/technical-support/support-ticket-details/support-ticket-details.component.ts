import { Component, OnInit } from "@angular/core";
import { SupportTicket } from "../../models/support-tickets/supportTicket";
import { SupportTicketsService } from "../../services/support-tickets.service";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-support-ticket-details',
  templateUrl: './support-ticket-details.component.html',
  styleUrl: './support-ticket-details.component.scss',
  standalone: false
})
export class SupportTicketDetailsComponent implements OnInit {
  supportTicket: SupportTicket | undefined;

  constructor(private readonly supportTicketsService: SupportTicketsService,
    private readonly toastrService: ToastrService,
    private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const supportTicketId = params.get('supportTicketId');

      if (supportTicketId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.supportTicketsService.getSupportTicketDetails(parseInt(supportTicketId))
        .subscribe({
          next: result => {
            this.supportTicket = result.data!;
          },
          error: (err) => {
            if (err?.error?.errors && Array.isArray(err.error.errors)) {
              this.toastrService.error(err.error.errors[0], 'Error');
            }
          }
        });
    });
  }
}
