import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionModel } from '../../models/auctionModel';
import { Router } from '@angular/router';
import { DeepLinkingService } from 'src/app/services/deep-linking.service';

@Component({
  selector: 'auction-card',
  templateUrl: './auction-card.component.html'
})
export class AuctionCardComponent {
  @Input()
  auction: AuctionModel;
}
