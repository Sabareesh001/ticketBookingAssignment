import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { OperatorAuthService } from '../services/operator-auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthRedirectGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private operatorAuthService: OperatorAuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // If user is logged in, redirect to dashboard
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
      return false;
    }

    // If operator is logged in, redirect to dashboard
    if (this.operatorAuthService.hasToken()) {
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }
}
