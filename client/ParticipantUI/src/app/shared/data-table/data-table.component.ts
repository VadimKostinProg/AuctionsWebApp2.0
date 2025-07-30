import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DataTableOptionsModel } from '../../models/shared/dataTableOptionsModel';
import { FormInputTypeEnum } from '../../models/shared/formInputTypeEnum';
import { PaginatedList } from '../../models/shared/paginatedList';
import { OptionalActionResultModel } from '../../models/shared/optionalActionResultModel';
import { PaginationModel } from '../../models/shared/paginationModel';
import { SortDirectionEnum } from '../../models/shared/sortDirectionEnum';
import { SortingModel } from '../../models/shared/sortingModel';
import { TableColumnSettingsModel } from '../../models/shared/tableColumnSettingsModel';
import { QueryParamsService } from '../../services/query-params.service';
import { ServiceResult } from '../../models/shared/serviceResult';

@Component({
  selector: 'data-table',
  standalone: false,
  templateUrl: './data-table.component.html'
})
export class DataTableComponent implements OnInit {

  @Input()
  options!: DataTableOptionsModel;

  @Input()
  apiUrl!: string;

  @Output()
  onCreate = new EventEmitter<void>();

  @Output()
  onEdit = new EventEmitter<void>();

  @Output()
  onDelete = new EventEmitter<string>();

  @Output()
  onAction = new EventEmitter<OptionalActionResultModel>();

  tableData: PaginatedList<any> | undefined;

  choosenItem: any;

  SortDirectionEnum = SortDirectionEnum;

  FormInputTypeEnum = FormInputTypeEnum;

  sorting!: SortingModel;

  pagination!: PaginationModel;

  pagesList: number[] = [];
  pageSizeOptions = [10, 25, 50, 75];

  constructor(
    private readonly queryParamsService: QueryParamsService,
    private readonly httpClient: HttpClient,
    private readonly modalService: NgbModal) { }

  async ngOnInit() {
    this.sorting = await this.queryParamsService.getSortingParams();
    this.pagination = await this.queryParamsService.getPaginationParams();

    if (!this.pagination.pageNumber || !this.pagination.pageSize) {
      this.pagination = {
        pageNumber: 1,
        pageSize: 25
      } as PaginationModel;

      await this.queryParamsService.setPaginationParams(this.pagination);
    }

    await this.reloadDatatable();
  }

  async reloadDatatable() {
    const specifications = await this.queryParamsService.getAllQueryParams();
    const params = new HttpParams({ fromObject: specifications });

    this.httpClient.get<ServiceResult<PaginatedList<any>>>(this.apiUrl, { params }).subscribe({
      next: async (result: ServiceResult<PaginatedList<any>>) => {
        this.tableData = result.data!;

        this.pagesList = [];

        for (let i = 1; i <= this.tableData!.pagination.totalPages; i++) {
          this.pagesList.push(i);
        }

        if (this.pagination.pageNumber! > this.tableData!.pagination.totalPages) {
          this.pagination.pageNumber = 1;

          await this.queryParamsService.setPaginationParams(this.pagination);
        }
      }
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  onEditClick(item: any, modal: TemplateRef<any>) {
    this.options.editFormOptions?.properties.forEach(element => {
      this.options.editFormOptions?.form.controls[element.propName].setValue(item[element.propName]);
    });
    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' });
  }

  onActionClick(item: any, modal: TemplateRef<any>) {
    this.choosenItem = item;
    if (this.options.optionalAction) {
      if (this.options.optionalAction.form) {
        this.options.optionalAction.form.controls['id'].setValue(this.choosenItem['id']);
      }
      else if (this.options.optionalAction.message) {
        var actionResult = {
          actionName: this.options.optionalAction.actionName,
          object: this.choosenItem
        } as OptionalActionResultModel;

        this.onAction.emit(actionResult);

        return;
      }
    }

    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' });
  }

  getPropValue(row: any, column: TableColumnSettingsModel) {
    const value = row[column.dataPropName!];

    if (!column.transformAction) {
      return value;
    }

    return column.transformAction(value);
  }

  getNavigationLink(row: any, column: TableColumnSettingsModel) {
    const routeParam = row[column.linkRouteParamName!];
    const url = column.pageLink!.replace('$routeParam$', routeParam);

    return url;
  }

  isTextType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Text;
  isNumberType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Number;
  isTextAreaType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.TextArea;
  isPasswordType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Password;
  isSelectType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Select;
  isDateType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Date;

  async decrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber! - 1);
  }

  async onPageNumberChanged(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;

    await this.queryParamsService.setPaginationParams(this.pagination);

    await this.reloadDatatable();
  }

  async incrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber! + 1);
  }

  async onPageSizeChanged(pageSize: any) {
    this.pagination.pageSize = pageSize.target.value;

    await this.queryParamsService.setPaginationParams(this.pagination);

    await this.reloadDatatable();
  }

  async onSortingChanged(field: string) {
    if (this.sorting.sortField != field) {
      this.sorting.sortField = field;
      this.sorting.sortDirection = SortDirectionEnum.ASC;

      await this.queryParamsService.setSortingParams(this.sorting);
    } else if (this.sorting.sortField == field && this.sorting.sortDirection == SortDirectionEnum.ASC) {
      this.sorting.sortDirection = SortDirectionEnum.DESC;

      await this.queryParamsService.setSortingParams(this.sorting);
    } else {
      this.sorting.sortField = undefined;

      await this.queryParamsService.clearSortingParams();
    }

    await this.reloadDatatable();
  }

  onCreateSubmit(modal: any) {
    modal.close();
    this.onCreate.emit();
  }

  onEditSubmit(modal: any) {
    modal.close();
    this.onEdit.emit();
  }

  onDeleteSubmit(modal: any) {
    modal.close();
    this.onDelete.emit(this.choosenItem.id);
  }

  onOptionalActionSubmit(modal: any) {
    modal.close();

    var actionResult = {
      actionName: this.options.optionalAction!.actionName,
      object: this.options.optionalAction!.form != null ?
        this.options.optionalAction!.form.value
        : this.choosenItem['id']
    } as OptionalActionResultModel;

    this.onAction.emit(actionResult);
  }
}
