import { Component, OnInit, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TechnicalSupportRequestModel } from 'src/app/models/technicalSupportRequestModel';
import { TechnicalSupportRequestsService } from 'src/app/services/technical-support-requests.service';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-technical-support-request',
  templateUrl: './technical-support-request.component.html'
})
export class TechnicalSupportRequestComponent implements OnInit {

  request: TechnicalSupportRequestModel;

  constructor(private readonly tsrService: TechnicalSupportRequestsService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly toastrService: ToastrService,
    private readonly router: Router,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit(): Promise<void> {
    var requestId = await this.deepLinkingService.getQueryParam('requestId');

    if (requestId == null) {
      this.toastrService.error('Invalid query params.', 'Error');
    }

    this.tsrService.getTechnicalSupportRequestById(requestId).subscribe(
      (response) => {
        this.request = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  handleRequest(modal: any) {
    modal.close();

    this.tsrService.handleTechnicalSupportRequest(this.request.id).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.router.navigate(['technical-support-requests']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
