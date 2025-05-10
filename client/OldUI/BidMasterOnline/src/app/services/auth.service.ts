import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Observable, catchError, map, tap, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SignInModel } from '../models/signInModel';
import { AuthenticationModel } from '../models/authenticationModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl: string = `${environment.apiUrl}/api/v1/auth`;

  user: AuthenticationModel | null;

  constructor(private readonly httpClient: HttpClient) {
    this.user = JSON.parse(localStorage.getItem('authenticatedUser') as string);
  }

  signIn(signInModel: SignInModel): Observable<AuthenticationModel> {
    return this.httpClient.post<AuthenticationModel>(`${this.baseUrl}/login`, signInModel)
      .pipe(map(user => {
        if (user) {
          this.user = user;
          localStorage.setItem('authenticatedUser', JSON.stringify(user));
        }

        return user;
      }));
  }

  logOut() {
    this.user = null;
    localStorage.removeItem('authenticatedUser');
  }
}
