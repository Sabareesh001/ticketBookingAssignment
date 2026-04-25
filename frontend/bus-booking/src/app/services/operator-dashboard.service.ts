import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LocationDto, CreateLocationDto, UpdateLocationDto } from '../models/operator-auth.model';

export interface BusDto {
  id: number;
  registrationNumber: string;
  operatorId: number;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
  isActive: boolean;
  sourceCity?: string;
  destinationCity?: string;
  distanceKm?: number;
  estimatedDurationHours?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBusDto {
  registrationNumber: string;
  operatorId: number;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
}

export interface UpdateBusDto {
  registrationNumber: string;
  operatorId: number;
  routeId: number;
  sourceLocationId: number;
  destinationLocationId: number;
  seatingCapacity?: number;
  price: number;
  isActive: boolean;
}

export interface RouteDto {
  id: number;
  sourceLocationId: number;
  destinationLocationId: number;
  distanceKm?: number;
  estimatedDurationHours?: number;
  createdAt: string;
  updatedAt: string;
}

export interface RouteDetailDto {
  id: number;
  sourceLocationId: number;
  destinationLocationId: number;
  sourceDistrictName: string;
  destinationDistrictName: string;
  distanceKm?: number;
  estimatedDurationHours?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRouteDto {
  sourceLocationId: number;
  destinationLocationId: number;
  distanceKm?: number;
  estimatedDurationHours?: number;
}

export interface UpdateRouteDto {
  sourceLocationId: number;
  destinationLocationId: number;
  distanceKm?: number;
  estimatedDurationHours?: number;
}

@Injectable({
  providedIn: 'root'
})
export class OperatorDashboardService {
  private apiUrl = 'http://localhost:5266/api/operator-dashboard';
  private busApiUrl = 'http://localhost:5266/api/bus';
  private routeApiUrl = 'http://localhost:5266/api/route';

  constructor(private http: HttpClient) {}

  // Location endpoints
  getOperatorLocations(): Observable<LocationDto[]> {
    return this.http.get<LocationDto[]>(`${this.apiUrl}/locations`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getLocationById(id: number): Observable<LocationDto> {
    return this.http.get<LocationDto>(`${this.apiUrl}/locations/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  createLocation(location: CreateLocationDto): Observable<LocationDto> {
    return this.http.post<LocationDto>(`${this.apiUrl}/locations`, location).pipe(
      catchError(error => this.handleError(error))
    );
  }

  updateLocation(id: number, location: UpdateLocationDto): Observable<LocationDto> {
    return this.http.put<LocationDto>(`${this.apiUrl}/locations/${id}`, location).pipe(
      catchError(error => this.handleError(error))
    );
  }

  deleteLocation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/locations/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  // Bus endpoints
  getOperatorBuses(): Observable<BusDto[]> {
    return this.http.get<BusDto[]>(`${this.busApiUrl}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getBusById(id: number): Observable<BusDto> {
    return this.http.get<BusDto>(`${this.busApiUrl}/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  createBus(bus: CreateBusDto): Observable<BusDto> {
    return this.http.post<BusDto>(`${this.busApiUrl}`, bus).pipe(
      catchError(error => this.handleError(error))
    );
  }

  updateBus(id: number, bus: UpdateBusDto): Observable<BusDto> {
    return this.http.put<BusDto>(`${this.busApiUrl}/${id}`, bus).pipe(
      catchError(error => this.handleError(error))
    );
  }

  deleteBus(id: number): Observable<void> {
    return this.http.delete<void>(`${this.busApiUrl}/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  // Route endpoints
  getAllRoutes(): Observable<RouteDto[]> {
    return this.http.get<RouteDto[]>(`${this.routeApiUrl}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getAllRouteDetails(): Observable<RouteDetailDto[]> {
    return this.http.get<RouteDetailDto[]>(`${this.routeApiUrl}/details/all`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getRouteById(id: number): Observable<RouteDto> {
    return this.http.get<RouteDto>(`${this.routeApiUrl}/${id}`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  getRouteDetailById(id: number): Observable<RouteDetailDto> {
    return this.http.get<RouteDetailDto>(`${this.routeApiUrl}/${id}/details`).pipe(
      catchError(error => this.handleError(error))
    );
  }

  createRoute(route: CreateRouteDto): Observable<RouteDto> {
    return this.http.post<RouteDto>(`${this.routeApiUrl}`, route).pipe(
      catchError(error => this.handleError(error))
    );
  }

  updateRoute(id: number, route: UpdateRouteDto): Observable<RouteDto> {
    return this.http.put<RouteDto>(`${this.routeApiUrl}/${id}`, route).pipe(
      catchError(error => this.handleError(error))
    );
  }

  deleteRoute(id: number): Observable<void> {
    return this.http.delete<void>(`${this.routeApiUrl}/${id}`).pipe(
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
