import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CategoriesService } from 'src/app/services/categories.service';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dateTable: DataTableComponent;

  options: DataTableOptionsModel;

  placeholder: string = 'Search category...';

  error: string;

  constructor(private readonly categoriesService: CategoriesService,
    private readonly toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.options = this.categoriesService.getDataTableOptions();
  }

  onCreateCategory() {
    if (!this.options.createFormOptions.form.valid) {
      return;
    }
    var category = this.options.createFormOptions.form.value;

    this.categoriesService.createNewCategory(category).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        await this.dateTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.success(error.error, 'Error');
      }
    )

    this.options = this.categoriesService.getDataTableOptions();
  }

  onEditCategory() {
    if (!this.options.editFormOptions.form.valid) {
      return;
    }
    var category = this.options.editFormOptions.form.value;

    this.categoriesService.updateCategory(category).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        await this.dateTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.success(error.error, 'Error');
      }
    )
  }

  onDeleteCategory(categoryId: string) {
    this.categoriesService.deleteCategory(categoryId).subscribe(
      async (response) => {
        this.toastrService.success(response.message, 'Success');

        await this.dateTable.reloadDatatable();
      },
      (error) => {
        this.toastrService.success(error.error, 'Error');
      }
    );
  }

  async onSearchClicked() {
    await this.dateTable.reloadDatatable();
  }

  getCategoriesApiUrl() {
    return this.categoriesService.getCategoriesDataTableApiUrl();
  }
}
