import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class RedirectService {
  // Define routes that should redirect to dashboard if user is logged in
  private readonly authRoutes = ['/login', '/signup', '/forgot-password', '/reset-password'];

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  /**
   * Check if user should be redirected to dashboard
   * Call this in components that are auth-related (login, signup, etc.)
   */
  redirectToDashboardIfLoggedIn(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  /**
   * Check if current route is an auth route
   */
  isAuthRoute(path: string): boolean {
    return this.authRoutes.includes(path);
  }

  /**
   * Get the list of auth routes
   */
  getAuthRoutes(): string[] {
    return this.authRoutes;
  }
}
