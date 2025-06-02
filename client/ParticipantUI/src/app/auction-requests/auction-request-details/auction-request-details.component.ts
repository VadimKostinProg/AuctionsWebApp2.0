import { Component, OnInit, TemplateRef } from "@angular/core";
import { AuctionRequest } from "../../models/auction-requests/auctionRequest";
import { AuctionRequestsService } from "../../services/auction-requests.service";
import { ToastrService } from "ngx-toastr";
import { ActivatedRoute, Router } from "@angular/router";
import { AuctionRequestStatusEnum } from "../../models/auction-requests/auctionRequestStatusEnum";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-auction-request-details',
  templateUrl: './auction-request-details.component.html',
  styleUrls: ['./auction-request-details.component.scss'],
  standalone: false
})
export class AuctionRequestDetailsComponent implements OnInit {

  auctionRequestDetails: AuctionRequest | undefined;

  AuctionRequestStatusEnum = AuctionRequestStatusEnum;

  constructor(private readonly auctionRequestsService: AuctionRequestsService,
    private readonly toastrService: ToastrService,
    private readonly modalService: NgbModal,
    private readonly router: Router,
    private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const auctionRequestId = params.get('auctionRequestId');

      if (auctionRequestId == null) {
        this.toastrService.error('Invalid route params.', 'Error');

        return;
      }

      this.auctionRequestsService.getAuctionRequestDetails(parseInt(auctionRequestId)).subscribe({
        next: result => {
          this.auctionRequestDetails = result.data!;
        },
        error: err => {
          if (err?.error?.errors && Array.isArray(err.error.errors)) {
            this.toastrService.error(err.error.errors[0], 'Error');
          }
        }
      })
    });
  }

  open(content: TemplateRef<any>) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' })
  }

  cancelAuctionRequest(modal: any) {
    modal.close();

    this.auctionRequestsService.cancelAuctionRequest(this.auctionRequestDetails!.id).subscribe({
      next: (result) => {
        this.toastrService.success(result.message!, 'Success');

        this.router.navigate(['/auction-requests']);
      },
      error: (err) => {
        if (err?.error?.errors && Array.isArray(err.error.errors)) {
          this.toastrService.error(err.error.errors[0], 'Error');
        }
      }
    })
  }

  getAuctionRequestStatusText(status: number): string {
    switch (status) {
      case AuctionRequestStatusEnum.Pending:
        return 'PENDING';
      case AuctionRequestStatusEnum.CanceledByUser:
        return 'CANCELED BY USER';
      case AuctionRequestStatusEnum.Approved:
        return 'APPROVED';
      case AuctionRequestStatusEnum.Declined:
        return 'DECLINED';
      default:
        return 'UNKNOWN';
    }
  }
}
