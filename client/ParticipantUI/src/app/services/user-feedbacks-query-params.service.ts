import { Injectable } from "@angular/core";
import { QueryParamsService } from "./query-params.service";
import { PaginationModel } from "../models/shared/paginationModel";

@Injectable({
  providedIn: 'root'
})
export class UserFeedbacksQueryParamsService extends QueryParamsService {
  override async getPaginationParams(): Promise<PaginationModel> {
    const pageNumber = await super.getQueryParam('feedbacksPageNumber') ?? undefined;
    const pageSize = await super.getQueryParam('feedbacksPageSize') ?? undefined;

    return {
      pageNumber: pageNumber,
      pageSize: pageSize
    } as PaginationModel;
  }

  override async setPaginationParams(pagination: PaginationModel) {
    await super.setQueryParams([
      { key: 'feedbacksPageNumber', value: pagination.pageNumber },
      { key: 'feedbacksPageSize', value: pagination.pageSize },
    ]);
  }

  override async clearPaginationParams() {
    await super.clearQueryParams(['feedbacksPageNumber', 'feedbacksPageSize']);
  }
}
