import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import {
  OperatorLoginRequest,
  OperatorSignupRequest,
  OperatorAuthResponse,
  OperatorDto
} from '../models/operator-auth.model';

@Injectable({
  providedIn: 'root'
})
export class OperatorAuthService {
  private apiUrl = 'http://localhost:5266/api';
  private tokenKey = 'operator_auth_token';
  private operatorKey = 'current_operator';

  private currentOperatorSubject = new BehaviorSubject<OperatorDto | null>(this.getOperatorFromStorage());
  public currentOperator$ = this.currentOperatorSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadOperatorFromStorage();
  }

  signup(request: OperatorSignupRequest): Observable<OperatorAuthResponse> {
    return this.http.post<OperatorAuthResponse>(`${this.apiUrl}/operator-auth/signup`, request).pipe(
      tap(response => {
        if (response.token && response.operator) {
          this.setToken(response.token);
          this.setOperator(response.operator);
          this.updateAuthState();
        }
      }),
      catchError(error => this.handleError(error))
    );
  }

  login(request: OperatorLoginRequest): Observable<OperatorAuthResponse> {
    return this.http.post<OperatorAuthResponse>(`${this.apiUrl}/operator-auth/login`, request).pipe(
      tap(response => {
        if (response.token && response.operator) {
          this.setToken(response.token);
          this.setOperator(response.operator);
          this.updateAuthState();
        }
      }),
      catchError(error => this.handleError(error))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.operatorKey);
    this.currentOperatorSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  hasToken(): boolean {
    return !!this.getToken();
  }

  getCurrentOperator(): OperatorDto | null {
    return this.currentOperatorSubject.value;
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private setOperator(operator: OperatorDto): void {
    localStorage.setItem(this.operatorKey, JSON.stringify(operator));
    this.currentOperatorSubject.next(operator);
  }

  private getOperatorFromStorage(): OperatorDto | null {
    const stored = localStorage.getItem(this.operatorKey);
    return stored ? JSON.parse(stored) : null;
  }

  private loadOperatorFromStorage(): void {
    const operator = this.getOperatorFromStorage();
    if (operator) {
      this.currentOperatorSubject.next(operator);
    }
  }

  private updateAuthState(): void {
    this.isAuthenticatedSubject.next(this.hasToken());
  }

  private handleError(error: any) {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else if (error.status === 400) {
      errorMessage = error.error?.message || 'Invalid request';
    } else if (error.status === 401) {
      errorMessage = error.error?.message || 'Unauthorized';
    } else if (error.status === 500) {
      errorMessage = error.error?.message || 'Server error';
    }
    return throwError(() => ({ message: errorMessage }));
  }
}
