import { AfterViewInit, Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AuctionRequestsService } from "../../services/auction-requests.service";
import { AuctionResourcesService } from "../../services/auction-resources.service";
import { AuctionCategory, AuctionType, AuctionFinishMethod } from "../../models/auctions/Auction";
import { forkJoin } from "rxjs";
import { PostAuctionRequest } from "../../models/auction-requests/postAuctionRequest";
import { NgxSpinnerService } from "ngx-spinner";
import { AuthService } from "../../services/auth.service";
import { UserStatusEnum } from "../../models/users/userStatusEnum";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-new-auction-request',
  standalone: false,
  templateUrl: './new-auction-request.component.html'
})
export class NewAuctionRequestComponent implements OnInit, AfterViewInit {
  @ViewChild('userBlockedModal') userBlockedModal: TemplateRef<any> | undefined;

  createAuctionRequestForm: FormGroup | undefined;

  images: File[] = [];

  categories: AuctionCategory[] = [];
  auctionTypes: AuctionType[] = [];
  finishMethods: AuctionFinishMethod[] = [];

  auctionResourcesInitialized: boolean = false;

  showFinishTimeInterval: boolean = false;
  showRequestedStartTime: boolean = false;
  showAimPrice: boolean = false;

  auctionTimeError: string | null = null;

  finishTimeIntervalError: string | null = null;

  requestedStartTimeError: string | null = null;

  error: string | null = null;

  constructor(private readonly auctionRequestsService: AuctionRequestsService,
    private readonly auctionResourcesService: AuctionResourcesService,
    private readonly toastrService: ToastrService,
    private readonly router: Router,
    private readonly modalService: NgbModal,
    private readonly spinnerService: NgxSpinnerService,
    private readonly authSerivce: AuthService) {

  }

  ngOnInit(): void {
    this.createAuctionRequestForm = new FormGroup({
      categoryId: new FormControl<number | null>(null, [Validators.required]),
      typeId: new FormControl<number | null>(null, [Validators.required]),
      finishMethodId: new FormControl<number | null>(null, [Validators.required]),
      lotTitle: new FormControl<string | null>(null, [Validators.required]),
      lotDescription: new FormControl<string | null>(null, [Validators.required]),
      auctionTimeDays: new FormControl<number | null>(0, [Validators.required, Validators.min(0), Validators.max(99)]),
      auctionTimeHours: new FormControl<number | null>(1, [Validators.required, Validators.min(0), Validators.max(23)]),
      startPrice: new FormControl<number | null>(null, [Validators.required, Validators.min(100), Validators.max(10e9)]),
      requestStartTimeFlag: new FormControl<boolean>(false),
      requestedStartTime: new FormControl<Date | null>(null),
      finishTimeIntervalHours: new FormControl<number | null>(null, [Validators.min(0), Validators.max(5)]),
      finishTimeIntervalMinutes: new FormControl<number | null>(null, [Validators.min(0), Validators.max(59)]),
      bidAmountInterval: new FormControl<number | null>(null, [Validators.required, Validators.min(0.01), Validators.max(10e5)]),
      setAimPriceFlag: new FormControl<boolean>(false),
      aimPrice: new FormControl<number | null>(null, [Validators.min(100), Validators.max(10e9)])
    });
  }

  ngAfterViewInit(): void {
    if (this.authSerivce.userStatus === UserStatusEnum.Active)
      this.fetchAuctionRecources();
    else
      this.modalService.open(this.userBlockedModal);
  }

  fetchAuctionRecources() {
    forkJoin([
      this.auctionResourcesService.getAuctionCategories(),
      this.auctionResourcesService.getAuctionTypes(),
      this.auctionResourcesService.getAuctionFinishMethods()
    ])
      .subscribe({
        next: ([categoriesResult, typesResult, finishMethodsResult]) => {
          this.categories = categoriesResult.data!;
          this.auctionTypes = typesResult.data!;
          this.finishMethods = finishMethodsResult.data!;

          this.auctionResourcesInitialized = true;
        },
        error: (err) => this.handleError(err)
      });
  }

  get categoryId() {
    return this.createAuctionRequestForm!.get('categoryId');
  }

  get typeId() {
    return this.createAuctionRequestForm!.get('typeId');
  }

  get finishMethodId() {
    return this.createAuctionRequestForm!.get('finishMethodId');
  }

  get lotTitle() {
    return this.createAuctionRequestForm!.get('lotTitle');
  }

  get lotDescription() {
    return this.createAuctionRequestForm!.get('lotDescription');
  }

  get auctionTimeDays() {
    return this.createAuctionRequestForm!.get('auctionTimeDays');
  }

  get auctionTimeHours() {
    return this.createAuctionRequestForm!.get('auctionTimeHours');
  }

  get requestStartTimeFlag() {
    return this.createAuctionRequestForm!.get('requestStartTimeFlag');
  }

  get requestedStartTime() {
    return this.createAuctionRequestForm!.get('requestedStartTime');
  }

  get startPrice() {
    return this.createAuctionRequestForm!.get('startPrice');
  }

  get finishTimeIntervalHours() {
    return this.createAuctionRequestForm!.get('finishTimeIntervalHours');
  }

  get finishTimeIntervalMinutes() {
    return this.createAuctionRequestForm!.get('finishTimeIntervalMinutes');
  }

  get bidAmountInterval() {
    return this.createAuctionRequestForm!.get('bidAmountInterval');
  }

  get setAimPriceFlag() {
    return this.createAuctionRequestForm!.get('setAimPriceFlag');
  }

  get aimPrice() {
    return this.createAuctionRequestForm!.get('aimPrice');
  }

  onFinishMethodChange(finishMethod: AuctionFinishMethod) {
    this.showFinishTimeInterval = finishMethod.name === 'Dynamic finish method';
  }

  onSetStartTimeFlagChange() {
    this.showRequestedStartTime = this.requestStartTimeFlag?.value ?? false;
  }

  onSetAimPriceFlagChange() {
    this.showAimPrice = this.setAimPriceFlag?.value ?? false;
  }

  onImagesChange(files: any) {
    this.images = files.target.files;
  }

  onSubmit() {
    if (!this.createAuctionRequestForm!.valid || !this.validateAuctionTime() || !this.validateFinishIntervalTime()) {
      return;
    }

    const formValue = this.createAuctionRequestForm!.value;

    const auctionRequest = {
      images: this.images,
      auctionCategoryId: formValue.categoryId,
      auctionTypeId: formValue.typeId,
      auctionFinishMethodId: formValue.finishMethodId,
      lotTitle: formValue.lotTitle,
      lotDescription: formValue.lotDescription,
      requestedAuctionTime: `${formValue.auctionTimeDays}.${formValue.auctionTimeHours}:0:0`,
      startPrice: formValue.startPrice,
      bidAmountInterval: formValue.bidAmountInterval,
      aimPrice: formValue.aimPrice
    } as PostAuctionRequest;

    if (this.isDynamicFinishMethod(formValue.finishMethodId)) {
      auctionRequest.finishTimeInterval = `${formValue.finishTimeIntervalHours}:${formValue.finishTimeIntervalMinutes}:0`;
    }

    if (formValue.requestStartTimeFlag) {
      auctionRequest.requestedStartTime = formValue.requestedStartTime;
    }

    this.spinnerService.show();

    this.auctionRequestsService.postAuctionRequest(auctionRequest).subscribe({
      next: (response) => {
        this.spinnerService.hide();

        if (response.isSuccessfull) {
          this.toastrService.success(response.message!, 'Success');

          this.router.navigate(['/']);
        }
        else {
          this.toastrService.error(response.errors[0], 'Error');
        }
      },
      error: (error) => {
        this.spinnerService.hide();
      }
    });
  }

  private validateAuctionTime(): boolean {
    if (!this.auctionTimeDays || !this.auctionTimeHours || (Number.parseInt(this.auctionTimeDays.value) == 0 && Number.parseInt(this.auctionTimeHours.value) == 0)) {
      this.auctionTimeError = 'Minimum auction time can be 1 hour.';

      return false;
    }

    this.auctionTimeError = null;

    return true;
  }

  private validateFinishIntervalTime(): boolean {
    if (!this.finishTimeIntervalHours || !this.finishTimeIntervalMinutes || (Number.parseInt(this.finishTimeIntervalHours.value) == 5 && Number.parseInt(this.finishTimeIntervalMinutes.value) > 0)) {
      this.finishTimeIntervalError = 'Maximum finish time interval can be 5 hours.';

      return false;
    }

    if (Number.parseInt(this.finishTimeIntervalHours.value) == 0 && Number.parseInt(this.finishTimeIntervalMinutes.value) < 5) {
      this.finishTimeIntervalError = 'Minimum finish time interval can be 5 minutes.';

      return false;
    }

    this.finishTimeIntervalError = null;

    return true;
  }

  private validateRequestedStartTime(): boolean {
    if (!this.requestStartTimeFlag) {
      return true;
    }

    const currentTimePlusTenMins = new Date();
    currentTimePlusTenMins.setTime(currentTimePlusTenMins.getTime() + 10 * 60 * 1000);
    const currentTimePlusOneMongth = new Date();
    currentTimePlusOneMongth.setMonth(currentTimePlusOneMongth.getMonth() + 1);

    if (!this.requestedStartTime || new Date(this.requestedStartTime.value) < currentTimePlusTenMins) {
      this.finishTimeIntervalError = 'You are not allowed to request auction less than in 10 mins.';

      return false;
    }

    this.finishTimeIntervalError = null;

    if (!this.requestedStartTime || new Date(this.requestedStartTime.value) > currentTimePlusOneMongth) {
      this.requestedStartTimeError = 'You are not allowed to request auction more than in 1 mongth.';

      return false;
    }

    this.requestedStartTimeError = null;

    return true;
  }

  private isDynamicFinishMethod(finishMethodId: number): boolean {
    return finishMethodId === this.finishMethods.filter(x => x.name === 'Dynamic finish method')[0].id
  }

  private handleError(err: any): void {
    if (err?.error?.errors && Array.isArray(err.error.errors)) {
      this.toastrService.error(err.error.errors[0], 'Error');
    }
  }
}
