import { Component, OnInit, TemplateRef } from '@angular/core';
import { AuctionDetailsModel } from 'src/app/models/auctionDetailsModel';
import { AuctionsVerificationService } from 'src/app/services/auctions-verification.service';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RejectAuctionModel } from 'src/app/models/rejectAuctionModel';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-auction-creation-request',
  templateUrl: './auction-creation-request.component.html'
})
export class AuctionCreationRequestComponent implements OnInit {

  auction: AuctionDetailsModel = null;

  rejectionForm: FormGroup;

  constructor(private readonly auctionsVerificationService: AuctionsVerificationService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly toastrService: ToastrService,
    private readonly router: Router,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit(): Promise<void> {
    var auctionId = await this.deepLinkingService.getQueryParam('auctionId');

    if (auctionId == null) {
      this.toastrService.error('Invalid query parameters.', 'Error');
    }

    this.auctionsVerificationService.getNotApporvedAuctionDetailsById(auctionId).subscribe(
      (response) => {
        this.auction = response;

        this.rejectionForm = new FormGroup({
          auctionId: new FormControl(this.auction.id),
          rejectionReason: new FormControl(null, [Validators.required])
        });
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  approveAuction() {
    this.auctionsVerificationService.approveAuction(this.auction.id).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.router.navigate(['/auction-creation-requests-list']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  rejectAuction(modal: any) {
    if (!this.rejectionForm.valid) {
      return;
    }

    modal.close();

    const formValue = this.rejectionForm.value;

    const rejectionModel = {
      auctionId: formValue.auctionId,
      rejectionReason: formValue.rejectionReason
    } as RejectAuctionModel;

    this.auctionsVerificationService.rejectAuction(rejectionModel).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.router.navigate(['/auction-creation-requests-list']);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
