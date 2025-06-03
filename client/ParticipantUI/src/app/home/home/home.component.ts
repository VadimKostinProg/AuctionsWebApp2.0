import { Component, OnInit } from '@angular/core';
import { AuctionsService } from '../../services/auctions.Service';
import { AuctionBasic } from '../../models/auctions/AuctionBasic';
import { forkJoin } from 'rxjs';
import { AuctionSpecifications } from '../../models/auctions/auctionSpecifications';
import { ToastrService } from 'ngx-toastr';
import { AuctionsQueryParamsService } from '../../services/auctions-query-params.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: false
})
export class HomeComponent implements OnInit {
  searchedAuctions: AuctionBasic[] = [];

  popularAuctions: AuctionBasic[] | undefined;
  finishingAuctions: AuctionBasic[] | undefined;

  searchMode: boolean = false;

  specifications: AuctionSpecifications | undefined;

  currentPage: number = 0;
  pageSize: number = 10;
  hasNext: boolean = false;

  constructor(private readonly auctionsService: AuctionsService,
    private readonly toastrService: ToastrService,
    private readonly auctionsQueryParamsService: AuctionsQueryParamsService) { }

  async ngOnInit(): Promise<void> {
    await this.checkMode();

    if (this.searchMode)
      this.fetchSearchedAuctions();
    else
      this.fetchPopularAndFinishingAuctions();
  }

  fetchSearchedAuctions() {
    this.auctionsService.searchAuctions(this.specifications!, ++this.currentPage, this.pageSize).subscribe({
      next: result => {
        this.searchedAuctions = [...this.searchedAuctions, ...result.data!.items];

        this.currentPage = result.data!.pagination.currentPage;
        this.hasNext = result.data!.pagination.hasNext;
      },
      error: err => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  fetchPopularAndFinishingAuctions() {
    forkJoin([
      this.auctionsService.getPopularAuctions(),
      this.auctionsService.getFinishingAuctions(),
    ])
      .subscribe({
        next: ([popularResult, finishingResult]) => {
          this.popularAuctions = popularResult.data?.items;
          this.finishingAuctions = finishingResult.data?.items;
        },
        error: err => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
        }
      });
  }

  onFilterChanged(event: AuctionSpecifications) {
    this.specifications = event;

    this.searchedAuctions = [];
    this.searchMode = true;
    this.currentPage = 0;

    this.fetchSearchedAuctions();
  }

  async checkMode(): Promise<void> {
    this.specifications = await this.auctionsQueryParamsService.getAuctionSpecifications();

    console.log(this.specifications);

    this.searchMode = this.checkIfSpecificationsAreNotEmpty(this.specifications);
  }

  checkIfSpecificationsAreNotEmpty(specs: AuctionSpecifications) {
    return specs.categoryId !== null
      || specs.typeId !== null
      || (specs.maxStartPrice !== null && specs.minStartPrice !== null)
      || (specs.maxCurrentPrice !== null && specs.minCurrentPrice !== null)
      || specs.searchTerm !== null
      || specs.auctionStatus !== null;
  }
}
