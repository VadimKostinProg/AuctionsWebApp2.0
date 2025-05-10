import { Component, EventEmitter, OnInit, Output, TemplateRef } from '@angular/core';
import { CategoryModel } from 'src/app/models/categoryModel';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { CategoriesService } from 'src/app/services/categories.service';

@Component({
  selector: 'auction-filters',
  templateUrl: './auction-filters.component.html'
})
export class AuctionFiltersComponent implements OnInit {

  defaultSliderMin = 2 * 10e6;
  defaultSliderMax = 8 * 10e6;

  minStartPrice = 0;
  maxStartPrice = 10e7;

  startPriceChanged = false;

  minCurrentBid = 0;
  maxCurrentBid = 10e7;

  currentBidChanged = false;

  categories: CategoryModel[];

  chosenCategoryId: string;

  chosenStatus: string;

  @Output()
  onFiltersChange = new EventEmitter<void>();

  constructor(private readonly auctionsDeeplinkingService: AuctionsDeepLinkingService,
    private readonly categoriesService: CategoriesService) {

  }

  async ngOnInit(): Promise<void> {
    this.categoriesService.getAllCategories().subscribe((categories) => {
      this.categories = categories;
    });

    this.categories = [
      {
        id: "id1",
        name: "Category1",
        description: "Description1",
      },
      {
        id: "id2",
        name: "Category2",
        description: "Description2",
      },
      {
        id: "id3",
        name: "Category3",
        description: "Description3",
      },
      {
        id: "id4",
        name: "Category4",
        description: "Description4",
      }
    ];

    const category = await this.auctionsDeeplinkingService.getCategoryId();
    const startPrice = await this.auctionsDeeplinkingService.getStartPriceDiapason();
    const currentBid = await this.auctionsDeeplinkingService.getCurrentBidDiapason();
    const status = await this.auctionsDeeplinkingService.getStatus();

    this.chosenCategoryId = category || '';

    this.minStartPrice = startPrice?.min || this.defaultSliderMin;
    this.maxStartPrice = startPrice?.max || this.defaultSliderMax;

    this.minCurrentBid = currentBid?.min || this.defaultSliderMin;
    this.maxCurrentBid = currentBid?.max || this.defaultSliderMax;

    this.chosenStatus = status || '';
  }

  async onCategoryChanged(category: any) {
    const value = category.target.value;

    if (this.chosenCategoryId == value) {
      this.chosenCategoryId = '';

      await this.auctionsDeeplinkingService.clearCategoryId();
    } else {
      this.chosenCategoryId = value;

      await this.auctionsDeeplinkingService.setCategoryId(value);
    }

    this.onFiltersChange.emit();
  }

  async onStartPriceFilterChange() {
    this.startPriceChanged = true;

    await this.auctionsDeeplinkingService.setStartPriceDiapason(this.minStartPrice, this.maxStartPrice);

    this.onFiltersChange.emit();
  }

  async clearStartPrice() {
    this.minStartPrice = this.defaultSliderMin;
    this.maxStartPrice = this.defaultSliderMax;

    this.startPriceChanged = false;

    await this.auctionsDeeplinkingService.clearStartPriceDiapason();

    this.onFiltersChange.emit();
  }

  async onCurrentBidFilterChange() {
    this.currentBidChanged = true;

    await this.auctionsDeeplinkingService.setCurrentBidDiapason(this.minCurrentBid, this.maxCurrentBid);

    this.onFiltersChange.emit();
  }

  async clearCurrentBid() {
    this.minCurrentBid = this.defaultSliderMin;
    this.maxCurrentBid = this.defaultSliderMax;

    this.currentBidChanged = false;

    await this.auctionsDeeplinkingService.clearCurrentBidDiapason();

    this.onFiltersChange.emit();
  }

  async onStatusChanged(status: any) {
    const value = status.target.value;

    if (this.chosenStatus == value) {
      this.chosenStatus = '';

      await this.auctionsDeeplinkingService.clearStatus();
    } else {
      this.chosenStatus = value;

      await this.auctionsDeeplinkingService.setStatus(value);
    }

    this.onFiltersChange.emit();
  }
}
