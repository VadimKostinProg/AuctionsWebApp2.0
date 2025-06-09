import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';
import { PaymentsService } from '../../services/payments.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-payment-setup',
  templateUrl: './payment-setup.component.html',
  styleUrls: ['./payment-setup.component.scss'],
  standalone: false
})
export class PaymentSetupComponent implements OnInit, OnDestroy {
  @ViewChild('cardElementContainer', { static: true }) cardElementContainer!: ElementRef;

  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  cardElement: StripeCardElement | null = null;

  loading: boolean = true;
  error: string | null = null;
  message: string | null = null;

  userId: number | undefined;

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly paymentsService: PaymentsService,
    private readonly toastrService: ToastrService
  ) { }

  async ngOnInit(): Promise<void> {
    this.userId = this.authService.user.userId;

    this.stripe = await loadStripe(environment.stripePublishableKey);

    if (!this.stripe) {
      this.error = 'Failed to load Stripe. Please check your Publishable Key.';
      this.loading = false;
      return;
    }

    this.elements = this.stripe.elements();

    const cardElementStyles = {
      base: {
        iconColor: '#666EE8',
        color: '#313259',
        fontWeight: '300',
        fontFamily: 'Helvetica Neue, Helvetica, Arial, sans-serif',
        fontSize: '18px',
        '::placeholder': {
          color: '#CFD7E0',
        },
      },
    };

    this.cardElement = this.elements.create('card', { style: cardElementStyles });

    if (this.cardElementContainer && this.cardElement) {
      this.cardElement.mount(this.cardElementContainer.nativeElement);
    } else {
      console.error('Card element container or card element not found.');
    }

    this.loading = false;
  }

  ngOnDestroy(): void {
    if (this.cardElement) {
      this.cardElement.destroy();
    }
  }

  async attachPaymentMethod(): Promise<void> {
    this.loading = true;
    this.error = null;
    this.message = null;

    if (!this.userId) {
      this.error = 'User ID is not set. Cannot proceed.';
      this.loading = false;
      return;
    }

    if (!this.stripe || !this.cardElement) {
      this.error = 'Stripe or card element not initialized.';
      this.loading = false;
      return;
    }

    try {
      this.paymentsService.createSetupIntent().subscribe({
        next: async result => {
          const clientSecret = result.data;

          if (!clientSecret) {
            throw new Error('Failed to get clientSecret from backend.');
          }

          const { setupIntent, error } = await this.stripe!.confirmCardSetup(
            clientSecret,
            {
              payment_method: {
                card: this.cardElement!,
                billing_details: {
                  name: this.authService.user.username,
                  email: this.authService.user.email,
                }
              },
            }
          );

          if (error) {
            this.error = error.message ?? '';
            this.loading = false;
          } else {
            console.log('SetupIntent succeeded:', setupIntent);

            this.paymentsService.confirmIntent(setupIntent.id).subscribe({
              next: result => {
                this.toastrService.success(result.message!, 'Success');
                this.router.navigate(['/users', this.userId]);
              },
              error: err => {
                if (err?.error?.errors && Array.isArray(err.error.errors)) {
                  this.toastrService.error(err.error.errors[0], 'Error');
                }
              }
            });
            this.loading = false;
          }
        },
        error: err => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
        }
      });

    } catch (e: any) {
      console.error('Error attaching payment method:', e);
      this.error = e.message || 'An unexpected error occurred.';
      this.loading = false;
    }
  }
}
