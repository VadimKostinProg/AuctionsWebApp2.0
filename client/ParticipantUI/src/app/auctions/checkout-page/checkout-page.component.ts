// src/app/checkout-page/checkout-page.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentsService } from '../../services/payments.service';
import { AuctionsService } from '../../services/auctions.Service';
import { Auction } from '../../models/auctions/Auction';
import { AuctionStatusEnum } from '../../models/auctions/auctionStatusEnum';
import { ServiceMessage } from '../../models/shared/serviceResult';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-checkout-page',
  templateUrl: './checkout-page.component.html',
  styleUrls: ['./checkout-page.component.scss'],
  standalone: false
})
export class CheckoutPageComponent implements OnInit {
  auctionId: number | null = null;
  auction: Auction | null = null;
  loading: boolean = true;
  processingPayment: boolean = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly paymentsService: PaymentsService,
    private readonly auctionsService: AuctionsService,
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService
  ) { }

  ngOnInit(): void {
    const auctionId = this.route.snapshot.paramMap.get('auctionId');


    if (!auctionId) {
      this.error = 'Invalid route params.';
      this.loading = false;
      return;
    }

    this.auctionId = parseInt(auctionId);

    this.loadAuctionDetails();
  }

  async loadAuctionDetails(): Promise<void> {
    try {
      this.loading = true;
      this.auctionsService.getAuctionDetailsById(this.auctionId!).subscribe({
        next: result => {
          this.auction = result.data!;

          if (this.auction.status !== AuctionStatusEnum.Finished) {
            this.error = 'Auction is not finished yet to performe payment.';
          }

          if (this.auction.status === AuctionStatusEnum.Finished) {
            if (this.auction.type === 'Dutch Auction' &&
              this.auction.auctioneer.userId !== this.authService.user.userId) {
              this.error = 'You are not allowed to perform payment of this auction.';
            }
            else if (this.auction.type !== 'Dutch Auction' &&
              this.auction.winner!.userId !== this.authService.user.userId) {
              this.error = 'You are not allowed to perform payment of this auction.';
            }
          }

          this.loading = false;
        },
        error: err => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
          this.loading = false;
        }
      });
    } catch (e: any) {
      this.error = 'An unexpected error occurred while loading auction details.';
      this.loading = false;
      console.error('Error:', e);
    }
  }

  async processPayment(): Promise<void> {
    if (!this.auctionId) {
      this.error = 'Auction ID is missing.';
      return;
    }
    if (this.auction?.isPaymentPerformed) {
      this.error = 'This auction has already been paid for.';
      return;
    }

    this.processingPayment = true;
    this.error = null;
    this.successMessage = null;

    this.paymentsService.chargeAuctionWin(this.auctionId).subscribe({
      next: (response: ServiceMessage) => {
        this.toastrService.success(response.message!, 'Success');
        this.processingPayment = false;

        this.router.navigate(['/']);
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
        this.processingPayment = false;
      }
    });
  }
}
