import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { OperatorAuthService } from '../../services/operator-auth.service';
import { BusDto, LocationDto, CreateBusDto, UpdateBusDto, CreateLocationDto, UpdateLocationDto } from '../../models/operator-auth.model';

@Component({
  selector: 'app-operator-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './operator-dashboard.component.html',
  styleUrls: ['./operator-dashboard.component.css']
})
export class OperatorDashboardComponent implements OnInit {
  activeTab: 'buses' | 'locations' = 'buses';
  
  buses: BusDto[] = [];
  locations: LocationDto[] = [];
  
  busForm!: FormGroup;
  locationForm!: FormGroup;
  
  loading = false;
  submitted = false;
  error = '';
  successMessage = '';
  
  editingBusId: number | null = null;
  editingLocationId: number | null = null;
  
  showBusForm = false;
  showLocationForm = false;

  private apiUrl = 'http://localhost:5266/api';

  constructor(
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private operatorAuthService: OperatorAuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.loadBuses();
    this.loadLocations();
  }

  private initializeForms(): void {
    this.busForm = this.formBuilder.group({
      registrationNumber: ['', [Validators.required, Validators.minLength(3)]],
      routeId: ['', Validators.required],
      sourceLocationId: ['', Validators.required],
      destinationLocationId: ['', Validators.required],
      seatingCapacity: ['', [Validators.required, Validators.min(1)]],
      price: ['', [Validators.required, Validators.min(0)]],
      isActive: [true]
    });

    this.locationForm = this.formBuilder.group({
      streetAddress: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      districtId: ['', Validators.required],
      stateId: ['', Validators.required],
      countryId: ['', Validators.required],
      postalCode: ['', [Validators.required, Validators.minLength(3)]],
      latitude: [''],
      longitude: ['']
    });
  }

  private getHeaders(): HttpHeaders {
    const token = this.operatorAuthService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  loadBuses(): void {
    this.loading = true;
    this.http.get<BusDto[]>(`${this.apiUrl}/operator-dashboard/buses`, { headers: this.getHeaders() })
      .subscribe({
        next: (data) => {
          this.buses = data;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load buses';
          this.loading = false;
        }
      });
  }

  loadLocations(): void {
    this.loading = true;
    this.http.get<LocationDto[]>(`${this.apiUrl}/operator-dashboard/locations`, { headers: this.getHeaders() })
      .subscribe({
        next: (data) => {
          this.locations = data;
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Failed to load locations';
          this.loading = false;
        }
      });
  }

  switchTab(tab: 'buses' | 'locations'): void {
    this.activeTab = tab;
    this.showBusForm = false;
    this.showLocationForm = false;
    this.error = '';
    this.successMessage = '';
    
    // Reload data when switching tabs
    if (tab === 'buses') {
      this.loadBuses();
    } else {
      this.loadLocations();
    }
  }

  // Bus Management
  toggleBusForm(): void {
    this.showBusForm = !this.showBusForm;
    this.editingBusId = null;
    this.busForm.reset({ isActive: true });
    this.submitted = false;
  }

  editBus(bus: BusDto): void {
    this.editingBusId = bus.id;
    this.showBusForm = true;
    this.busForm.patchValue({
      registrationNumber: bus.registrationNumber,
      routeId: bus.routeId,
      sourceLocationId: bus.sourceLocationId,
      destinationLocationId: bus.destinationLocationId,
      seatingCapacity: bus.seatingCapacity,
      price: bus.price,
      isActive: bus.isActive
    });
  }

  saveBus(): void {
    this.submitted = true;
    this.error = '';
    this.successMessage = '';

    if (this.busForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.busForm.value;

    if (this.editingBusId) {
      const updateDto: UpdateBusDto = {
        registrationNumber: formValue.registrationNumber,
        routeId: formValue.routeId,
        sourceLocationId: formValue.sourceLocationId,
        destinationLocationId: formValue.destinationLocationId,
        seatingCapacity: formValue.seatingCapacity,
        price: formValue.price,
        isActive: formValue.isActive
      };

      this.http.put(`${this.apiUrl}/operator-dashboard/buses/${this.editingBusId}`, updateDto, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Bus updated successfully';
            this.loadBuses();
            this.toggleBusForm();
            this.loading = false;
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to update bus';
            this.loading = false;
          }
        });
    } else {
      const createDto: CreateBusDto = {
        registrationNumber: formValue.registrationNumber,
        operatorId: this.operatorAuthService.getCurrentOperator()?.id || 0,
        routeId: formValue.routeId,
        sourceLocationId: formValue.sourceLocationId,
        destinationLocationId: formValue.destinationLocationId,
        seatingCapacity: formValue.seatingCapacity,
        price: formValue.price
      };

      this.http.post(`${this.apiUrl}/operator-dashboard/buses`, createDto, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Bus created successfully';
            this.loadBuses();
            this.toggleBusForm();
            this.loading = false;
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to create bus';
            this.loading = false;
          }
        });
    }
  }

  deleteBus(id: number): void {
    if (confirm('Are you sure you want to delete this bus?')) {
      this.http.delete(`${this.apiUrl}/operator-dashboard/buses/${id}`, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Bus deleted successfully';
            this.loadBuses();
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to delete bus';
          }
        });
    }
  }

  // Location Management
  toggleLocationForm(): void {
    this.showLocationForm = !this.showLocationForm;
    this.editingLocationId = null;
    this.locationForm.reset();
    this.submitted = false;
  }

  editLocation(location: LocationDto): void {
    this.editingLocationId = location.id;
    this.showLocationForm = true;
    this.locationForm.patchValue({
      streetAddress: location.streetAddress,
      city: location.city,
      districtId: location.districtId,
      stateId: location.stateId,
      countryId: location.countryId,
      postalCode: location.postalCode,
      latitude: location.latitude,
      longitude: location.longitude
    });
  }

  saveLocation(): void {
    this.submitted = true;
    this.error = '';
    this.successMessage = '';

    if (this.locationForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.locationForm.value;

    if (this.editingLocationId) {
      const updateDto: UpdateLocationDto = {
        streetAddress: formValue.streetAddress,
        city: formValue.city,
        districtId: formValue.districtId,
        stateId: formValue.stateId,
        countryId: formValue.countryId,
        postalCode: formValue.postalCode,
        latitude: formValue.latitude,
        longitude: formValue.longitude,
        operatorId: this.operatorAuthService.getCurrentOperator()?.id
      };

      this.http.put(`${this.apiUrl}/operator-dashboard/locations/${this.editingLocationId}`, updateDto, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Location updated successfully';
            this.loadLocations();
            this.toggleLocationForm();
            this.loading = false;
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to update location';
            this.loading = false;
          }
        });
    } else {
      const createDto: CreateLocationDto = {
        streetAddress: formValue.streetAddress,
        city: formValue.city,
        districtId: formValue.districtId,
        stateId: formValue.stateId,
        countryId: formValue.countryId,
        postalCode: formValue.postalCode,
        latitude: formValue.latitude,
        longitude: formValue.longitude,
        operatorId: this.operatorAuthService.getCurrentOperator()?.id
      };

      this.http.post(`${this.apiUrl}/operator-dashboard/locations`, createDto, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Location created successfully';
            this.loadLocations();
            this.toggleLocationForm();
            this.loading = false;
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to create location';
            this.loading = false;
          }
        });
    }
  }

  deleteLocation(id: number): void {
    if (confirm('Are you sure you want to delete this location?')) {
      this.http.delete(`${this.apiUrl}/operator-dashboard/locations/${id}`, { headers: this.getHeaders() })
        .subscribe({
          next: () => {
            this.successMessage = 'Location deleted successfully';
            this.loadLocations();
          },
          error: (error) => {
            this.error = error.error?.message || 'Failed to delete location';
          }
        });
    }
  }

  logout(): void {
    this.operatorAuthService.logout();
    this.router.navigate(['/login']);
  }

  get busFormControls() {
    return this.busForm.controls;
  }

  get locationFormControls() {
    return this.locationForm.controls;
  }
}
