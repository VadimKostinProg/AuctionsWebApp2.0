import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { AuctionCategoriesService } from "../../services/auciton-categories.service";
import { QueryParamsService } from "../../services/query-params.service";
import { AuctionTypesService } from "../../services/auction-types.service";
import { AuctionType } from "../../models/auctions/auctionType";
import { AuctionCategory } from "../../models/auction-categories/auctionCategory";
import { AuctionStatusEnum } from "../../models/auctions/auctionStatusEnum";
import { forkJoin } from "rxjs";

@Component({
  selector: 'app-auction-filters',
  templateUrl: './auction-filters.component.html',
  standalone: false,
})
export class AuctionFiltersComponent implements OnInit {
  auctionId: number | null = null;
  searchTerm: string | null = null;
  startTime: string | null = null;
  finishTime: string | null = null;
  selectedStatus: AuctionStatusEnum | null = null;
  selectedCategory: number | null = null;
  selectedType: number | null = null;

  AuctionStatusEnum = AuctionStatusEnum;

  statusOptions = Object.keys(AuctionStatusEnum)
    .filter(key => isNaN(Number(key)))
    .map(key => ({
      key: key,
      value: AuctionStatusEnum[key as keyof typeof AuctionStatusEnum]
    }));

  categories: AuctionCategory[] = [];
  types: AuctionType[] = [];

  resourcesInitialized = false;

  @Output() onFiltersChanged = new EventEmitter<void>();

  constructor(
    private readonly queryParamsService: QueryParamsService,
    private readonly auctionCategoriesService: AuctionCategoriesService,
    private readonly auctionTypesService: AuctionTypesService
  ) { }

  async ngOnInit(): Promise<void> {
    forkJoin([
      this.auctionCategoriesService.getAllAuctionCategories(),
      this.auctionTypesService.getAllAuctionTypes()
    ])
      .subscribe({
        next: ([categoriesResult, typesResult]) => {
          this.categories = categoriesResult.data!;
          this.types = typesResult.data!;

          this.resourcesInitialized = true;
        }
      });

    const auctionIdParam = this.queryParamsService.getQueryParam('auctionId') || null;
    if (auctionIdParam) {
      this.auctionId = parseInt(auctionIdParam);
    } else {
      this.auctionId = null;
    }

    this.searchTerm = this.queryParamsService.getQueryParam('searchTerm') || null;
    this.startTime = this.queryParamsService.getQueryParam('startTime') || null;
    this.finishTime = this.queryParamsService.getQueryParam('finishTime') || null;

    const statusParam = this.queryParamsService.getQueryParam('status');
    if (statusParam !== null && statusParam !== undefined) {
      this.selectedStatus = parseInt(statusParam, 10) as AuctionStatusEnum;
    } else {
      this.selectedStatus = null;
    }

    const categoryParam = this.queryParamsService.getQueryParam('categoryId');
    if (categoryParam) {
      this.selectedCategory = parseInt(categoryParam, 10);
    } else {
      this.selectedCategory = null;
    }

    const typeParam = this.queryParamsService.getQueryParam('typeId');
    if (typeParam !== null && typeParam !== undefined) {
      this.selectedType = parseInt(typeParam, 10);
    } else {
      this.selectedType = null;
    }
  }

  async onSearchClicked(): Promise<void> {
    await this.updateQueryParams();
    this.onFiltersChanged.emit();
  }

  async updateQueryParams(): Promise<void> {
    await this.queryParamsService.setQueryParam('auctionId', this.auctionId);
    await this.queryParamsService.setQueryParam('searchTerm', this.searchTerm);
    await this.queryParamsService.setQueryParam('startTime', this.startTime);
    await this.queryParamsService.setQueryParam('finishTime', this.finishTime);
    await this.queryParamsService.setQueryParam('status', this.selectedStatus);
    await this.queryParamsService.setQueryParam('categoryId', this.selectedCategory);
    await this.queryParamsService.setQueryParam('typeId', this.selectedType);
  }

  async clearFilters(): Promise<void> {
    this.auctionId = null;
    this.searchTerm = null;
    this.startTime = null;
    this.finishTime = null;
    this.selectedStatus = null;
    this.selectedCategory = null;
    this.selectedType = null;
    await this.onSearchClicked();
  }

  getAuctionStatusText(statusValue: AuctionStatusEnum): string {
    switch (statusValue) {
      case AuctionStatusEnum.Pending: return 'Pending';
      case AuctionStatusEnum.Active: return 'Active';
      case AuctionStatusEnum.CancelledByAuctionist: return 'Cancelled by Auctionist';
      case AuctionStatusEnum.CancelledByModerator: return 'Cancelled by Moderator';
      case AuctionStatusEnum.Finished: return 'Finished';
      default: return 'Unknown';
    }
  }
}
