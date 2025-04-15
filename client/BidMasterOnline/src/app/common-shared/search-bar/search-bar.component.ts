import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AuctionsDeepLinkingService } from 'src/app/services/auctions-deep-linking.service';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';

@Component({
  selector: 'search-bar',
  templateUrl: './search-bar.component.html'
})
export class SearchBarComponent implements OnInit {

  searchTerm: string;

  @Input()
  placeholder: string = 'Search...';

  @Output()
  onSubmit = new EventEmitter<void>();

  constructor(private readonly auctionsDeepLinkingService: AuctionsDeepLinkingService) {
  }

  async ngOnInit(): Promise<void> {
    this.searchTerm = await this.auctionsDeepLinkingService.getSearchTerm() || null;
  }

  async onSearchPressed() {
    if (this.searchTerm !== null && this.searchTerm !== '')
      await this.auctionsDeepLinkingService.setQueryParam('searchTerm', this.searchTerm);
    else
      await this.auctionsDeepLinkingService.clearSearchTerm();

    this.onSubmit.emit();
  }
}
