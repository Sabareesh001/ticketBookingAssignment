import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { OperatorAuthService } from '../services/operator-auth.service';

@Injectable({
  providedIn: 'root'
})
export class OperatorAuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private operatorAuthService: OperatorAuthService
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const isAuthenticated = this.operatorAuthService.hasToken();

    if (isAuthenticated) {
      return true;
    }

    // Not logged in, redirect to login
    this.router.navigate(['/login']);
    return false;
  }
}
