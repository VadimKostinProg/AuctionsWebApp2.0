import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { AuctionSpecifications } from "../../models/auctions/auctionSpecifications";
import { AuctionCategory, AuctionType } from "../../models/auctions/Auction";
import { AuctionStatusEnum } from "../../models/auctions/auctionStatusEnum";
import { AuctionsQueryParamsService } from "../../services/auctions-query-params.service";
import { AuctionResourcesService } from "../../services/auction-resources.service";
import { forkJoin } from "rxjs";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'app-auction-filters',
  templateUrl: './auction-filters.component.html',
  standalone: false
})
export class AuctionFiltersComponent implements OnInit {

  @Output() onFilterChanged = new EventEmitter<AuctionSpecifications>();

  specifications = new AuctionSpecifications();

  defaultSliderMin = 100;
  defaultSliderMax = 9 * 10e5;

  minStartPrice: number = this.defaultSliderMin;
  maxStartPrice: number = this.defaultSliderMax;

  minCurrentPrice: number = this.defaultSliderMin;
  maxCurrentPrice: number = this.defaultSliderMax;

  startPriceChanged = false;
  currentPriceChanged = false;

  categories: AuctionCategory[] | undefined;
  types: AuctionType[] | undefined;

  allResourcesInitialized: boolean = false;

  AuctionStatusEnum = AuctionStatusEnum;

  constructor(private readonly auctionsQueryParamsService: AuctionsQueryParamsService,
    private readonly auctionResourcesService: AuctionResourcesService,
    private readonly toastrService: ToastrService) { }

  async ngOnInit(): Promise<void> {
    this.specifications = await this.auctionsQueryParamsService.getAuctionSpecifications();
    this.specifications.minStartPrice = this.defaultSliderMin;
    this.specifications.maxStartPrice = this.defaultSliderMax;
    this.specifications.minCurrentPrice = this.defaultSliderMin;
    this.specifications.maxCurrentPrice = this.defaultSliderMax;

    forkJoin([
      this.auctionResourcesService.getAuctionCategories(),
      this.auctionResourcesService.getAuctionTypes()
    ])
      .subscribe({
        next: ([categoriesResult, typesResult]) => {
          this.categories = categoriesResult.data!;
          this.types = typesResult.data!;

          this.allResourcesInitialized = true;
        },
        error: (err) => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
        }
      });
  }

  async onSearchPressed(event: string) {
    this.specifications.searchTerm = event;

    this.onFilterChanged.emit(this.specifications);
  }

  async onCategoryChanged(category: any) {
    const value = category.target.value;

    if (this.specifications.categoryId == value) {
      this.specifications.categoryId = null;
    } else {
      this.specifications.categoryId = value;
    }

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async onTypeChanged(type: any) {
    const value = type.target.value;

    if (this.specifications.typeId == value) {
      this.specifications.typeId = null;
    } else {
      this.specifications.typeId = value;
    }

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async onStartPriceFilterChange() {
    this.startPriceChanged = true;

    this.specifications.minStartPrice = this.minStartPrice;
    this.specifications.maxStartPrice = this.maxStartPrice;

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async resetStartPrice() {
    this.minStartPrice = this.defaultSliderMin;
    this.maxStartPrice = this.defaultSliderMax;

    this.specifications.minStartPrice = null;
    this.specifications.maxStartPrice = null;

    this.startPriceChanged = false;

    this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async onCurrentPriceFilterChange() {
    this.currentPriceChanged = true;

    this.specifications.minCurrentPrice = this.minCurrentPrice;
    this.specifications.maxCurrentPrice = this.maxCurrentPrice;

    this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async resetCurrentPrice() {
    this.minCurrentPrice = this.defaultSliderMin;
    this.maxCurrentPrice = this.defaultSliderMax;

    this.specifications.minCurrentPrice = null;
    this.specifications.maxCurrentPrice = null;

    this.currentPriceChanged = false;

    this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async onStatusChanged(status: any) {
    const value = status.target.value;

    if (this.specifications.auctionStatus == value) {
      this.specifications.auctionStatus = null;
    } else {
      this.specifications.auctionStatus = AuctionStatusEnum[value as keyof typeof AuctionStatusEnum];
    }

    this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }
}
