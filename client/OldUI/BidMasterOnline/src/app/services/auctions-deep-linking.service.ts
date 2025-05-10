import { Injectable } from '@angular/core';
import { DeepLinkingService } from './deep-linking.service';

@Injectable({
  providedIn: 'root'
})
export class AuctionsDeepLinkingService extends DeepLinkingService {

  getAuctionId() {
    return super.getQueryParam('auctionId');
  }

  async setAuctionId(auctionId: string) {
    await super.setQueryParam('auctionId', auctionId);
  }

  async clearAuctionId() {
    await super.clearQueryParam('auctionId');
  }

  getSearchTerm() {
    return super.getQueryParam('searchTerm');
  }

  async setSearchTerm(value: string) {
    await super.setQueryParam('searchTerm', value);
  }

  async clearSearchTerm() {
    await super.clearQueryParam('searchTerm');
  }

  getCategoryId() {
    return super.getQueryParam('categoryId');
  }

  async setCategoryId(value: string) {
    await super.setQueryParam('categoryId', value);
  }

  async clearCategoryId() {
    await super.clearQueryParam('categoryId');
  }

  getStartPriceDiapason() {
    return {
      min: super.getQueryParam('minStartPrice'),
      max: super.getQueryParam('maxStartPrice'),
    };
  }

  async setStartPriceDiapason(min: number, max: number) {
    await super.setQueryParams([
      { key: 'minStartPrice', value: min },
      { key: 'maxStartPrice', value: max },
    ]);
  }

  async clearStartPriceDiapason() {
    await super.clearQueryParams(['minStartPrice', 'maxStartPrice']);
  }

  getCurrentBidDiapason() {
    return {
      min: super.getQueryParam('minCurrentBid'),
      max: super.getQueryParam('maxCurrentBid'),
    };
  }

  async setCurrentBidDiapason(min: number, max: number) {
    await super.setQueryParams([
      { key: 'minCurrentBid', value: min },
      { key: 'maxCurrentBid', value: max },
    ]);
  }

  async clearCurrentBidDiapason() {
    await super.clearQueryParams(['minCurrentBid', 'maxCurrentBid']);
  }

  getStatus() {
    return super.getQueryParam('status');
  }

  async setStatus(value: string) {
    await super.setQueryParam('status', value);
  }

  async clearStatus() {
    await super.clearQueryParam('status');
  }
}