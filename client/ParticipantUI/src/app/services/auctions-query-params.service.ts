import { Injectable } from "@angular/core";
import { QueryParamsService } from "./query-params.service";
import { AuctionSpecifications } from "../models/auctions/auctionSpecifications";
import { AuctionStatusEnum } from "../models/auctions/auctionStatusEnum";
import { SortDirectionEnum } from "../models/sortDirectionEnum";

@Injectable({
  providedIn: 'root'
})
export class AuctionsQueryParamsService extends QueryParamsService {
  async setAuctionSpecifications(specs: AuctionSpecifications): Promise<void> {
    await super.setQueryParams([
      { key: 'searchTerm', value: specs.searchTerm },
      { key: 'categoryId', value: specs.categoryId },
      { key: 'typeId', value: specs.typeId },
      { key: 'minStartPrice', value: specs.minStartPrice },
      { key: 'maxStartPrice', value: specs.maxStartPrice },
      { key: 'minCurrentPrice', value: specs.minCurrentPrice },
      { key: 'maxCurrentPrice', value: specs.maxCurrentPrice },
      { key: 'auctionStatus', value: specs.auctionStatus },
      { key: 'sortBy', value: specs.sortBy },
      { key: 'sortDirection', value: specs.sortDirection }
    ]);
  }

  async getAuctionSpecifications(): Promise<AuctionSpecifications> {
    const searchTerm = await super.getQueryParam('searchTerm');
    const categoryId = await super.getQueryParam('categoryId');
    const typeId = await super.getQueryParam('typeId');
    const minStartPrice = await super.getQueryParam('minStartPrice');
    const maxStartPrice = await super.getQueryParam('maxStartPrice');
    const minCurrentPrice = await super.getQueryParam('minCurrentPrice');
    const maxCurrentPrice = await super.getQueryParam('maxCurrentPrice');
    const auctionStatus = await super.getQueryParam('auctionStatus');
    const sortBy = await super.getQueryParam('sortBy');
    const sortDirection = await super.getQueryParam('sortDirection');

    return {
      searchTerm: searchTerm,
      categoryId: categoryId
        ? parseInt(categoryId)
        : null,
      typeId: typeId
        ? parseInt(typeId)
        : null,
      minStartPrice: minStartPrice
        ? parseInt(minStartPrice)
        : null,
      maxStartPrice: maxStartPrice
        ? parseInt(maxStartPrice)
        : null,
      minCurrentPrice: minCurrentPrice
        ? parseInt(minCurrentPrice)
        : null,
      maxCurrentPrice: maxCurrentPrice
        ? parseInt(maxCurrentPrice)
        : null,
      auctionStatus: auctionStatus
        ? AuctionStatusEnum[auctionStatus as keyof typeof AuctionStatusEnum]
        : null,
      sortBy: sortBy,
      sortDirection: sortDirection
        ? SortDirectionEnum[sortDirection as keyof typeof SortDirectionEnum]
        : null,
    } as AuctionSpecifications;
  }
}
