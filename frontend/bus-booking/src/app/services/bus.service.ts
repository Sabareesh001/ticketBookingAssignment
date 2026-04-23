import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface BusDto {
  id: number;
  registrationNumber: string;
  operatorId: number;
  operatorName: string;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
  isActive: boolean;
  sourceCity: string;
  destinationCity: string;
  distanceKm?: number;
  estimatedDurationHours?: number;
  createdAt: string;
  updatedAt: string;
}

export interface AvailableBusesResponse {
  success: boolean;
  message: string;
  buses: BusDto[];
}

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private apiUrl = 'http://localhost:5266/api';

  constructor(private http: HttpClient) {}

  getAvailableBuses(sourceDistrict: string, destinationDistrict: string): Observable<AvailableBusesResponse> {
    return this.http.get<AvailableBusesResponse>(
      `${this.apiUrl}/bus/available?sourceDistrict=${sourceDistrict}&destinationDistrict=${destinationDistrict}`
    ).pipe(
      catchError(error => this.handleError(error))
    );
  }

  private handleError(error: any) {
    let errorMessage = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else if (error.status) {
      errorMessage = error.error?.message || `Error: ${error.status}`;
    }

    console.error('Bus Service Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
