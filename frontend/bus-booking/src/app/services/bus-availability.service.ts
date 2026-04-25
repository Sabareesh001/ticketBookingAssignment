import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface BusAvailabilityDto {
  id: number;
  busId: number;
  availableDate: string;
  totalSeats: number;
  availableSeats: number;
  isActive: boolean;
  scheduleId?: number;
  pickupTime?: string;
  dropTime?: string;
  journeyDurationHours?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBusAvailabilityDto {
  busId: number;
  availableDate: string;
  totalSeats: number;
  availableSeats: number;
  isActive: boolean;
  pickupTime?: string;
  dropTime?: string;
  journeyDurationHours?: number;
}

export interface UpdateBusAvailabilityDto {
  busId: number;
  availableDate: string;
  totalSeats: number;
  availableSeats: number;
  isActive: boolean;
  pickupTime?: string;
  dropTime?: string;
  journeyDurationHours?: number;
}

@Injectable({
  providedIn: 'root'
})
export class BusAvailabilityService {
  private apiUrl = 'http://localhost:5266/api/busavailability';

  constructor(private http: HttpClient) {}

  getAllAvailabilities(): Observable<BusAvailabilityDto[]> {
    return this.http.get<BusAvailabilityDto[]>(`${this.apiUrl}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getAvailabilityById(id: number): Observable<BusAvailabilityDto> {
    return this.http.get<BusAvailabilityDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getAvailabilitiesByBus(busId: number): Observable<BusAvailabilityDto[]> {
    return this.http.get<BusAvailabilityDto[]>(`${this.apiUrl}/bus/${busId}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  createAvailability(availability: CreateBusAvailabilityDto): Observable<BusAvailabilityDto> {
    return this.http.post<BusAvailabilityDto>(`${this.apiUrl}`, availability).pipe(
      catchError(error => this.handleError(error))
    );
  }

  updateAvailability(id: number, availability: UpdateBusAvailabilityDto): Observable<BusAvailabilityDto> {
    return this.http.put<BusAvailabilityDto>(`${this.apiUrl}/${id}`, availability).pipe(
      catchError(error => this.handleError(error))
    );
  }

  deleteAvailability(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  generateBusAvailability(busId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/generate/${busId}`, {}).pipe(
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
    return throwError(() => new Error(errorMessage));
  }
}
