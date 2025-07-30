import { Component, OnInit } from "@angular/core";
import { Auction } from "../../models/auctions/Auction";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AuctionsService } from "../../services/auctions.service";
import { ComplaintsService } from "../../services/complaints.service";
import { ToastrService } from "ngx-toastr";
import { ComplaintTypeEnum } from "../../models/complaints/complaintTypeEnum";
import { PostComplaint } from "../../models/complaints/postComplaint";
import { AuthService } from "../../services/auth.service";
import { forkJoin } from "rxjs";

@Component({
  selector: 'app-complaints',
  templateUrl: './complaints.component.html',
  standalone: false,
})
export class ComplaintsComponent implements OnInit {
  notDeliveredAuctionsForBuyer: Auction[] | undefined;
  notPayedAuctionsForSeller: Auction[] | undefined;

  complaintOnNoDeliveryForm: FormGroup | undefined;
  complaintOnNoPaymentForm: FormGroup | undefined;

  constructor(private readonly auctionsService: AuctionsService,
    private readonly complaintsService: ComplaintsService,
    private readonly toastrService: ToastrService,
    private readonly authService: AuthService) { }

  ngOnInit(): void {
    forkJoin([
      this.auctionsService.getNotDeliveredAuctionsForBuyer(),
      this.auctionsService.getNotPayedAuctionsForSeller()
    ])
      .subscribe({
        next: ([notDeliveredResult, notPayedResult]) => {
          this.notDeliveredAuctionsForBuyer = notDeliveredResult.data!;
          this.notPayedAuctionsForSeller = notPayedResult.data!;
        },
        error: (err) => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
        }
      })

    this.reloadComplaintOnNoDeliveryForm();
    this.reloadComplaintOnNoPaymentForm();
  }

  reloadComplaintOnNoDeliveryForm() {
    this.complaintOnNoDeliveryForm = new FormGroup({
      auctionIdNoDelivery: new FormControl(null, [Validators.required]),
      complaintTextNoDelivery: new FormControl(null, [Validators.required])
    });
  }

  reloadComplaintOnNoPaymentForm() {
    this.complaintOnNoPaymentForm = new FormGroup({
      auctionIdNoPayment: new FormControl(null, [Validators.required]),
      complaintTextNoPayment: new FormControl(null, [Validators.required])
    });
  }

  get auctionIdNoDelivery() {
    return this.complaintOnNoDeliveryForm!.get('auctionIdNoDelivery');
  }

  get complaintTextNoDelivery() {
    return this.complaintOnNoDeliveryForm!.get('complaintTextNoDelivery');
  }

  get auctionIdNoPayment() {
    return this.complaintOnNoPaymentForm!.get('auctionIdNoPayment');
  }

  get complaintTextNoPayment() {
    return this.complaintOnNoPaymentForm!.get('complaintTextNoPayment');
  }

  sendComplaintOnNoDelivery() {
    if (!this.complaintOnNoDeliveryForm!.valid) {
      return;
    }

    const formValue = this.complaintOnNoDeliveryForm!.value;

    this.reloadComplaintOnNoDeliveryForm();

    this.auctionsService.getAuctionDetailsById(formValue.auctionIdNoDelivery).subscribe(
      (response) => {
        const auctionDetails = response.data!;

        const complaint = {
          accusedUserId: auctionDetails.type === 'Dutch Auction'
            ? auctionDetails.winner!.userId
            : auctionDetails.auctioneer.userId,
          accusedAuctionId: auctionDetails.id,
          accusingUserId: this.authService.user.userId,
          type: ComplaintTypeEnum.ComplaintOnUserBehaviour,
          complaintText: formValue.complaintTextNoDelivery
        } as PostComplaint;

        this.sendComplaint(complaint);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  sendComplaintOnNoPayment() {
    if (!this.complaintOnNoPaymentForm!.valid) {
      return;
    }

    const formValue = this.complaintOnNoPaymentForm!.value;

    this.reloadComplaintOnNoPaymentForm();

    this.auctionsService.getAuctionDetailsById(formValue.auctionIdNoPayment).subscribe({
      next: (response) => {
        const auctionDetails = response.data!;

        const complaint = {
          accusedUserId: auctionDetails.type === 'Dutch Auction'
            ? auctionDetails.auctioneer.userId
            : auctionDetails.winner!.userId,
          accusedAuctionId: auctionDetails.id,
          accusingUserId: this.authService.user.userId,
          type: ComplaintTypeEnum.ComplaintOnUserBehaviour,
          complaintText: formValue.complaintTextNoPayment
        } as PostComplaint;

        this.sendComplaint(complaint);
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }

  sendComplaint(complaint: PostComplaint) {
    this.complaintsService.postComplaint(complaint).subscribe({
      next: (response) => {
        this.toastrService.success(response.message!, 'Success');
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    });
  }
}
