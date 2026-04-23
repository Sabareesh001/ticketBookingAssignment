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
  createdAt: string;
  updatedAt: string;
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

  getBookedSeats(busId: number, travelDate: string): Observable<any[]> {
    const url = `${this.apiUrl}/booking/bus/${busId}/seats?travelDate=${travelDate}`;
    return this.http.get<any[]>(url).pipe(
      catchError(error => {
        console.error('Error fetching booked seats:', error);
        // Return empty array if there's an error (no bookings or bus not found)
        return [];
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
