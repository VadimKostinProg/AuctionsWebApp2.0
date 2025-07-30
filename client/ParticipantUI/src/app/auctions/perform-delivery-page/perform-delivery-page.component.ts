import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Auction } from '../../models/auctions/Auction';
import { AuctionsService } from '../../services/auctions.service';
import { AuctionStatusEnum } from '../../models/auctions/auctionStatusEnum';
import { ToastrService } from 'ngx-toastr';
import { ServiceMessage } from '../../models/shared/serviceResult';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-perform-delivery-page',
  templateUrl: './perform-delivery-page.component.html',
  styleUrls: ['./perform-delivery-page.component.scss'],
  standalone: false
})
export class PerformDeliveryPageComponent implements OnInit {
  auctionId: number | null = null;
  auction: Auction | null = null;
  waybillNumber: string = '';

  loadingAuction: boolean = true;
  submittingWaybill: boolean = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly auctionService: AuctionsService,
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService
  ) { }

  ngOnInit(): void {
    const auctionId = this.route.snapshot.paramMap.get('auctionId');

    if (!auctionId) {
      this.error = 'Invalid route params.';
      this.loadingAuction = false;
      return;
    }

    this.auctionId = parseInt(auctionId);

    this.loadAuctionDetails();
  }

  loadAuctionDetails(): void {
    this.loadingAuction = true;
    this.auctionService.getAuctionDetailsById(this.auctionId!).subscribe({
      next: result => {
        this.auction = result.data!;
        this.loadingAuction = false;

        if (this.auction.status !== AuctionStatusEnum.Finished) {
          this.error = 'Waybill can only be added for finished and paid auctions.';
        }
        else {
          if (this.auction.type === 'Dutch Auction' &&
            this.auction.winner!.userId !== this.authService.user.userId) {
            this.error = 'You are not allowed to perform delivery of this auction.';
          }
          else if (this.auction.type !== 'Dutch Auction' &&
            this.auction.auctioneer!.userId !== this.authService.user.userId) {
            this.error = 'You are not allowed to perform delivery of this auction.';
          }
        }
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
        this.loadingAuction = false;
      }
    });
  }

  submitWaybill(): void {
    if (!this.auctionId) {
      this.error = 'Auction ID is missing.';
      return;
    }
    if (!this.waybillNumber.trim()) {
      this.error = 'Waybill number cannot be empty.';
      return;
    }

    this.submittingWaybill = true;
    this.error = null;
    this.successMessage = null;

    this.auctionService.setDeliveryWaybill(this.auctionId, this.waybillNumber).subscribe({
      next: (response: ServiceMessage) => {
        this.toastrService.success(response.message!, 'Success');
        this.submittingWaybill = false;

        this.router.navigate(['/']);
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
        this.submittingWaybill = false;
      }
    });
  }
}
