import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { DataTableOptionsModel } from "../models/shared/dataTableOptionsModel";
import { UserStatusEnum } from "../models/users/userStatusEnum";
import { Observable } from "rxjs";
import { ServiceMessage, ServiceResult } from "../models/shared/serviceResult";
import { UserProfileInfo } from "../models/users/userProfileInfo";
import { BlockUserModel } from "../models/users/blockUserModel";
import { ModeratorInfoBasic } from "../models/users/moderatorInfoBasic";

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  baseUrl: string = `${environment.apiUrl}${environment.apiPrefix}`;

  constructor(private readonly httpClient: HttpClient) { }

  getAllModerators(): Observable<ServiceResult<ModeratorInfoBasic[]>> {
    return this.httpClient.get<ServiceResult<ModeratorInfoBasic[]>>(`${this.baseUrl}/moderators`);
  }

  getUserProfileDetails(userId: number): Observable<ServiceResult<UserProfileInfo>> {
    return this.httpClient.get<ServiceResult<UserProfileInfo>>(`${this.baseUrl}/users/${userId}`);
  }

  blockUser(userId: number, blockUser: BlockUserModel): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/${userId}/users/block`, blockUser);
  }

  unblockUser(userId: number): Observable<ServiceMessage> {
    return this.httpClient.put<ServiceMessage>(`${this.baseUrl}/${userId}/users/unblock`, null);
  }

  getDataTableApiUrl() {
    return `${this.baseUrl}/users`;
  }

  getDataTableOptions() {
    return {
      id: 'users',
      title: null,
      resourceName: 'user',
      showIndexColumn: false,
      allowCreating: false,
      createFormOptions: null,
      allowEdit: false,
      editFormOptions: null,
      allowDelete: false,
      optionalAction: null,
      emptyListDisplayLabel: 'There are not users mathing your filters.',
      columnSettings: [
        {
          title: 'Id',
          dataPropName: 'id',
          isOrderable: true,
          isLink: true,
          pageLink: '/users/$routeParam$',
          linkRouteParamName: 'id',
          transformAction: (value) => `#${value}`
        },
        {
          title: 'Username',
          dataPropName: 'username',
          isOrderable: true
        },
        {
          title: 'Full name',
          dataPropName: 'fullName',
          isOrderable: true
        },
        {
          title: 'Email',
          dataPropName: 'email',
          isOrderable: true
        },
        {
          title: 'Status',
          dataPropName: 'status',
          isOrderable: true,
          transformAction: (value: UserStatusEnum) => {
            switch (value) {
              case UserStatusEnum.Active:
                return 'Active';
              case UserStatusEnum.Blocked:
                return 'Blocked';
              case UserStatusEnum.Deleted:
                return 'Deleted';
            }
          }
        }
      ]
    } as DataTableOptionsModel;
  }
}
