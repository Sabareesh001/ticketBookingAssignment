import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    console.log(`🔍 [AuthInterceptor] Intercepting request: ${request.url}`);
    
    // Skip operator-related requests - they have their own interceptor
    if (request.url.includes('/operator-dashboard') || request.url.includes('/operator-auth')) {
      console.log(`⏭️ [AuthInterceptor] Skipping operator URL: ${request.url}`);
      return next.handle(request).pipe(
        catchError((error: HttpErrorResponse) => {
          console.log(`❌ [AuthInterceptor] Error caught for operator URL: ${request.url}, Status: ${error.status}`);
          if (error.status === 401) {
            console.log(`🚨 [AuthInterceptor] 401 detected for operator URL`);
            const token = this.authService.getToken();
            if (token && this.isTokenExpired(token)) {
              console.warn('⚠️ [AuthInterceptor] Token expired on 401, auto-flushing');
              this.authService.logout();
              this.router.navigate(['/login']);
            }
          }
          return throwError(() => error);
        })
      );
    }

    const token = this.authService.getToken();
    console.log(`🔑 [AuthInterceptor] Token exists: ${!!token}`);

    if (token) {
      // Check if token is expired before adding it
      const isExpired = this.isTokenExpired(token);
      console.log(`⏰ [AuthInterceptor] Token expired check: ${isExpired}`);
      
      if (isExpired) {
        console.warn('⚠️ [AuthInterceptor] Token is expired, auto-flushing and redirecting');
        this.authService.logout();
        this.router.navigate(['/login']);
        return throwError(() => new Error('Token expired'));
      }

      console.log(`✅ [AuthInterceptor] Adding Authorization header`);
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        console.log(`❌ [AuthInterceptor] Error caught: ${request.url}, Status: ${error.status}`);
        
        if (error.status === 401) {
          console.log(`🚨 [AuthInterceptor] 401 Unauthorized detected!`);
          const token = this.authService.getToken();
          console.log(`🔑 [AuthInterceptor] Token exists after 401: ${!!token}`);
          
          // Confirm token is expired before flushing
          if (token && this.isTokenExpired(token)) {
            console.warn('⚠️ [AuthInterceptor] 401 error with expired token, auto-flushing');
            this.authService.logout();
            this.router.navigate(['/login']);
          } else if (token) {
            console.log(`⚠️ [AuthInterceptor] 401 but token not expired, still logging out`);
            this.authService.logout();
            this.router.navigate(['/login']);
          } else {
            console.log(`⚠️ [AuthInterceptor] 401 and no token found`);
          }
        }
        return throwError(() => error);
      })
    );
  }

  private isTokenExpired(token: string): boolean {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) {
        return true;
      }

      const payload = JSON.parse(atob(parts[1]));
      const exp = payload.exp;

      if (!exp) {
        return false; // No expiration claim
      }

      // exp is in seconds, Date.now() is in milliseconds
      const expirationTime = exp * 1000;
      const currentTime = Date.now();

      return currentTime >= expirationTime;
    } catch (error) {
      console.error('Error checking token expiration:', error);
      return true; // Treat as expired if we can't parse it
    }
  }
}
