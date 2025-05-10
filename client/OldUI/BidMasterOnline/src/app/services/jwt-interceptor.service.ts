import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JwtInterceptorService implements HttpInterceptor {

  constructor() { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (localStorage.getItem('authenticatedUser') != null) {
      var authenticatedUser = JSON.parse(localStorage.getItem('authenticatedUser') as string);

      request = request.clone({
        setHeaders: {
          Authorization: "Bearer " + authenticatedUser.token,
        }
      });
    }

    return next.handle(request);
  }
}
