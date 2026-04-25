import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface Country {
  id: number;
  countryName: string;
  countryCode: string;
}

export interface State {
  id: number;
  stateName: string;
  countryId: number;
}

export interface District {
  id: number;
  districtName: string;
  stateId: number;
}

@Injectable({
  providedIn: 'root'
})
export class GeographicService {
  private apiUrl = 'http://localhost:5266/api';

  constructor(private http: HttpClient) {}

  getCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(`${this.apiUrl}/country`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getStatesByCountry(countryId: number): Observable<State[]> {
    return this.http.get<State[]>(`${this.apiUrl}/state?countryId=${countryId}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getDistrictsByState(stateId: number): Observable<District[]> {
    return this.http.get<District[]>(`${this.apiUrl}/district?stateId=${stateId}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  private handleError(error: any) {
    let errorMessage = 'Failed to load geographic data';
    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else if (error.status) {
      errorMessage = error.error?.message || `Error: ${error.status}`;
    }
    console.error('Geographic Service Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
