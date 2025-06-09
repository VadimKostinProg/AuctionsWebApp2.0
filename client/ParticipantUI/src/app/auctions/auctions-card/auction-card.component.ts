import { Component, Input, OnInit, OnDestroy } from "@angular/core";
import { AuctionBasic } from "../../models/auctions/AuctionBasic";
import { interval, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuctionStatusEnum } from "../../models/auctions/auctionStatusEnum";

@Component({
  selector: 'auction-card',
  templateUrl: './auction-card.component.html',
  styleUrl: './auction-card.component.scss',
  standalone: false
})
export class AuctionCardComponent implements OnInit, OnDestroy {
  @Input() auction: AuctionBasic | undefined;

  AuctionStatusEnum = AuctionStatusEnum;

  timeRemaining: string = '';
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    if (this.auction && this.auction.status === AuctionStatusEnum.Active) {
      this.startCountdown();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get mainImageUrl(): string {
    if (this.auction?.imageUrls && this.auction.imageUrls.length > 0 && this.auction.imageUrls[0]) {
      return this.auction.imageUrls[0];
    }
    return '';
  }

  get showCountdown(): boolean {
    return this.auction?.status === AuctionStatusEnum.Active;
  }

  private startCountdown(): void {
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        if (this.auction && this.auction.finishTime) {
          const now = new Date().getTime();

          const finishTime = new Date(this.auction.finishTime).getTime();
          const distance = finishTime - now;

          if (distance < 0) {
            this.timeRemaining = 'Auction Finished!';
            this.destroy$.next();
          } else {
            const days = Math.floor(distance / (1000 * 60 * 60 * 24));
            const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);

            this.timeRemaining = `${days}d ${hours}h ${minutes}m ${seconds}s`;
          }
        }
      });
  }

  get showStartPrice(): boolean {
    return this.auction?.status !== AuctionStatusEnum.Pending &&
      this.auction?.status !== AuctionStatusEnum.CancelledByAuctioneer &&
      this.auction?.status !== AuctionStatusEnum.CancelledByModerator;
  }
}
