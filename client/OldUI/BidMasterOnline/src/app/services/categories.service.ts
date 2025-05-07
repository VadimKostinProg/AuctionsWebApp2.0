import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Params } from '@angular/router';
import { environment } from 'src/environments/environment';
import { CategoryModel } from '../models/categoryModel';
import { Observable, catchError, throwError } from 'rxjs';
import { CreateCategoryModel } from '../models/createCategoryModel';
import { ListModel } from '../models/listModel';
import { DataTableOptionsModel } from '../models/dataTableOptionsModel';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormInputTypeEnum } from '../models/formInputTypeEnum';
import { UserRoleEnum } from '../models/userRoleEnum';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {

  baseUrl: string = `${environment.apiUrl}/api/v1/categories`;

  constructor(private readonly httpClient: HttpClient) { }

  getCategoriesDataTableApiUrl() {
    return `${this.baseUrl}/list`;
  }

  getAllCategories(): Observable<CategoryModel[]> {
    return this.httpClient.get<CategoryModel[]>(this.baseUrl);
  }

  getCategoriesList(specifications: Params): Observable<ListModel<CategoryModel>> {
    const params = new HttpParams({ fromObject: specifications });

    return this.httpClient.get<ListModel<CategoryModel>>(`${this.baseUrl}/list`, { params });
  }

  getCategoryById(id: string) {
    return this.httpClient.get<CategoryModel[]>(`${this.baseUrl}/${id}`);
  }

  createNewCategory(category: CreateCategoryModel): Observable<any> {
    return this.httpClient.post(this.baseUrl, category);
  }

  updateCategory(category: CategoryModel): Observable<any> {
    return this.httpClient.put(this.baseUrl, category);
  }

  deleteCategory(id: string): Observable<any> {
    return this.httpClient.delete(`${this.baseUrl}/${id}`);
  }

  getDataTableOptions() {
    var options = {
      title: 'Categories',
      resourceName: 'category',
      showIndexColumn: true,
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
            type: FormInputTypeEnum.Text,
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
      emptyListDisplayLabel: 'The list of categories is empty.',
      columnSettings: [
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

    return options;
  }
}
