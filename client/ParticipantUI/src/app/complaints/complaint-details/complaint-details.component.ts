import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { Complaint } from "../../models/complaints/complaint";
import { ComplaintsService } from "../../services/complaints.service";

@Component({
  selector: 'app-complaint-details',
  templateUrl: './complaint-details.component.html',
  styleUrl: './complaint-details.component.scss',
  standalone: false
})
export class ComplaintDetailsComponent implements OnInit {
  complaint: Complaint | undefined;

  constructor(private readonly complaintsService: ComplaintsService,
    private readonly toastrService: ToastrService,
    private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const complaintId = params.get('complaintId');

      if (complaintId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.complaintsService.getComplaintDetails(parseInt(complaintId))
        .subscribe({
          next: result => {
            this.complaint = result.data!;
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
