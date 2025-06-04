import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { FormInputTypeEnum } from "../models/shared/formInputTypeEnum";
import { PostAuctionCategory } from "../models/auction-categories/postAuctionCategory";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { HttpClient } from "@angular/common/http";
import { AuctionCategory } from "../models/auction-categories/auctionCategory";

@Injectable({
  providedIn: 'root'
})
export class AuctionCategoriesService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}/auction-categories`;

  constructor(private readonly httpClient: HttpClient) { }

  getAllAuctionCategories(): Observable<ServiceResult<AuctionCategory[]>> {
    return this.httpClient.get<ServiceResult<AuctionCategory[]>>(`${this.baseUrl}/all`);
  }

  postAuctionCategory(category: PostAuctionCategory): Observable<ServiceMessage> {
    return this.httpClient.post<ServiceMessage>(`${this.baseUrl}`, category);
  }

  editAuctionCategory(id: number, category: PostAuctionCategory): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/${id}`, category);
  }

  deleteAuctionCategory(id: number): Observable<ServiceMessage> {
    return this.httpClient.delete<ServiceMessage>(`${this.baseUrl}/${id}`);
  }

  getDataTableApiUrl() {
    return this.baseUrl;
  }

  getDataTableOptions() {
    return {
      title: null,
      resourceName: 'auction category',
      showIndexColumn: false,
      allowCreating: true,
      createFormOptions: {
        form: new FormGroup({
          name: new FormControl(null, [Validators.required]),
          description: new FormControl(null, [Validators.required])
        }),
        properties: [
          {
            label: 'Name',
            propName: 'name',
            type: FormInputTypeEnum.Text,
          },
          {
            label: 'Description',
            propName: 'description',
            type: FormInputTypeEnum.Text,
          }
        ],
      },
      allowEdit: true,
      editFormOptions: {
        form: new FormGroup({
          id: new FormControl(null, [Validators.required]),
          name: new FormControl(null, [Validators.required]),
          description: new FormControl(null, [Validators.required])
        }),
        properties: [
          {
            label: 'Id',
            propName: 'id',
            type: FormInputTypeEnum.Number,
          },
          {
            label: 'Name',
            propName: 'name',
            type: FormInputTypeEnum.Text,
          },
          {
            label: 'Description',
            propName: 'description',
            type: FormInputTypeEnum.Text,
          }
        ],
      },
      allowDelete: true,
      optionalAction: null,
      emptyListDisplayLabel: 'The list of auction categories is empty.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: true
        },
        {
          title: 'Name',
          dataPropName: 'name',
          isOrderable: true
        },
        {
          title: 'Description',
          dataPropName: 'description',
          isOrderable: false
        },
      ]
    } as DataTableOptionsModel;
  }
}
