import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { SupportTicketsService } from "../../services/support-tickets.service";
import { ToastrService } from "ngx-toastr";
import { PostSupportTicket } from "../../models/support-tickets/postSupportTicket";

@Component({
  selector: 'app-technical-support',
  templateUrl: './technical-support.component.html',
  standalone: false,
})
export class TechnicalSupportComponent implements OnInit {
  requestForm: FormGroup | undefined;

  constructor(private readonly supportTicketsService: SupportTicketsService,
    private readonly toastrService: ToastrService) { }

  ngOnInit(): void {
    this.reloadRequestForm();
  }

  get supportTicketTitle() {
    return this.requestForm!.get('supportTicketTitle');
  }

  get supportTicketText() {
    return this.requestForm!.get('supportTicketText');
  }

  reloadRequestForm() {
    this.requestForm = new FormGroup({
      supportTicketTitle: new FormControl(null, [Validators.required]),
      supportTicketText: new FormControl(null, [Validators.required])
    });
  }

  sendSupportTicket() {
    if (!this.requestForm!.valid) {
      return;
    }

    const formValue = this.requestForm!.value;

    const supportTicket = {
      title: formValue.supportTicketTitle,
      text: formValue.supportTicketText
    } as PostSupportTicket;

    this.supportTicketsService.postSupportTicket(supportTicket).subscribe({
      next: (response) => {
        if (response.isSuccessfull) {
          this.toastrService.success(response.message!, 'Success');

          this.reloadRequestForm();
        }
        else
          this.toastrService.error(response.errors[0], 'Error');
      },
      error: (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    });
  }
}
