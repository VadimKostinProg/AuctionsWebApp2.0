import { Component, Input } from "@angular/core";
import { AuctionBasic } from "../../models/auctions/AuctionBasic";

@Component({
  selector: 'auction-card',
  templateUrl: './auction-card.component.html',
  styleUrl: './auction-card.component.scss'
})
export class AuctionCardComponent {
  @Input() auction: AuctionBasic | undefined;
}
