import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { AuctionsPaymentDeliveryOptionsService } from './services/auction-payment-delivery-options.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(private readonly authService: AuthService,
    private readonly paymentDeliveryOptionsService: AuctionsPaymentDeliveryOptionsService) {
    this.authService = authService;
  }

  get user() {
    return this.authService.user;
  }

  onLogOutClick() {
    this.authService.logOut();
  }
}