export class PaginatedList<T> {
  public items!: T[];
  public pagination!: ListPagination;
}

export class ListPagination {
  public pageSize!: number;
  public currentPage!: number;
  public totalPages!: number;
  public totalRecords!: number;
  public hasNext!: boolean;
}
