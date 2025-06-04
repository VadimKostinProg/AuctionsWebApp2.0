import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { QueryParamsService } from '../../services/query-params.service';
import { UserBasic } from '../../models/users/userBasic';
import { Auction } from '../../models/auctions/Auction';
import { AuctionsService } from '../../services/auctions.Service';
import { AuthService } from '../../services/auth.service';
import { BidsService } from '../../services/bids.service';
import { AuctionStatusEnum } from '../../models/auctions/auctionStatusEnum';
import { PostBid } from '../../models/bids/postBid';
import { PostComplaint } from '../../models/complaints/postComplaint';
import { ComplaintsService } from '../../services/complaints.service';
import { DataTableComponent } from '../../shared/data-table/data-table.component';
import { DataTableOptionsModel } from '../../models/shared/dataTableOptionsModel';
import { ComplaintTypeEnum } from '../../models/complaints/complaintTypeEnum';
import { CancelAuction } from '../../models/auctions/CancelAuction';
import { ActivatedRoute, Router } from '@angular/router';
import { UserStatusEnum } from '../../models/users/userStatusEnum';

@Component({
  selector: 'app-auction-details',
  templateUrl: './auction-details.component.html',
  standalone: false
})
export class AuctionDetailsComponent implements OnInit {

  @ViewChild(DataTableComponent)
  bidsDataTable!: DataTableComponent;

  auctionDetails: Auction | undefined;

  bidsDataTableOptions: DataTableOptionsModel | undefined;

  setBidForm!: FormGroup;

  cancelationForm!: FormGroup;

  complaintOnAuctionForm!: FormGroup;

  minBidAmount: number | undefined;
  maxBidAmount: number | undefined;

  user: UserBasic | undefined;

  AuctionStatusEnum = AuctionStatusEnum;

  UserStatusEnum = UserStatusEnum;

  constructor(private readonly toastrService: ToastrService,
    private readonly auctionsService: AuctionsService,
    private readonly authService: AuthService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService,
    private readonly bidsService: BidsService,
    private readonly route: ActivatedRoute) {

  }

  async ngOnInit() {
    this.user = this.authService.user;

    this.route.paramMap.subscribe(params => {
      const auctionId = params.get('auctionId');

      if (auctionId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.bidsDataTableOptions = this.bidsService.getAuctionBidsDataTableOptions();

      this.reloadAuctionDetails(parseInt(auctionId));

      this.reloadCancelationForm();

      this.reloadComplaintForm();
    });
  }

  get amount() {
    return this.setBidForm.get('amount');
  }

  get auctionStatus() {
    if (!this.auctionDetails) {
      return null;
    }

    switch (this.auctionDetails.status) {
      case AuctionStatusEnum.Active:
        return 'Active';
      case AuctionStatusEnum.Finished:
        return 'Finished';
      default:
        return 'Canceled';
    }
  }

  get userStatus() {
    return this.authService.userStatus;
  }

  reloadAuctionDetails(auctionId: number) {
    this.auctionsService.getAuctionDetailsById(auctionId).subscribe({
      next: (response) => {
        this.auctionDetails = response.data!;

        this.reloadSetBidForm();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  reloadSetBidForm() {
    if (this.auctionDetails!.type === 'Dutch Auction') {
      this.minBidAmount = 100;
      this.maxBidAmount = this.auctionDetails!.currentPrice - this.auctionDetails!.bidAmountInterval;

      this.setBidForm = new FormGroup({
        amount: new FormControl(this.auctionDetails!.currentPrice - this.auctionDetails!.bidAmountInterval,
          [Validators.required, Validators.min(100),
          Validators.max(this.auctionDetails!.currentPrice - this.auctionDetails!.bidAmountInterval)])
      });
    }
    else {
      this.minBidAmount = this.auctionDetails!.currentPrice + this.auctionDetails!.bidAmountInterval;
      this.maxBidAmount = 10e7;

      this.setBidForm = new FormGroup({
        amount: new FormControl(this.auctionDetails!.currentPrice + this.auctionDetails!.bidAmountInterval,
          [Validators.required, Validators.min(this.auctionDetails!.currentPrice + this.auctionDetails!.bidAmountInterval),
          Validators.max(10e7)])
      });
    }
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

  get isPlacingBidDisabled() {
    return this.user && this.auctionDetails &&
      (this.auctionDetails.status === AuctionStatusEnum.CancelledByModerator ||
        this.auctionDetails.status === AuctionStatusEnum.CancelledByAuctionist ||
        this.user.userId == this.auctionDetails.auctionist!.userId);
  }

  get auctionActionsAreAvailable() {
    return this.user && this.auctionDetails?.status !== AuctionStatusEnum.CancelledByModerator;
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  setBid(modal: any) {
    if (!this.setBidForm.valid) {
      return;
    }

    modal.close();

    var amount = this.setBidForm.value.amount;

    this.reloadSetBidForm();

    const bid = {
      auctionId: this.auctionDetails!.id,
      amount: amount
    } as PostBid;

    this.bidsService.postBid(bid).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadAuctionDetails(this.auctionDetails!.id);

        this.bidsDataTable.reloadDatatable();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  complainOnAuction(modal: any) {
    if (!this.complaintOnAuctionForm.valid) {
      return;
    }

    modal.close();

    var complaintText = this.complaintOnAuctionForm.value.complaintText;

    this.reloadComplaintForm();

    const complaint = {
      accusedUserId: this.auctionDetails!.auctionist!.userId,
      accusedAuctionId: this.auctionDetails!.id,
      type: ComplaintTypeEnum.ComplaintOnAuctionContent,
      complaintText: complaintText
    } as PostComplaint;

    this.complaintsService.postComplaint(complaint).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  cancelAuction(modal: any) {
    if (!this.cancelationForm.valid) {
      return;
    }

    modal.close();

    const cancelationReason = this.cancelationForm.value.cancelationReason;

    this.reloadCancelationForm();

    const model = {
      auctionId: this.auctionDetails!.id,
      cancelationReason: cancelationReason
    } as CancelAuction;

    this.auctionsService.cancelAuction(model).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');

        this.reloadAuctionDetails(this.auctionDetails!.id);
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  getAuctionBidsApiUrl() {
    return this.bidsService.getAuctionBidsApiUrl(this.auctionDetails!.id);
  }
}
