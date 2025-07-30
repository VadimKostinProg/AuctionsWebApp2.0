import { Component, OnInit, TemplateRef } from "@angular/core";
import { Auction } from "../../models/auctions/auction";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuctionStatusEnum } from "../../models/auctions/auctionStatusEnum";
import { ToastrService } from "ngx-toastr";
import { AuctionsService } from "../../services/auctions.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BidsService } from "../../services/bids.service";
import { ActivatedRoute } from "@angular/router";
import { CancelAuctionModel } from "../../models/auctions/cancelAuctionModel";

@Component({
  selector: 'app-auction-details',
  templateUrl: './auction-details.component.html',
  standalone: false
})
export class AuctionDetailsComponent implements OnInit {

  auctionDetails: Auction | undefined;

  bidsDataTableOptions: DataTableOptionsModel | undefined;

  cancelationForm!: FormGroup;

  AuctionStatusEnum = AuctionStatusEnum;

  showBidsList: boolean = false;

  constructor(private readonly toastrService: ToastrService,
    private readonly auctionsService: AuctionsService,
    private readonly modalService: NgbModal,
    private readonly bidsService: BidsService,
    private readonly route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const auctionId = params.get('auctionId');

      if (auctionId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.bidsDataTableOptions = this.bidsService.getDataTableOptions();

      this.reloadAuctionDetails(parseInt(auctionId));

      this.reloadCancelationForm();
    });
  }

  get auctionStatus() {
    if (!this.auctionDetails) {
      return null;
    }

    switch (this.auctionDetails.status) {
      case AuctionStatusEnum.Pending:
        return 'Pending';
      case AuctionStatusEnum.Active:
        return 'Active';
      case AuctionStatusEnum.Finished:
        return 'Finished';
      case AuctionStatusEnum.CancelledByAuctioneer:
        return 'Canceled by Auctioneer';
      case AuctionStatusEnum.CancelledByModerator:
        return 'Canceled by Moderator';
    }
  }

  get bidsDataTableApiUrl() {
    return this.bidsService.getDataTableApiUrl(this.auctionDetails!.id);
  }

  reloadAuctionDetails(auctionId: number) {
    this.showBidsList = false;

    this.auctionsService.getAuctionDetails(auctionId).subscribe({
      next: (response) => {
        this.auctionDetails = response.data!;

        this.showBidsList = true;
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  reloadCancelationForm() {
    this.cancelationForm = new FormGroup({
      auctionId: new FormControl(),
      cancelationReason: new FormControl(null, [Validators.required])
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
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
      reason: cancelationReason
    } as CancelAuctionModel;

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

  recoverAuction(modal: any) {
    modal.close();

    this.auctionsService.recoverAuction(this.auctionDetails!.id).subscribe({
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
}
