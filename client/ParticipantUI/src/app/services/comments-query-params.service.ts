import { Injectable } from "@angular/core";
import { QueryParamsService } from "./query-params.service";
import { PaginationModel } from "../models/shared/paginationModel";

@Injectable({
  providedIn: 'root'
})
export class CommentsQueryParamsService extends QueryParamsService {
  override async getPaginationParams(): Promise<PaginationModel> {
    const pageNumber = await super.getQueryParam('commentsPageNumber') ?? undefined;
    const pageSize = await super.getQueryParam('commentsPageSize') ?? undefined;

    return {
      pageNumber: pageNumber,
      pageSize: pageSize
    } as PaginationModel;
  }

  override async setPaginationParams(pagination: PaginationModel) {
    await super.setQueryParams([
      { key: 'commentsPageNumber', value: pagination.pageNumber },
      { key: 'commentsPageSize', value: pagination.pageSize },
    ]);
  }

  override async clearPaginationParams() {
    await super.clearQueryParams(['commentsPageNumber', 'commentsPageSize']);
  }
}
