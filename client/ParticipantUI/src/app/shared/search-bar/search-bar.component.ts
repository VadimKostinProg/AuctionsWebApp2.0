import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { QueryParamsService } from '../../services/query-params.service';

@Component({
  selector: 'search-bar',
  standalone: false,
  templateUrl: './search-bar.component.html'
})
export class SearchBarComponent implements OnInit {

  searchTerm: string = '';

  @Input()
  placeholder: string = 'Search...';

  @Output()
  onSubmit = new EventEmitter<void>();

  constructor(private readonly queryParamsService: QueryParamsService) {
  }

  async ngOnInit(): Promise<void> {
    this.searchTerm = await this.queryParamsService.getSearchTerm() || '';
  }

  async onSearchPressed() {
    if (this.searchTerm !== '')
      await this.queryParamsService.setQueryParam('searchTerm', this.searchTerm);
    else
      await this.queryParamsService.clearSearchTerm();

    this.onSubmit.emit();
  }
}
