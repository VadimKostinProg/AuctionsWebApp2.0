import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { AuctionDetailsModel } from 'src/app/models/auctionDetailsModel';
import { ToastrService } from 'ngx-toastr';
import { AuctionsService } from 'src/app/services/auctions.service';
import { AuthService } from 'src/app/services/auth.service';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthenticationModel } from 'src/app/models/authenticationModel';
import { SetBidModel } from 'src/app/models/setBidModel';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { CancelAuctionModel } from 'src/app/models/cancelAuctionModel';
import { ComplaintTypeEnum } from 'src/app/models/complaintTypeEnum';
import { SetComplaintModel } from 'src/app/models/setComplaintModel';
import { ComplaintsService } from 'src/app/services/complaints.service';

@Component({
  selector: 'app-auction-details',
  templateUrl: './auction-details.component.html'
})
export class AuctionDetailsComponent implements OnInit {

  @ViewChild(DataTableComponent)
  bidsDataTable: DataTableComponent;

  auctionDetails: AuctionDetailsModel = null;

  bidsDataTableOptions: DataTableOptionsModel;

  setBidForm: FormGroup;

  cancelationForm: FormGroup;

  complaintOnAuctionForm: FormGroup;

  maxBidAmount: number = 10e7;

  user: AuthenticationModel;

  constructor(private readonly deepLinkingService: DeepLinkingService,
    private readonly toastrService: ToastrService,
    private readonly auctionsService: AuctionsService,
    private readonly authService: AuthService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService) {

  }

  async ngOnInit() {
    var auctionId = await this.deepLinkingService.getQueryParam('auctionId');

    if (auctionId == null) {
      this.toastrService.error('Invalid query params.', 'Error');

      return;
    }

    this.user = this.authService.user;

    this.bidsDataTableOptions = this.auctionsService.getAuctionBidsDataTableOptions();

    this.reloadAuctionDetails(auctionId);

    this.reloadCancelationForm();

    this.reloadComplaintForm();
  }

  get amount() {
    return this.setBidForm.get('amount');
  }

  reloadAuctionDetails(auctionId: string) {
    this.auctionsService.getAuctionDetailsById(auctionId).subscribe(
      (response) => {
        this.auctionDetails = response;

        this.reloadSetBidForm();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  reloadSetBidForm() {
    this.setBidForm = new FormGroup({
      amount: new FormControl(this.auctionDetails.currentBid + 1, [Validators.required, Validators.min(this.auctionDetails.currentBid + 1), Validators.max(10e7)])
    });
  }

  reloadCancelationForm() {
    this.cancelationForm = new FormGroup({
      auctionId: new FormControl(),
      cancelationReason: new FormControl(null, [Validators.required])
    });
  }

  reloadComplaintForm() {
    this.complaintOnAuctionForm = new FormGroup({
      complaintText: new FormControl(null, [Validators.required])
    });
  }

  userCannotPlaceBid() {
    return this.user &&
      (this.user.role == 'Admin' ||
        this.user.role == 'TechnicalSupportSpecialist' ||
        this.auctionDetails.status == 'Canceled' ||
        this.user.userId == this.auctionDetails.auctionistId);
  }

  auctionActionsAreEnabled() {
    return this.user != null &&
      (this.user.role == 'TechnicalSupportSpecialist' ||
        (this.user.role == 'Customer' && this.auctionDetails.status != 'Canceled')
      );
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  openSetBidOrSignInModal(setBidModal: TemplateRef<any>, signInModal: TemplateRef<any>) {
    if (this.user == null) {
      this.open(signInModal);

      return;
    }

    this.open(setBidModal);
  }

  setBid(modal: any) {
    if (!this.setBidForm.valid) {
      return;
    }

    modal.close();

    var amount = this.setBidForm.value.amount;

    this.reloadSetBidForm();

    const bid = {
      auctionId: this.auctionDetails.id,
      amount: amount
    } as SetBidModel;

    this.auctionsService.setBidOnAuction(bid).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadAuctionDetails(this.auctionDetails.id);

        this.bidsDataTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  complaintOnAuction(modal: any) {
    if (!this.complaintOnAuctionForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintOnAuctionForm.value.complaintText;

    this.reloadComplaintForm();

    const complaint = {
      accusedUserId: this.auctionDetails.auctionistId,
      auctionId: this.auctionDetails.id,
      complaintType: ComplaintTypeEnum.ComplaintOnAuctionContent,
      complaintText: complaintText
    } as SetComplaintModel;

    this.complaintsService.setComplaint(complaint).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  openCancelModal(cancelAuctionModal: TemplateRef<any>, cancelOwnAuctionModal: TemplateRef<any>) {
    if (this.user.role == 'TechnicalSupportSpecialist') {
      this.open(cancelAuctionModal);

      return;
    }

    this.open(cancelOwnAuctionModal)
  }

  cancelAuction(modal: any) {
    if (!this.cancelationForm.valid) {
      return;
    }

    modal.close();

    var cancelationReason = this.cancelationForm.value.cancelationReason;

    this.reloadCancelationForm();

    const model = {
      auctionId: this.auctionDetails.id,
      cancelationReason: cancelationReason
    } as CancelAuctionModel;

    this.auctionsService.cancelAuction(model).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadAuctionDetails(this.auctionDetails.id);

        await this.bidsDataTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  cancelOwnAuction(modal: any) {
    modal.close();

    this.auctionsService.cancelOwnAuction(this.auctionDetails.id).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadAuctionDetails(this.auctionDetails.id);

        await this.bidsDataTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  recoverAuction(modal: any) {
    modal.close();

    this.auctionsService.recoverAuction(this.auctionDetails.id).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadAuctionDetails(this.auctionDetails.id);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  cancelLastBidOfAuction(modal: any) {
    modal.close();

    this.auctionsService.cancelLastBidOfAuction(this.auctionDetails.id).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        this.reloadAuctionDetails(this.auctionDetails.id);

        await this.bidsDataTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  getAuctionBidsApiUrl() {
    return this.auctionsService.getAuctionBidsApiUrl(this.auctionDetails.id);
  }
}
