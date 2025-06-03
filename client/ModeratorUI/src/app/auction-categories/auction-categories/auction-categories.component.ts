import { Component, OnInit, ViewChild } from "@angular/core";
import { AuctionCategoriesService } from "../../services/auciton-categories.service";
import { DataTableOptionsModel } from "../../models/shared/dataTableOptionsModel";
import { ToastrService } from "ngx-toastr";
import { PostAuctionCategory } from "../../models/auction-categories/postAuctionCategory";
import { DataTableComponent } from "../../shared/data-table/data-table.component";

@Component({
  selector: 'app-auction-categories',
  templateUrl: 'auction-categories.component.html',
  standalone: false
})
export class AuctionCategoriesComponent implements OnInit {

  @ViewChild(DataTableComponent) dateTable: DataTableComponent | undefined;

  options: DataTableOptionsModel | undefined;

  placeholder: string = 'Search category...';

  constructor(private readonly auctionCategoriesService: AuctionCategoriesService,
    private readonly toastrService: ToastrService) { }

  ngOnInit(): void {
    this.options = this.auctionCategoriesService.getDataTableOptions();
  }

  get dataTableApiUrl() {
    return this.auctionCategoriesService.getDataTableApiUrl();
  }

  onCreateCategory() {
    if (!this.options!.createFormOptions!.form.valid) {
      return;
    }
    const value = this.options!.createFormOptions!.form.value;

    const category = {
      name: value.name,
      description: value.description
    } as PostAuctionCategory;

    this.auctionCategoriesService.postAuctionCategory(category).subscribe({
      next: async (response) => {
        this.toastrService.success(response.message!, 'Success');

        await this.dateTable!.reloadDatatable();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });

    this.options = this.auctionCategoriesService.getDataTableOptions();
  }

  onEditCategory() {
    if (!this.options!.editFormOptions!.form.valid) {
      return;
    }

    const value = this.options!.editFormOptions!.form.value;

    const category = {
      name: value.name,
      description: value.description
    } as PostAuctionCategory;

    this.auctionCategoriesService.editAuctionCategory(value.id, category).subscribe({
      next: async (response) => {
        this.toastrService.success(response.message!, 'Success');

        await this.dateTable!.reloadDatatable();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  onDeleteCategory(categoryId: number) {
    this.auctionCategoriesService.deleteAuctionCategory(categoryId).subscribe({
      next: async (response) => {
        this.toastrService.success(response.message!, 'Success');

        await this.dateTable!.reloadDatatable();
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  async onSearchClicked() {
    if (this.dateTable)
      await this.dateTable.reloadDatatable();
  }
}
