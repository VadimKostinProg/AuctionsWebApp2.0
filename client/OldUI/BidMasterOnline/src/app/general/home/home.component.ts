import { Component, OnInit } from '@angular/core';
import { AuctionsService } from '../../services/auctions.service';
import { AuctionModel } from '../../models/auctionModel';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  placeholder: string = 'Search auction...';

  popularAuctions: AuctionModel[] = [];
  finishingAuctions: AuctionModel[] = [];

  constructor(private readonly auctionsService: AuctionsService,
    private readonly router: Router,
    private readonly toastrService: ToastrService) {
  }

  ngOnInit() {
    this.auctionsService.getPopularAuctions().subscribe(
      (response) => {
        this.popularAuctions = response.list;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.auctionsService.getFinishingAuctions().subscribe(
      (response) => {
        this.finishingAuctions = response.list;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  async onSearchPressed() {
    await this.router.navigate(['/search'], { queryParamsHandling: 'merge' });
  }
}
