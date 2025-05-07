import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class CanActivateGuardService implements CanActivate {

  constructor(private readonly authService: AuthService,
    private readonly router: Router,
    private readonly jwtHelperService: JwtHelperService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    var currentUser = this.authService.user;

    if (currentUser == null || this.jwtHelperService.isTokenExpired(currentUser.token)) {
      this.authService.logOut();
      this.router.navigate(["sign-in"]);
      return false;
    }

    if (!route.data['expectedRoles'].includes(currentUser.role)) {
      console.log('You have no access to this route.');
      return false;
    }

    return true;
  }
}
