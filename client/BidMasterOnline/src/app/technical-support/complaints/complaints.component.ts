import { Component, OnInit, ViewChild } from '@angular/core';
import { ComplaintsService } from 'src/app/services/complaints.service';
import { ComplaintTypeEnum } from 'src/app/models/complaintTypeEnum';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { OptionalActionResultModel } from 'src/app/models/optionalActionResultModal';
import { Router } from '@angular/router';

@Component({
  selector: 'app-complaints',
  templateUrl: './complaints.component.html'
})
export class ComplaintsComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dataTable: DataTableComponent;

  ComplaintTypeEnum = ComplaintTypeEnum;

  currentType: ComplaintTypeEnum;

  options: DataTableOptionsModel;

  constructor(private readonly complaintsService: ComplaintsService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly router: Router) {

  }

  async ngOnInit(): Promise<void> {
    this.currentType = ComplaintTypeEnum.ComplaintOnAuctionContent;

    await this.deepLinkingService.setQueryParam('type', ComplaintTypeEnum[this.currentType]);

    this.options = this.complaintsService.getDataTableOptions();
  }

  getComplaintsDataTableApiUrl() {
    return this.complaintsService.getDataTableApiUrl();
  }

  async onTypeChange(type: ComplaintTypeEnum) {
    this.currentType = type;

    await this.deepLinkingService.setQueryParam('type', ComplaintTypeEnum[this.currentType]);

    await this.dataTable.reloadDatatable();
  }

  async onViewExecuted(actionResult: OptionalActionResultModel) {
    await this.deepLinkingService.setQueryParam('complaintId', actionResult.object.id);
    this.router.navigate(['/complaint-details'], { queryParamsHandling: 'merge' });
  }
}
