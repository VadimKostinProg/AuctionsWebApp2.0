import { Injectable } from '@angular/core';
import { ActivatedRoute, NavigationExtras, Router } from '@angular/router';
import { SortDirectionEnum } from '../models/sortDirectionEnum';
import { PaginationModel } from '../models/paginationModel';
import { SortingModel } from '../models/sortingModel';
import { QueryStringParam } from '../models/queryStringParam';

@Injectable({
  providedIn: 'root'
})
export class DeepLinkingService {

  constructor(protected readonly route: ActivatedRoute,
    protected readonly router: Router) {

  }

  getQueryParam(key: string) {
    return this.route.snapshot.queryParams[key] || null;
  }

  getAllQueryParams() {
    return this.route.snapshot.queryParams;
  }

  async setQueryParam(key: string, value: any) {
    const params = { ...this.route.snapshot.queryParams };
    params[key] = value;

    const navigationExtras: NavigationExtras = {
      queryParams: params,
      queryParamsHandling: 'merge',
    };

    await this.router.navigate([], navigationExtras);
  }

  async setQueryParams(params: QueryStringParam[]) {
    const existantParams = { ...this.route.snapshot.queryParams };

    for (let param of params) {
      existantParams[param.key] = param.value;
    }

    const navigationExtras: NavigationExtras = {
      queryParams: existantParams,
      queryParamsHandling: 'merge',
    };

    await this.router.navigate([], navigationExtras);
  }

  async clearQueryParam(key: string) {
    const params = { ...this.route.snapshot.queryParams };
    params[key] = null;

    const navigationExtras: NavigationExtras = {
      queryParams: params,
      queryParamsHandling: 'merge',
    };

    await this.router.navigate([], navigationExtras);
  }

  async clearQueryParams(keys: string[]) {
    const params = { ...this.route.snapshot.queryParams };
    for (let key of keys) {
      params[key] = null;
    }

    const navigationExtras: NavigationExtras = {
      queryParams: params,
      queryParamsHandling: 'merge',
    };

    await this.router.navigate([], navigationExtras);
  }

  async clearAllQueryParams() {
    const navigationExtras: NavigationExtras = {
      queryParams: [],
      queryParamsHandling: 'merge',
    };

    await this.router.navigate([], navigationExtras);
  }

  async getSortingParams(): Promise<SortingModel> {
    const sortField = await this.getQueryParam('sortField');
    const sortDirection = await this.getQueryParam('sortDirection') as SortDirectionEnum;

    return {
      sortField: sortField,
      sortDirection: sortDirection
    } as SortingModel;
  }

  async setSortingParams(sorting: SortingModel) {
    await this.setQueryParams([
      { key: 'sortField', value: sorting.sortField },
      { key: 'sortDirection', value: SortDirectionEnum[sorting.sortDirection] },
    ]);
  }

  async clearSortingParams() {
    await this.clearQueryParams(['sortField', 'sortDirection']);
  }

  async getPaginationParams(): Promise<PaginationModel> {
    const pageNumber = await this.getQueryParam('pageNumber');
    const pageSize = await this.getQueryParam('pageSize');

    return {
      pageNumber: pageNumber,
      pageSize: pageSize
    } as PaginationModel;
  }

  async setPaginationParams(pagination: PaginationModel) {
    await this.setQueryParams([
      { key: 'pageNumber', value: pagination.pageNumber },
      { key: 'pageSize', value: pagination.pageSize },
    ]);
  }

  async clearPaginationParams() {
    await this.clearQueryParams(['pageNumber', 'pageSize']);
  }
}
