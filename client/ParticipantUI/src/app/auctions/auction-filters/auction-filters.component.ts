import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { AuctionSpecifications } from "../../models/auctions/auctionSpecifications";
import { AuctionCategory, AuctionType } from "../../models/auctions/Auction";
import { AuctionStatusEnum } from "../../models/auctions/auctionStatusEnum";
import { AuctionsQueryParamsService } from "../../services/auctions-query-params.service";
import { AuctionResourcesService } from "../../services/auction-resources.service";
import { forkJoin } from "rxjs";
import { ToastrService } from "ngx-toastr";
import { Options } from "@angular-slider/ngx-slider";

@Component({
  selector: 'app-auction-filters',
  templateUrl: './auction-filters.component.html',
  styleUrls: ['./auction-filters.component.scss'],
  standalone: false
})
export class AuctionFiltersComponent implements OnInit {

  @Output() onFilterChanged = new EventEmitter<AuctionSpecifications>();

  specifications = new AuctionSpecifications();

  defaultSliderMin = 100;
  defaultSliderMax = 900000;

  minStartPrice: number = this.defaultSliderMin;
  maxStartPrice: number = this.defaultSliderMax;
  startPriceOptions: Options = {
    floor: this.defaultSliderMin,
    ceil: this.defaultSliderMax,
    translate: (value: number): string => {
      return value + '$';
    }
  };

  minCurrentPrice: number = this.defaultSliderMin;
  maxCurrentPrice: number = this.defaultSliderMax;
  currentPriceOptions: Options = {
    floor: this.defaultSliderMin,
    ceil: this.defaultSliderMax,
    translate: (value: number): string => {
      return value + '$';
    }
  };

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

    this.minStartPrice = this.specifications.minStartPrice ?? this.defaultSliderMin;
    this.maxStartPrice = this.specifications.maxStartPrice ?? this.defaultSliderMax;
    this.startPriceChanged = this.specifications.minStartPrice != null || this.specifications.maxStartPrice != null;

    this.minCurrentPrice = this.specifications.minCurrentPrice ?? this.defaultSliderMin;
    this.maxCurrentPrice = this.specifications.maxCurrentPrice ?? this.defaultSliderMax;
    this.currentPriceChanged = this.specifications.minCurrentPrice != null || this.specifications.maxCurrentPrice != null;

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
          } else {
            this.toastrService.error('Failed to load auction resources.', 'Error');
          }
        }
      });
  }

  async onStartPriceFilterChange() {
    if (this.minStartPrice === this.defaultSliderMin && this.maxStartPrice === this.defaultSliderMax) {
      this.specifications.minStartPrice = null;
      this.specifications.maxStartPrice = null;
      this.startPriceChanged = false;
    } else {
      this.specifications.minStartPrice = this.minStartPrice;
      this.specifications.maxStartPrice = this.maxStartPrice;
      this.startPriceChanged = true;
    }
    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async resetStartPrice() {
    this.minStartPrice = this.defaultSliderMin;
    this.maxStartPrice = this.defaultSliderMax;
    this.specifications.minStartPrice = null;
    this.specifications.maxStartPrice = null;
    this.startPriceChanged = false;
    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async onSearchPressed(event: string) {
    this.specifications.searchTerm = event;

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);

    this.onFilterChanged.emit(this.specifications);
  }

  async onCategoryChanged(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value;

    if (value === 'all') {
      this.specifications.categoryId = null;
    } else {
      if (this.specifications.categoryId?.toString() === value) {
        this.specifications.categoryId = null;
      } else {
        this.specifications.categoryId = parseInt(value);
      }
    }

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async onTypeChanged(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value;

    if (value === 'all') {
      this.specifications.typeId = null;
    } else {
      if (this.specifications.typeId?.toString() === value) {
        this.specifications.typeId = null;
      } else {
        this.specifications.typeId = parseInt(value);
      }
    }

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async onStatusChanged(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value;

    if (value === 'all') {
      this.specifications.auctionStatus = null;
    } else {
      const enumValue = AuctionStatusEnum[value as keyof typeof AuctionStatusEnum];

      if (this.specifications.auctionStatus === enumValue) {
        this.specifications.auctionStatus = null;
      } else {
        this.specifications.auctionStatus = enumValue;
      }
    }

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async onCurrentPriceFilterChange() {
    if (this.minCurrentPrice === this.defaultSliderMin && this.maxCurrentPrice === this.defaultSliderMax) {
      this.specifications.minCurrentPrice = null;
      this.specifications.maxCurrentPrice = null;
      this.currentPriceChanged = false;
    } else {
      this.specifications.minCurrentPrice = this.minCurrentPrice;
      this.specifications.maxCurrentPrice = this.maxCurrentPrice;
      this.currentPriceChanged = true;
    }
    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async resetCurrentPrice() {
    this.minCurrentPrice = this.defaultSliderMin;
    this.maxCurrentPrice = this.defaultSliderMax;
    this.specifications.minCurrentPrice = null;
    this.specifications.maxCurrentPrice = null;
    this.currentPriceChanged = false;
    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
  }

  async resetAllFilters() {
    this.specifications = new AuctionSpecifications();

    this.minStartPrice = this.defaultSliderMin;
    this.maxStartPrice = this.defaultSliderMax;
    this.startPriceChanged = false;

    this.minCurrentPrice = this.defaultSliderMin;
    this.maxCurrentPrice = this.defaultSliderMax;
    this.currentPriceChanged = false;

    await this.auctionsQueryParamsService.setAuctionSpecifications(this.specifications);
    this.onFilterChanged.emit(this.specifications);
    this.toastrService.info('All filters have been reset.', 'Filters Reset');
  }
}
