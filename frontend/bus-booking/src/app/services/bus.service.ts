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
  
  // Timing Information
  operatingDays: string;
  pickupTime: string; // HH:mm:ss format
  dropTime: string; // HH:mm:ss format
  journeyDurationHours: number;
  advanceBookingDays: number;
  
  createdAt: string;
  updatedAt: string;
  
  // Schedule information
  schedules?: BusScheduleDto[];
}

export interface BusScheduleDto {
  id: number;
  busId: number;
  scheduleName: string;
  pickupTime: string; // HH:mm:ss format
  dropTime: string; // HH:mm:ss format
  isActive: boolean;
  operatingDays: string;
  effectiveFrom: string;
  effectiveTo: string;
}

export interface AvailableBusesResponse {
  Success: boolean;
  Message: string;
  Buses: BusDto[];
}

export interface BusAvailabilityDto {
  id: number;
  busId: number;
  availableDate: string;
  totalSeats: number;
  availableSeats: number;
  isActive: boolean;
  scheduleId?: number;
  
  // Date-specific timing information
  pickupTime: string; // HH:mm:ss format or TimeSpan format
  dropTime: string; // HH:mm:ss format or TimeSpan format
  journeyDurationHours: number;
  
  createdAt: string;
  updatedAt: string;
}

export interface AvailableDateInfo {
  date: string;
  availableSeats: number;
  pickupTime: string; // HH:mm:ss format
  dropTime: string; // HH:mm:ss format
  journeyDurationHours: number;
  formattedPickupTime: string;
  formattedDropTime: string;
}

export interface AvailableDatesResponse {
  availableDates: AvailableDateInfo[];
  dateAvailability: { [key: string]: AvailableDateInfo };
}

export interface UpdateBusAvailabilityTimingDto {
  busId: number;
  availableDate: string;
  pickupTime: string; // HH:mm:ss format
  dropTime: string; // HH:mm:ss format
  journeyDurationHours: number;
}

export interface BulkUpdateAvailabilityTimingDto {
  busId: number;
  dates: string[];
  pickupTime: string; // HH:mm:ss format
  dropTime: string; // HH:mm:ss format
  journeyDurationHours: number;
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
    let url = `${this.apiUrl}/busavailability/available-dates-with-timing/${busId}`;
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

  updateAvailabilityTiming(updateDto: UpdateBusAvailabilityTimingDto): Observable<any> {
    const url = `${this.apiUrl}/busavailability/update-timing`;
    
    return this.http.put<any>(url, updateDto).pipe(
      catchError(error => this.handleError(error))
    );
  }

  bulkUpdateAvailabilityTiming(bulkUpdateDto: BulkUpdateAvailabilityTimingDto): Observable<any> {
    const url = `${this.apiUrl}/busavailability/bulk-update-timing`;
    
    return this.http.put<any>(url, bulkUpdateDto).pipe(
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

  getBusSchedules(busId: number): Observable<BusScheduleDto[]> {
    const url = `${this.apiUrl}/busschedule/bus/${busId}`;
    
    return this.http.get<BusScheduleDto[]>(url).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getActiveSchedulesForDate(busId: number, date: string): Observable<BusScheduleDto[]> {
    const url = `${this.apiUrl}/busschedule/bus/${busId}/active?date=${date}`;
    
    return this.http.get<BusScheduleDto[]>(url).pipe(
      catchError(error => this.handleError(error))
    );
  }

  formatTime(timeString: string): string {
    if (!timeString) return '';
    
    // Handle .NET TimeSpan format (e.g., "08:00:00" or "08:00:00.0000000")
    // Also handle HH:mm format
    let timeParts: string[];
    
    if (timeString.includes('.')) {
      // Remove milliseconds/ticks if present
      timeString = timeString.split('.')[0];
    }
    
    timeParts = timeString.split(':');
    
    if (timeParts.length >= 2) {
      const hours = parseInt(timeParts[0]);
      const minutes = timeParts[1];
      const ampm = hours >= 12 ? 'PM' : 'AM';
      const displayHours = hours % 12 || 12;
      return `${displayHours}:${minutes} ${ampm}`;
    }
    
    return timeString;
  }

  parseTimeSpan(timeString: string): string {
    if (!timeString) return '00:00:00';
    
    // If already in HH:mm:ss format, return as is
    if (timeString.match(/^\d{2}:\d{2}:\d{2}$/)) {
      return timeString;
    }
    
    // If in .NET TimeSpan format with ticks, remove them
    if (timeString.includes('.')) {
      return timeString.split('.')[0];
    }
    
    return timeString;
  }

  calculateJourneyDuration(pickupTime: string, dropTime: string): string {
    if (!pickupTime || !dropTime) return '';
    
    const pickup = new Date(`2000-01-01T${pickupTime}`);
    const drop = new Date(`2000-01-01T${dropTime}`);
    
    // Handle next day arrival
    if (drop < pickup) {
      drop.setDate(drop.getDate() + 1);
    }
    
    const diffMs = drop.getTime() - pickup.getTime();
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    const diffMinutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (diffHours > 0 && diffMinutes > 0) {
      return `${diffHours}h ${diffMinutes}m`;
    } else if (diffHours > 0) {
      return `${diffHours}h`;
    } else {
      return `${diffMinutes}m`;
    }
  }

  generateBusAvailability(busId: number): Observable<any> {
    const url = `${this.apiUrl}/busavailability/generate/${busId}`;
    
    return this.http.post<any>(url, {}).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getAllBuses(): Observable<BusDto[]> {
    const url = `${this.apiUrl}/bus`;
    
    return this.http.get<BusDto[]>(url).pipe(
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
