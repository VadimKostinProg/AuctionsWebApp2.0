import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { FormInputTypeEnum } from 'src/app/models/formInputTypeEnum';
import { ListModel } from 'src/app/models/listModel';
import { OptionalActionResultModel } from 'src/app/models/optionalActionResultModal';
import { PaginationModel } from 'src/app/models/paginationModel';
import { SortDirectionEnum } from 'src/app/models/sortDirectionEnum';
import { SortingModel } from 'src/app/models/sortingModel';
import { TableColumnSettingsModel } from 'src/app/models/tableColumnSettingsModel';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';

@Component({
  selector: 'data-table',
  templateUrl: './data-table.component.html'
})
export class DataTableComponent implements OnInit {

  @Input()
  options: DataTableOptionsModel;

  @Input()
  apiUrl: string;

  @Output()
  onCreate = new EventEmitter<void>();

  @Output()
  onEdit = new EventEmitter<void>();

  @Output()
  onDelete = new EventEmitter<string>();

  @Output()
  onAction = new EventEmitter<OptionalActionResultModel>();

  tableData: ListModel<any>;

  choosenItem: any;

  SortDirectionEnum = SortDirectionEnum;

  FormInputTypeEnum = FormInputTypeEnum;

  sorting: SortingModel;

  pagination: PaginationModel;

  pagesList: number[];
  pageSizeOptions = [10, 25, 50, 75];

  constructor(
    private readonly deepLinkingService: DeepLinkingService,
    private readonly httpClient: HttpClient,
    private readonly modalService: NgbModal) {

  }

  async ngOnInit() {
    this.sorting = await this.deepLinkingService.getSortingParams();
    this.pagination = await this.deepLinkingService.getPaginationParams();

    if (this.pagination.pageNumber == null || !this.pageSizeOptions.includes(this.pagination.pageSize)) {
      this.pagination = {
        pageNumber: 1,
        pageSize: 25
      } as PaginationModel;

      await this.deepLinkingService.setPaginationParams(this.pagination);
    }

    await this.reloadDatatable();
  }

  async reloadDatatable() {
    const specifications = await this.deepLinkingService.getAllQueryParams();
    const params = new HttpParams({ fromObject: specifications });

    this.httpClient.get(this.apiUrl, { params }).subscribe(
      async (data: any) => {
        this.tableData = data;

        this.pagesList = [];

        for (let i = 1; i <= this.tableData.totalPages; i++) {
          this.pagesList.push(i);
        }

        if (this.pagination.pageNumber > this.tableData.totalPages) {
          this.pagination.pageNumber = 1;

          await this.deepLinkingService.setPaginationParams(this.pagination);
        }
      }
    );
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
    if (this.options.optionalAction != null) {
      if (this.options.optionalAction.form != null) {
        this.options.optionalAction.form.controls['id'].setValue(this.choosenItem['id']);
      } else if (this.options.optionalAction.message == null) {
        var actionResult = {
          actionName: this.options.optionalAction.actionName,
          object: this.options.optionalAction.form != null ?
            this.options.optionalAction.form.value : this.choosenItem
        } as OptionalActionResultModel;

        this.onAction.emit(actionResult);

        return;
      }
    }

    this.modalService.open(modal, { ariaLabelledBy: 'modal-basic-title' });
  }

  getQueryParams(column: TableColumnSettingsModel, row: any) {
    const object = {
      [column.linkQueryParam]: row[column.linkQueryDataPropName]
    };

    return object;
  }

  isTextType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Text;
  isNumberType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Number;
  isTextAreaType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.TextArea;
  isPasswordType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Password;
  isSelectType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Select;
  isDateType = (inputType: FormInputTypeEnum): boolean => inputType == FormInputTypeEnum.Date;

  async decrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber - 1);
  }

  async onPageNumberChanged(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;

    await this.deepLinkingService.setPaginationParams(this.pagination);

    await this.reloadDatatable();
  }

  async incrementPageNumber() {
    await this.onPageNumberChanged(this.pagination.pageNumber + 1);
  }

  async onPageSizeChanged(pageSize: any) {
    this.pagination.pageSize = pageSize.target.value;

    await this.deepLinkingService.setPaginationParams(this.pagination);

    await this.reloadDatatable();
  }

  async onSortingChanged(field: string) {
    if (this.sorting.sortField != field) {
      this.sorting.sortField = field;
      this.sorting.sortDirection = SortDirectionEnum.ASC;

      await this.deepLinkingService.setSortingParams(this.sorting);
    } else if (this.sorting.sortField == field && this.sorting.sortDirection == SortDirectionEnum.ASC) {
      this.sorting.sortDirection = SortDirectionEnum.DESC;

      await this.deepLinkingService.setSortingParams(this.sorting);
    } else {
      this.sorting.sortField = null;

      await this.deepLinkingService.clearSortingParams();
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
      actionName: this.options.optionalAction.actionName,
      object: this.options.optionalAction.form != null ?
        this.options.optionalAction.form.value : this.choosenItem['id']
    } as OptionalActionResultModel;

    this.onAction.emit(actionResult);
  }
}
