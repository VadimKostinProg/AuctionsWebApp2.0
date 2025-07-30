import { AfterViewInit, Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { QueryParamsService } from '../../services/query-params.service';
import { UserBasic } from '../../models/users/userBasic';
import { Auction } from '../../models/auctions/Auction';
import { AuctionsService } from '../../services/auctions.service';
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
import { UserProfileService } from '../../services/user-profiles.service';

@Component({
  selector: 'app-auction-details',
  templateUrl: './auction-details.component.html',
  styleUrls: ['./auction-details.component.scss'],
  standalone: false
})
export class AuctionDetailsComponent implements OnInit {

  @ViewChild(DataTableComponent) bidsDataTable!: DataTableComponent;

  @ViewChild('setBidModal') setBidModal: any;
  @ViewChild('userBlockedModal') userBlockedModal: any;
  @ViewChild('notAttachedPaymentModal') notAttachedPaymentModal: any;

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

  isPaymentMethodAttached: boolean = false;

  timeLeft: string = 'Calculating...';
  private countdownInterval: any;

  constructor(private readonly toastrService: ToastrService,
    private readonly auctionsService: AuctionsService,
    private readonly authService: AuthService,
    private readonly modalService: NgbModal,
    private readonly complaintsService: ComplaintsService,
    private readonly bidsService: BidsService,
    private readonly route: ActivatedRoute,
    private readonly userProfilesService: UserProfileService) {

  }

  async ngOnInit() {
    this.user = this.authService.user;

    this.isPaymentMethodAttached = await this.userProfilesService.isPaymentMethodAttached();

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

        if (this.auctionDetails.status === AuctionStatusEnum.Active) {
          this.startCountdown();
        }

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
        this.auctionDetails.status === AuctionStatusEnum.CancelledByAuctioneer ||
        this.user.userId == this.auctionDetails.auctioneer!.userId);
  }

  get auctionActionsAreAvailable() {
    if (!this.user || !this.auctionDetails)
      return false;

    const auction = this.auctionDetails;
    const currentUser = this.user;

    const isNotCancelled = auction.status !== AuctionStatusEnum.CancelledByModerator &&
      auction.status !== AuctionStatusEnum.CancelledByAuctioneer;

    if (!isNotCancelled) {
      return false;
    }

    let isAnyActionAvailable = false;

    const canPerformDelivery = auction.status === AuctionStatusEnum.Finished &&
      !auction.isDeliveryPerformed &&
      (
        (auction.type === 'Dutch Auction' && auction.winner?.userId === currentUser.userId) ||
        (auction.type !== 'Dutch Auction' && auction.auctioneer?.userId === currentUser.userId)
      );
    if (canPerformDelivery) {
      isAnyActionAvailable = true;
    }

    const canGoToPayment = auction.status === AuctionStatusEnum.Finished &&
      !auction.isPaymentPerformed &&
      (
        (auction.type === 'Dutch Auction' && auction.auctioneer?.userId === currentUser.userId) ||
        (auction.type !== 'Dutch Auction' && auction.winner?.userId === currentUser.userId)
      );
    if (canGoToPayment) {
      isAnyActionAvailable = true;
    }

    const canComplain = auction.auctioneer?.userId !== currentUser.userId;
    if (canComplain) {
      isAnyActionAvailable = true;
    }

    const canCancel = auction.status === AuctionStatusEnum.Active && auction.auctioneer?.userId === currentUser.userId;
    if (canCancel) {
      isAnyActionAvailable = true;
    }

    return isAnyActionAvailable;
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  handleBidButtonClick(): void {
    if (!this.isPaymentMethodAttached) {
      this.open(this.notAttachedPaymentModal);
    } else if (this.userStatus === UserStatusEnum.Blocked) {
      this.open(this.userBlockedModal);
    } else {
      this.open(this.setBidModal);
    }
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
      accusedUserId: this.auctionDetails!.auctioneer!.userId,
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
      cancellationReason: cancelationReason
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

  private startCountdown(): void {
    this.countdownInterval = setInterval(() => {
      this.updateTimeLeft();
    }, 1000);
  }

  private stopCountdown(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }

  private updateTimeLeft(): void {
    if (!this.auctionDetails || !this.auctionDetails.finishTime) {
      this.timeLeft = 'N/A';
      this.stopCountdown();
      return;
    }

    const finishTime = new Date(this.auctionDetails.finishTime).getTime();
    const now = new Date().getTime();
    const distance = finishTime - now;

    if (distance < 0) {
      this.timeLeft = 'Auction Finished!';
      this.stopCountdown();
      if (this.auctionDetails.status === AuctionStatusEnum.Active) {
        this.auctionDetails.status = AuctionStatusEnum.Finished;
      }
      return;
    }

    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

    let parts: string[] = [];
    if (days > 0) parts.push(`${days}d`);
    if (hours > 0 || days > 0) parts.push(`${hours}h`);
    if (minutes > 0 || hours > 0 || days > 0) parts.push(`${minutes}m`);
    parts.push(`${seconds}s`);

    this.timeLeft = parts.join(' ');
  }
}
