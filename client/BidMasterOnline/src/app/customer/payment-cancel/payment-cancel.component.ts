import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-payment-cancel',
  templateUrl: './payment-cancel.component.html'
})
export class PaymentCancelComponent implements OnInit {

  constructor(private readonly toastrService: ToastrService,
    private readonly router: Router) {

  }

  ngOnInit() {
    this.toastrService.error('Payment failed.', 'Error');

    this.router.navigate(['/']);
  }
}
