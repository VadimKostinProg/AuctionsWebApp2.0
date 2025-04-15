import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ComplaintModel } from 'src/app/models/complaintModel';
import { ComplaintTypeEnum } from 'src/app/models/complaintTypeEnum';
import { ComplaintsService } from 'src/app/services/complaints.service';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';

@Component({
  selector: 'app-complaint-details',
  templateUrl: './complaint-details.component.html'
})
export class ComplaintDetailsComponent implements OnInit {

  complaint: ComplaintModel;

  showComment: boolean = false;

  constructor(private readonly complaintsService: ComplaintsService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly toastrService: ToastrService,
    private readonly router: Router,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit(): Promise<void> {
    var complaintId = await this.deepLinkingService.getQueryParam('complaintId');

    if (complaintId == null) {
      this.toastrService.error('Invalid query params.', 'Error');
    }

    this.complaintsService.getComplaintById(complaintId).subscribe(
      (response) => {
        this.complaint = response;

        this.showComment = this.complaint.complaintType == ComplaintTypeEnum.ComplaintOnUserComment;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  handleComplaint(modal: any) {
    modal.close();

    this.complaintsService.handleComplaint(this.complaint.id).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.router.navigate(['/complaints']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
