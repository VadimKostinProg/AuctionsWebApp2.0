import { Component, OnInit } from '@angular/core';
import { AuctionsService } from '../../services/auctions.Service';
import { AuctionBasic } from '../../models/auctions/AuctionBasic';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: false
})
export class HomeComponent implements OnInit {
  popularAuctions: AuctionBasic[] | undefined;

  constructor(private auctionsService: AuctionsService) { }

  ngOnInit(): void {
    this.auctionsService.getPopularAuctions().subscribe(
      (result) => {
        this.popularAuctions = result;
      }
    );
  }
}
