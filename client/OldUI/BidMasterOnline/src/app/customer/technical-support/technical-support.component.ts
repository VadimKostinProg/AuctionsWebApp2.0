import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { AuctionsService } from 'src/app/services/auctions.service';
import { AuctionModel } from 'src/app/models/auctionModel';
import { AuctionParticipantEnum } from 'src/app/models/auctionParticipantEnum';
import { ToastrService } from 'ngx-toastr';
import { ComplaintsService } from 'src/app/services/complaints.service';
import { TechnicalSupportRequestsService } from 'src/app/services/technical-support-requests.service';
import { SetComplaintModel } from 'src/app/models/setComplaintModel';
import { ComplaintTypeEnum } from 'src/app/models/complaintTypeEnum';

@Component({
  selector: 'app-technical-support',
  templateUrl: './technical-support.component.html'
})
export class TechnicalSupportComponent implements OnInit {

  notConfirmedAuctionsForAuctionist: AuctionModel[] = [];
  notConfirmedAuctionsForAuctioner: AuctionModel[] = [];

  complaintOnNoDeliveryForm: FormGroup;
  complaintOnNoPaymentForm: FormGroup;

  requestForm: FormGroup;

  constructor(private readonly auctionsService: AuctionsService,
    private readonly toastrService: ToastrService,
    private readonly complaintsService: ComplaintsService,
    private readonly tsrService: TechnicalSupportRequestsService) {

  }

  ngOnInit(): void {
    this.auctionsService.getFinishedAuctionsWithNotConfirmedOptions(AuctionParticipantEnum.Auctionist).subscribe(
      (response) => {
        this.notConfirmedAuctionsForAuctionist = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.auctionsService.getFinishedAuctionsWithNotConfirmedOptions(AuctionParticipantEnum.Auctioner).subscribe(
      (response) => {
        this.notConfirmedAuctionsForAuctioner = response;
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );

    this.reloadComplaintOnNoDeliveryForm();

    this.reloadComplaintOnNoPaymentForm();

    this.reloadRequestForm();
  }

  get auctionIdNoDelivery() {
    return this.complaintOnNoDeliveryForm.get('auctionIdNoDelivery');
  }

  get complaintTextNoDelivery() {
    return this.complaintOnNoDeliveryForm.get('complaintTextNoDelivery');
  }

  get auctionIdNoPayment() {
    return this.complaintOnNoPaymentForm.get('auctionIdNoPayment');
  }

  get complaintTextNoPayment() {
    return this.complaintOnNoPaymentForm.get('complaintTextNoPayment');
  }

  get requestText() {
    return this.requestForm.get('requestText');
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

  reloadRequestForm() {
    this.requestForm = new FormGroup({
      requestText: new FormControl(null, [Validators.required])
    });
  }

  sendComplaintOnNoDelivery() {
    if (!this.complaintOnNoDeliveryForm.valid) {
      return;
    }

    const formValue = this.complaintOnNoDeliveryForm.value;

    this.reloadComplaintOnNoDeliveryForm();

    this.auctionsService.getAuctionDetailsById(formValue.auctionIdNoDelivery).subscribe(
      (response) => {
        var auctionDetails = response;

        const complaint = {
          accusedUserId: auctionDetails.auctionistId,
          auctionId: auctionDetails.id,
          accusingUserId: auctionDetails.winnerId,
          commentId: null,
          complaintType: ComplaintTypeEnum.ComplaintOnUserNonProvidingLot,
          complaintText: formValue.complaintTextNoDelivery
        } as SetComplaintModel;

        this.setComplaint(complaint);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  sendComplaintOnNoPayment() {
    if (!this.complaintOnNoPaymentForm.valid) {
      return;
    }

    const formValue = this.complaintOnNoPaymentForm.value;

    this.reloadComplaintOnNoPaymentForm();

    this.auctionsService.getAuctionDetailsById(formValue.auctionIdNoPayment).subscribe(
      (response) => {
        var auctionDetails = response;

        const complaint = {
          accusedUserId: auctionDetails.winnerId,
          auctionId: auctionDetails.id,
          accusingUserId: auctionDetails.auctionistId,
          commentId: null,
          complaintType: ComplaintTypeEnum.ComplaintOnUserNonPayemnt,
          complaintText: formValue.complaintTextNoPayment
        } as SetComplaintModel;

        this.setComplaint(complaint);
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }

  setComplaint(complaint: SetComplaintModel) {
    this.complaintsService.setComplaint(complaint).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    )
  }

  sendTechnicalSupportRequest() {
    if (!this.requestForm.valid) {
      return;
    }

    const requestText = this.requestForm.value.requestText;

    this.reloadRequestForm();

    this.tsrService.setTechnicalSupportRequest(requestText).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
