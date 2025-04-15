import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionsDeepLinkingService } from '../../services/auctions-deep-linking.service';
import { AuctionsService } from '../../services/auctions.service';
import { AuctionModel } from '../../models/auctionModel';
import { PaginationModel } from 'src/app/models/paginationModel';
import { ListModel } from 'src/app/models/listModel';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnInit {
  placeholder = 'Search auction...';

  auctionsList: ListModel<AuctionModel>;

  pagination: PaginationModel;

  pagesList: number[];
  pageSizeOptions = [10, 25, 50, 75];

  constructor(private readonly auctionsService: AuctionsService,
    private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService) {
  }

  async ngOnInit(): Promise<void> {
    await this.onSearchOrFilterProceed();

    this.pagination = await this.auctionsDeepLinkingService.getPaginationParams();

    if (this.pagination.pageNumber == null || this.pagination.pageSize == null) {
      this.pagination.pageNumber = 1;
      this.pagination.pageSize = 25;

      await this.auctionsDeepLinkingService.setPaginationParams(this.pagination);
    }
  }

  onSearchOrFilterProceed() {
    const queryParams = this.auctionsDeepLinkingService.getAllQueryParams();

    this.auctionsService.getAuctionsList(queryParams).subscribe(
      async (auctionsList) => {
        this.auctionsList = auctionsList;

        this.pagesList = [];

        for (let i = 1; i <= this.auctionsList.totalPages; i++) {
          this.pagesList.push(i);
        }

        if (this.pagination.pageNumber > this.auctionsList.totalPages) {
          this.pagination.pageNumber = 1;

          await this.auctionsDeepLinkingService.setPaginationParams(this.pagination);
        }
      }
    );
  }

  async decrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber - 1);
  }

  async onPageNumberChanged(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;

    await this.auctionsDeepLinkingService.setPaginationParams(this.pagination);

    this.onSearchOrFilterProceed();
  }

  async incrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber + 1);
  }

  async onPageSizeChanged(pageSize: any) {
    this.pagination.pageSize = pageSize.target.value;

    await this.auctionsDeepLinkingService.setPaginationParams(this.pagination);

    this.onSearchOrFilterProceed();
  }
}
