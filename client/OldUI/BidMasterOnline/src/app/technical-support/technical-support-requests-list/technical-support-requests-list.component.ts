import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { Router } from '@angular/router';
import { TechnicalSupportRequestsService } from 'src/app/services/technical-support-requests.service';
import { OptionalActionResultModel } from 'src/app/models/optionalActionResultModal';

@Component({
  selector: 'app-technical-support-requests-list',
  templateUrl: './technical-support-requests-list.component.html'
})
export class TechnicalSupportRequestsListComponent {

  @ViewChild(DataTableComponent)
  dataTable: DataTableComponent;

  options: DataTableOptionsModel;

  constructor(private readonly tsrService: TechnicalSupportRequestsService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly router: Router) {

  }

  async ngOnInit(): Promise<void> {
    this.options = this.tsrService.getDataTableOptions();
  }

  getDataTableApiUrl() {
    return this.tsrService.getDataTableApiUrl();
  }

  getComplaintsDataTableApiUrl() {
    return this.tsrService.getDataTableApiUrl();
  }

  async onViewExecuted(actionResult: OptionalActionResultModel) {
    await this.deepLinkingService.setQueryParam('requestId', actionResult.object.id);
    this.router.navigate(['/technical-support-request'], { queryParamsHandling: 'merge' });
  }
}
