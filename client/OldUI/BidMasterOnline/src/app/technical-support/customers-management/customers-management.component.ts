import { Component, OnInit, ViewChild } from '@angular/core';
import { UsersService } from 'src/app/services/users.service';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { OptionalActionResultModel } from 'src/app/models/optionalActionResultModal';
import { BlockUserModel } from 'src/app/models/blockUserModel';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-customers-management',
  templateUrl: './customers-management.component.html'
})
export class CustomersManagementComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dateTable: DataTableComponent;

  title: string = 'Customers';

  options: DataTableOptionsModel;

  placeholder: string = 'Search user...';

  status: string;

  showError: boolean;
  error: string = 'Invalid query parameters.';

  constructor(private readonly usersService: UsersService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly toastrService: ToastrService) {

  }

  async ngOnInit() {
    var status = await this.deepLinkingService.getQueryParam('status') || 'Active';

    this.showError = false;

    if (status != 'Active' && status != 'Blocked') {
      this.showError = true;
    }

    this.status = status;
    await this.updateStatus(this.status);
  }

  getCustomersDataTableApiUrl() {
    return this.usersService.getCustomersDataTableApiUrl();
  }

  async onSearchClicked() {
    await this.dateTable.reloadDatatable();
  }

  async onStatusChanged(status: any) {
    var value = status.target.value;

    await this.updateStatus(value);
  }

  async updateStatus(value: string) {
    this.status = value;

    var showBlocked = value == 'Blocked';

    await this.deepLinkingService.setQueryParam('status', this.status);
    this.options = this.usersService.getCustomersDataTableOptions(showBlocked);
    await this.dateTable.reloadDatatable();
  }

  async onActionExecuted(actionResult: OptionalActionResultModel) {

    switch (actionResult.actionName) {
      case 'Block':
        var model = {
          userId: actionResult.object.id,
          blockingReason: actionResult.object.blockingReason,
          days: actionResult.object.days
        } as BlockUserModel;
        this.usersService.blockUser(model).subscribe(
          (response) => {
            this.toastrService.success(response.message, 'Success');
          },
          (error) => {
            this.toastrService.success(error.error, 'Error');
          }
        );
        break;
      case 'Unblock':
        const userId = actionResult.object as string;
        this.usersService.unblockUser(userId).subscribe(
          (response) => {
            this.toastrService.success(response.message, 'Success');
          },
          (error) => {
            this.toastrService.success(error.error, 'Error');
          }
        );
        break;
    }

    await this.dateTable.reloadDatatable();
  }
}
