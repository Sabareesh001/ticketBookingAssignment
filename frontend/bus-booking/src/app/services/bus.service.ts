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
  Success: boolean;
  Message: string;
  Buses: BusDto[];
}

export interface AvailableDatesResponse {
  availableDates: string[];
  dateAvailability: { [key: string]: number };
}

export interface BusAvailabilityDto {
  id: number;
  busId: number;
  availableDate: string;
  totalSeats: number;
  availableSeats: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class BusService {
  private apiUrl = 'http://localhost:5266/api';

  constructor(private http: HttpClient) {}

  getAvailableBuses(sourceDistrict: string, destinationDistrict: string): Observable<any> {
    const url = `${this.apiUrl}/bus/available?sourceDistrict=${sourceDistrict}&destinationDistrict=${destinationDistrict}`;
    
    return this.http.get<any>(url).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getBusAvailableDates(busId: number, startDate?: string, endDate?: string): Observable<AvailableDatesResponse> {
    let url = `${this.apiUrl}/bus/${busId}/available-dates`;
    const params = [];
    
    if (startDate) params.push(`startDate=${startDate}`);
    if (endDate) params.push(`endDate=${endDate}`);
    
    if (params.length > 0) {
      url += `?${params.join('&')}`;
    }
    
    return this.http.get<AvailableDatesResponse>(url).pipe(
      catchError(error => this.handleError(error))
    );
  }

  checkDateAvailability(busId: number, date: string, requiredSeats: number = 1): Observable<any> {
    const url = `${this.apiUrl}/busavailability/check-availability/${busId}?date=${date}&requiredSeats=${requiredSeats}`;
    
    return this.http.get<any>(url).pipe(
      catchError(error => {
        console.error('Date availability check error:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to check availability'));
      })
    );
  }

  getBusAvailabilityDetails(busId: number, date: string): Observable<BusAvailabilityDto[]> {
    const url = `${this.apiUrl}/busavailability/details/${busId}?date=${date}`;
    
    return this.http.get<BusAvailabilityDto[]>(url).pipe(
      catchError(error => {
        console.error('Bus availability details error:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to get availability details'));
      })
    );
  }

  generateBusAvailability(busId: number): Observable<any> {
    const url = `${this.apiUrl}/bus/${busId}/generate-availability`;
    
    return this.http.post<any>(url, {}).pipe(
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
