import { Component, OnInit } from '@angular/core';
import { AuctionsService } from '../../services/auctions.Service';
import { AuctionBasic } from '../../models/auctions/AuctionBasic';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: false
})
export class HomeComponent implements OnInit {
  popularAuctions: AuctionBasic[] | undefined;
  finishingAuctions: AuctionBasic[] | undefined;

  constructor(private auctionsService: AuctionsService) { }

  ngOnInit(): void {
    this.fetchPopularAndFinishingAuctions();
  }

  fetchPopularAndFinishingAuctions() {
    forkJoin([
      this.auctionsService.getPopularAuctions(),
      this.auctionsService.getFinishingAuctions(),
    ])
      .subscribe(
        ([popularResult, finishingResult]) => {
          this.popularAuctions = popularResult.data?.items;
          this.finishingAuctions = finishingResult.data?.items;
        }
      );
  }

  onSearchPressed() {

  }
}
