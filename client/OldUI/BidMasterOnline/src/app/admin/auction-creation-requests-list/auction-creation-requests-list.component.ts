import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionsVerificationService } from 'src/app/services/auctions-verification.service';
import { DataTableOptionsModel } from 'src/app/models/dataTableOptionsModel';
import { OptionalActionResultModel } from 'src/app/models/optionalActionResultModal';
import { DataTableComponent } from 'src/app/common-shared/data-table/data-table.component';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auction-creation-requests-list',
  templateUrl: './auction-creation-requests-list.component.html'
})
export class AuctionCreationRequestsListComponent implements OnInit {

  @ViewChild(DataTableComponent)
  dataTable: DataTableComponent;

  options: DataTableOptionsModel;

  constructor(private readonly auctionVerificationService: AuctionsVerificationService,
    private readonly deepLinkingService: DeepLinkingService,
    private readonly router: Router) {

  }

  getNotApporovedAuctionsApiUrl() {
    return this.auctionVerificationService.getNotApporovedAuctionsDataTableApiUrl();
  }

  ngOnInit(): void {
    this.options = this.auctionVerificationService.getNotApporovedAuctionsDataTableOptions();
  }

  async onViewExecuted(actionResult: OptionalActionResultModel) {
    await this.deepLinkingService.setQueryParam('auctionId', actionResult.object.id);
    this.router.navigate(['/auction-creation-request'], { queryParamsHandling: 'merge' });
  }
}
