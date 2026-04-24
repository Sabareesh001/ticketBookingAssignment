import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface CreateBookingRequest {
  userId: number;
  busId: number;
  travelDate: string; // ISO string format
  seatNumbers: string;
  totalFare: number;
  pickupLocationId?: number | null;
  dropLocationId?: number | null;
  scheduleId?: number | null;
}

export interface BookingResponse {
  id: number;
  userId: number;
  busId: number;
  bookingDate: string;
  travelDate: string;
  seatNumbers: string;
  totalFare: number;
  bookingStatus: string;
  paymentStatus: string;
  travelStatus: string;
  pickupLocationId?: number;
  dropLocationId?: number;
  pickupTime?: string;
  dropTime?: string;
  scheduleId?: number;
  pickupLocationName?: string;
  dropLocationName?: string;
  createdAt: string;
  updatedAt: string;
}

export interface ReserveSeatRequest {
  userId: number;
  busId: number;
  travelDate: string;
  seatNumbers: string;
}

export interface ReserveSeatResponse {
  reservationId: number;
  seatNumbers: string;
  reservedUntil: string;
  remainingSeconds: number;
}

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private apiUrl = 'http://localhost:5266/api';

  constructor(private http: HttpClient) {}

  createBooking(bookingData: CreateBookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.apiUrl}/booking`, bookingData).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getBookedSeats(busId: number, travelDate: string): Observable<any> {
    const url = `${this.apiUrl}/booking/bus/${busId}/seats?travelDate=${travelDate}`;
    return this.http.get<any>(url).pipe(
      catchError(error => {
        console.error('Error fetching booked seats:', error);
        // Return empty object if there's an error
        return throwError(() => ({ confirmedBookings: [], reservedSeats: [] }));
      })
    );
  }

  reserveSeats(reserveData: ReserveSeatRequest): Observable<ReserveSeatResponse> {
    return this.http.post<ReserveSeatResponse>(`${this.apiUrl}/booking/reserve`, reserveData).pipe(
      catchError(error => this.handleError(error))
    );
  }

  releaseReservation(reservationId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/booking/reserve/${reservationId}`).pipe(
      catchError(error => {
        console.error('Error releasing reservation:', error);
        return throwError(() => new Error('Failed to release reservation'));
      })
    );
  }

  private handleError(error: any) {
    let errorMessage = 'An error occurred';

    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else if (error.status) {
      errorMessage = error.error?.message || `Error: ${error.status}`;
    }

    console.error('Booking Service Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
