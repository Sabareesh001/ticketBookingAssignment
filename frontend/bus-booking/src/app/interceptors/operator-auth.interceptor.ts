import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { OperatorAuthService } from '../services/operator-auth.service';
import { Router } from '@angular/router';

@Injectable()
export class OperatorAuthInterceptor implements HttpInterceptor {
  constructor(private operatorAuthService: OperatorAuthService, private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Only intercept operator-dashboard and operator-auth API calls
    if (request.url.includes('/operator-dashboard') || request.url.includes('/operator-auth')) {
      const token = this.operatorAuthService.getToken();

      console.log(`🔍 [OperatorAuthInterceptor] URL: ${request.url}`);
      console.log(`🔍 [OperatorAuthInterceptor] Token exists: ${!!token}`);
      console.log(`🔍 [OperatorAuthInterceptor] Token value: ${token ? token.substring(0, 20) + '...' : 'null'}`);
      
      if (token) {
        // Check if token is expired before adding it
        if (this.isTokenExpired(token)) {
          console.warn(`⚠️ [OperatorAuthInterceptor] Token is expired, logging out`);
          this.operatorAuthService.logout();
          this.router.navigate(['/operator-login']);
          return throwError(() => new Error('Token expired'));
        }
        
        console.log(`✅ [OperatorAuthInterceptor] Adding Authorization header with token`);
        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        console.log(`✅ [OperatorAuthInterceptor] Request headers after clone:`, request.headers.get('Authorization'));
      } else {
        console.warn(`❌ [OperatorAuthInterceptor] No token found for operator request`);
      }
    } else {
      console.log(`⏭️ [OperatorAuthInterceptor] Skipping non-operator URL: ${request.url}`);
    }

    return next.handle(request).pipe(
      tap(event => {
        if (request.url.includes('/operator-dashboard')) {
          console.log(`✅ [OperatorAuthInterceptor] Response received for ${request.url}`);
        }
      }),
      catchError((error: HttpErrorResponse) => {
        console.log(`❌ [OperatorAuthInterceptor] Error caught for ${request.url}, Status: ${error.status}`);
        
        if (request.url.includes('/operator-dashboard') || request.url.includes('/operator-auth')) {
          console.error(`❌ [OperatorAuthInterceptor] Error for operator URL ${request.url}:`, error);
          
          if (error.status === 401) {
            console.log(`🚨 [OperatorAuthInterceptor] 401 Unauthorized detected!`);
            const token = this.operatorAuthService.getToken();
            console.log(`🔑 [OperatorAuthInterceptor] Token exists after 401: ${!!token}`);
            
            // Confirm token is expired before flushing
            if (token && this.isTokenExpired(token)) {
              console.warn(`⚠️ [OperatorAuthInterceptor] 401 error with expired token, auto-flushing`);
              this.operatorAuthService.logout();
              this.router.navigate(['/operator-login']);
            } else if (token) {
              console.warn(`⚠️ [OperatorAuthInterceptor] 401 but token not expired, still logging out`);
              this.operatorAuthService.logout();
              this.router.navigate(['/operator-login']);
            } else {
              console.warn(`⚠️ [OperatorAuthInterceptor] 401 and no token found`);
            }
          }
        } else {
          console.log(`⏭️ [OperatorAuthInterceptor] Error for non-operator URL, passing through`);
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
