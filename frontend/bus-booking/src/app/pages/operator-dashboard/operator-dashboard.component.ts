import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { OperatorAuthService } from '../../services/operator-auth.service';
import { OperatorDashboardService, BusDto, CreateBusDto, UpdateBusDto, RouteDetailDto } from '../../services/operator-dashboard.service';
import { LocationService, Country, State, District } from '../../services/location.service';
import { LocationDto, CreateLocationDto, UpdateLocationDto } from '../../models/operator-auth.model';
import { BusAvailabilityService, BusAvailabilityDto, CreateBusAvailabilityDto, UpdateBusAvailabilityDto } from '../../services/bus-availability.service';

interface PaginatedLocations {
  items: LocationDto[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

interface PaginatedBuses {
  items: BusDto[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

interface PaginatedAvailability {
  items: BusAvailabilityDto[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

@Component({
  selector: 'app-operator-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './operator-dashboard.component.html',
  styleUrl: './operator-dashboard.component.css'
})
export class OperatorDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Tab management
  activeTab: 'locations' | 'buses' | 'availability' = 'locations';

  // Locations Manager
  locations: LocationDto[] = [];
  paginatedLocations: PaginatedLocations = {
    items: [],
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  };

  // Buses Manager
  buses: BusDto[] = [];
  paginatedBuses: PaginatedBuses = {
    items: [],
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  };

  // Availability Manager
  availabilities: BusAvailabilityDto[] = [];
  filteredAvailabilities: BusAvailabilityDto[] = [];
  paginatedAvailability: PaginatedAvailability = {
    items: [],
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  };
  
  // Availability filters
  availabilityFilters = {
    busId: '',
    status: '',
    dateFrom: '',
    dateTo: ''
  };
  availabilitySearchTerm = '';

  // Modal state
  showLocationModal = false;
  showBusModal = false;
  showAvailabilityModal = false;
  isEditMode = false;
  selectedLocationId: number | null = null;
  selectedBusId: number | null = null;
  selectedAvailabilityId: number | null = null;

  // Forms
  locationForm: FormGroup;
  busForm: FormGroup;
  availabilityForm: FormGroup;

  // Dropdowns
  countries: Country[] = [];
  states: State[] = [];
  districts: District[] = [];
  routes: RouteDetailDto[] = [];
  operatorLocations: LocationDto[] = [];
  
  // Cache for all states and districts
  allStates: Map<number, State[]> = new Map();
  allDistricts: Map<number, District[]> = new Map();
  
  // Cache for location details and districts
  locationDetailsCache: Map<number, LocationDto> = new Map();
  districtCache: Map<number, District> = new Map();

  // Loading and error states
  isLoading = false;
  isLoadingCountries = false;
  isLoadingStates = false;
  isLoadingDistricts = false;
  isLoadingRoutes = false;
  isLoadingLocations = false;
  isLoadingAvailability = false;
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  constructor(
    private operatorAuthService: OperatorAuthService,
    private operatorDashboardService: OperatorDashboardService,
    private locationService: LocationService,
    private busAvailabilityService: BusAvailabilityService,
    private formBuilder: FormBuilder,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.locationForm = this.formBuilder.group({
      streetAddress: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      postalCode: ['', [Validators.required, Validators.pattern(/^\d{5,10}$/)]],
      countryId: ['', Validators.required],
      stateId: ['', Validators.required],
      districtId: ['', Validators.required],
      latitude: ['', Validators.pattern(/^-?\d+(\.\d+)?$/)],
      longitude: ['', Validators.pattern(/^-?\d+(\.\d+)?$/)]
    });

    this.busForm = this.formBuilder.group({
      registrationNumber: ['', [Validators.required, Validators.minLength(3)]],
      routeId: ['', Validators.required],
      seatingCapacity: ['', [Validators.required, Validators.min(1), Validators.max(100)]],
      price: ['', [Validators.required, Validators.min(0)]],
      isActive: [true]
    });

    this.availabilityForm = this.formBuilder.group({
      busId: ['', Validators.required],
      availableDate: ['', Validators.required],
      totalSeats: ['', [Validators.required, Validators.min(1), Validators.max(100)]],
      availableSeats: ['', [Validators.required, Validators.min(0)]],
      pickupTime: [''],
      dropTime: [''],
      journeyDurationHours: ['', [Validators.min(0)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCountries();
    this.loadLocations();
    this.loadBuses();
    this.loadRoutes();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Location Management
  loadLocations(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.operatorDashboardService.getOperatorLocations()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.locations = data;
          this.operatorLocations = data;
          this.paginatedLocations = {
            items: [],
            currentPage: 1,
            pageSize: 10,
            totalItems: this.locations.length,
            totalPages: Math.ceil(this.locations.length / 10)
          };
          this.updateCurrentPageItems();
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to load locations';
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  updateCurrentPageItems(): void {
    const startIndex = (this.paginatedLocations.currentPage - 1) * this.paginatedLocations.pageSize;
    const endIndex = startIndex + this.paginatedLocations.pageSize;
    this.paginatedLocations = {
      ...this.paginatedLocations,
      items: this.locations.slice(startIndex, endIndex)
    };
    this.cdr.markForCheck();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.paginatedLocations.totalPages) {
      this.paginatedLocations = {
        ...this.paginatedLocations,
        currentPage: page
      };
      this.updateCurrentPageItems();
    }
  }

  // Modal Management
  openCreateModal(): void {
    this.isEditMode = false;
    this.selectedLocationId = null;
    this.locationForm.reset();
    this.showLocationModal = true;
    this.errorMessage = '';
  }

  openEditModal(location: LocationDto): void {
    this.isEditMode = true;
    this.selectedLocationId = location.id;
    this.locationForm.patchValue({
      streetAddress: location.streetAddress,
      city: location.city,
      postalCode: location.postalCode,
      countryId: location.countryId,
      stateId: location.stateId,
      districtId: location.districtId,
      latitude: location.latitude || '',
      longitude: location.longitude || ''
    });
    this.errorMessage = '';

    // Load states and districts from cache
    this.states = this.allStates.get(location.countryId) || [];
    this.districts = this.allDistricts.get(location.stateId) || [];
    this.showLocationModal = true;
    this.cdr.markForCheck();
  }

  closeModal(): void {
    this.showLocationModal = false;
    this.locationForm.reset();
    this.errorMessage = '';
  }

  // Form Handlers
  onCountryChange(eventOrId: Event | number): void {
    let countryId: number;
    
    if (eventOrId instanceof Event) {
      countryId = parseInt((eventOrId.target as HTMLSelectElement).value);
    } else {
      countryId = eventOrId;
    }

    if (!countryId) {
      this.states = [];
      this.districts = [];
      this.locationForm.patchValue({ stateId: '', districtId: '' });
      this.cdr.markForCheck();
      return;
    }

    // Use cached states
    this.states = this.allStates.get(countryId) || [];
    this.districts = [];
    this.locationForm.patchValue({ stateId: '', districtId: '' });
    this.cdr.markForCheck();
  }

  onStateChange(eventOrId: Event | number): void {
    let stateId: number;
    
    if (eventOrId instanceof Event) {
      stateId = parseInt((eventOrId.target as HTMLSelectElement).value);
    } else {
      stateId = eventOrId;
    }

    if (!stateId) {
      this.districts = [];
      this.locationForm.patchValue({ districtId: '' });
      this.cdr.markForCheck();
      return;
    }

    // Use cached districts
    this.districts = this.allDistricts.get(stateId) || [];
    this.locationForm.patchValue({ districtId: '' });
    this.cdr.markForCheck();
  }

  saveLocation(): void {
    if (!this.locationForm.valid) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const formValue = this.locationForm.value;
    const locationData = {
      streetAddress: formValue.streetAddress,
      city: formValue.city,
      postalCode: formValue.postalCode,
      countryId: parseInt(formValue.countryId),
      stateId: parseInt(formValue.stateId),
      districtId: parseInt(formValue.districtId),
      latitude: formValue.latitude ? parseFloat(formValue.latitude) : undefined,
      longitude: formValue.longitude ? parseFloat(formValue.longitude) : undefined
    };

    if (this.isEditMode && this.selectedLocationId) {
      this.operatorDashboardService.updateLocation(this.selectedLocationId, locationData as UpdateLocationDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Location updated successfully';
            this.closeModal();
            this.loadLocations();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to update location';
            this.isSubmitting = false;
          }
        });
    } else {
      this.operatorDashboardService.createLocation(locationData as CreateLocationDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Location created successfully';
            this.closeModal();
            this.loadLocations();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to create location';
            this.isSubmitting = false;
          }
        });
    }
  }

  deleteLocation(id: number): void {
    if (!confirm('Are you sure you want to delete this location?')) {
      return;
    }

    this.operatorDashboardService.deleteLocation(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Location deleted successfully';
          this.loadLocations();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to delete location';
        }
      });
  }

  // Dropdown Loading
  loadCountries(): void {
    this.isLoadingCountries = true;
    this.locationService.getCountries()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.countries = data;
          this.isLoadingCountries = false;
          this.cdr.markForCheck();
          
          // Load all states and districts for all countries
          this.loadAllStatesAndDistricts();
        },
        error: (error) => {
          this.errorMessage = 'Failed to load countries';
          this.isLoadingCountries = false;
          this.cdr.markForCheck();
        }
      });
  }

  private loadAllStatesAndDistricts(): void {
    // Load states for each country
    this.countries.forEach(country => {
      this.locationService.getStatesByCountry(country.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (statesData) => {
            this.allStates.set(country.id, statesData);
            
            // Load districts for each state
            statesData.forEach(state => {
              this.locationService.getDistrictsByState(state.id)
                .pipe(takeUntil(this.destroy$))
                .subscribe({
                  next: (districtsData) => {
                    this.allDistricts.set(state.id, districtsData);
                    this.cdr.markForCheck();
                  },
                  error: (error) => {
                    console.error('Failed to load districts for state', state.id);
                  }
                });
            });
          },
          error: (error) => {
            console.error('Failed to load states for country', country.id);
          }
        });
    });
  }

  // Logout
  logout(): void {
    this.operatorAuthService.logout();
    this.router.navigate(['/login']);
  }

  // Bus Management
  loadBuses(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.operatorDashboardService.getOperatorBuses()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.buses = data;
          this.paginatedBuses = {
            items: [],
            currentPage: 1,
            pageSize: 10,
            totalItems: this.buses.length,
            totalPages: Math.ceil(this.buses.length / 10)
          };
          this.updateBusPageItems();
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to load buses';
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  updateBusPageItems(): void {
    const startIndex = (this.paginatedBuses.currentPage - 1) * this.paginatedBuses.pageSize;
    const endIndex = startIndex + this.paginatedBuses.pageSize;
    this.paginatedBuses = {
      ...this.paginatedBuses,
      items: this.buses.slice(startIndex, endIndex)
    };
    this.cdr.markForCheck();
  }

  goToBusPage(page: number): void {
    if (page >= 1 && page <= this.paginatedBuses.totalPages) {
      this.paginatedBuses = {
        ...this.paginatedBuses,
        currentPage: page
      };
      this.updateBusPageItems();
    }
  }

  openCreateBusModal(): void {
    this.isEditMode = false;
    this.selectedBusId = null;
    this.busForm.reset({ isActive: true });
    this.showBusModal = true;
    this.errorMessage = '';
  }

  openEditBusModal(bus: BusDto): void {
    this.isEditMode = true;
    this.selectedBusId = bus.id;
    this.busForm.patchValue({
      registrationNumber: bus.registrationNumber,
      routeId: bus.routeId,
      seatingCapacity: bus.seatingCapacity,
      price: bus.price,
      isActive: bus.isActive
    });
    this.errorMessage = '';
    this.showBusModal = true;
    this.cdr.markForCheck();
  }

  closeBusModal(): void {
    this.showBusModal = false;
    this.busForm.reset();
    this.errorMessage = '';
  }

  saveBus(): void {
    if (!this.busForm.valid) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const formValue = this.busForm.value;
    const route = this.routes.find(r => r.id === parseInt(formValue.routeId));
    
    if (!route) {
      this.errorMessage = 'Invalid route selected';
      this.isSubmitting = false;
      return;
    }

    const busData = {
      registrationNumber: formValue.registrationNumber,
      operatorId: this.operatorAuthService.getOperatorId(),
      routeId: parseInt(formValue.routeId),
      sourceLocationId: route.sourceLocationId,
      destinationLocationId: route.destinationLocationId,
      seatingCapacity: parseInt(formValue.seatingCapacity),
      price: parseFloat(formValue.price),
      isActive: formValue.isActive
    };

    if (this.isEditMode && this.selectedBusId) {
      this.operatorDashboardService.updateBus(this.selectedBusId, busData as UpdateBusDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Bus updated successfully';
            this.closeBusModal();
            this.loadBuses();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to update bus';
            this.isSubmitting = false;
          }
        });
    } else {
      this.operatorDashboardService.createBus(busData as CreateBusDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Bus created successfully';
            this.closeBusModal();
            this.loadBuses();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to create bus';
            this.isSubmitting = false;
          }
        });
    }
  }

  deleteBus(id: number): void {
    if (!confirm('Are you sure you want to delete this bus?')) {
      return;
    }

    this.operatorDashboardService.deleteBus(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Bus deleted successfully';
          this.loadBuses();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to delete bus';
        }
      });
  }

  loadRoutes(): void {
    this.isLoadingRoutes = true;
    this.operatorDashboardService.getAllRouteDetails()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.routes = data;
          this.isLoadingRoutes = false;
          this.cdr.markForCheck();
        },
        error: (error) => {
          console.error('Failed to load routes:', error);
          this.isLoadingRoutes = false;
          this.cdr.markForCheck();
        }
      });
  }

  getRouteName(routeId: number): string {
    const route = this.routes.find(r => r.id === routeId);
    if (!route) return 'N/A';
    
    return `${route.sourceDistrictName} → ${route.destinationDistrictName}`;
  }

  // Utility
  getCountryName(countryId: number): string {
    return this.countries.find(c => c.id === countryId)?.countryName || 'N/A';
  }

  getStateName(stateId: number): string {
    // Search in all cached states
    for (const statesArray of this.allStates.values()) {
      const state = statesArray.find(s => s.id === stateId);
      if (state) return state.stateName;
    }
    return 'N/A';
  }

  getDistrictName(districtId: number): string {
    // Search in all cached districts
    for (const districtsArray of this.allDistricts.values()) {
      const district = districtsArray.find(d => d.id === districtId);
      if (district) return district.districtName;
    }
    return 'N/A';
  }

  getDistrictNameFromLocationId(locationId: number): string {
    const location = this.operatorLocations.find(l => l.id === locationId);
    if (!location) return 'Unknown';
    
    // Get district from cache
    const district = this.getDistrictName(location.districtId);
    return district !== 'N/A' ? district : 'Unknown';
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  // Bus Availability Management
  loadAvailability(): void {
    this.isLoadingAvailability = true;
    this.errorMessage = '';
    this.busAvailabilityService.getAllAvailabilities()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          // Sort by date descending (most recent first)
          this.availabilities = data.sort((a, b) => 
            new Date(b.availableDate).getTime() - new Date(a.availableDate).getTime()
          );
          this.applyAvailabilityFilters();
          this.isLoadingAvailability = false;
          this.cdr.markForCheck();
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to load availability records';
          this.isLoadingAvailability = false;
          this.cdr.markForCheck();
        }
      });
  }

  applyAvailabilityFilters(): void {
    let filtered = [...this.availabilities];

    // Filter by bus
    if (this.availabilityFilters.busId) {
      const busId = parseInt(this.availabilityFilters.busId);
      filtered = filtered.filter(a => a.busId === busId);
    }

    // Filter by status
    if (this.availabilityFilters.status) {
      const isActive = this.availabilityFilters.status === 'active';
      filtered = filtered.filter(a => a.isActive === isActive);
    }

    // Filter by date range
    if (this.availabilityFilters.dateFrom) {
      const fromDate = new Date(this.availabilityFilters.dateFrom);
      filtered = filtered.filter(a => new Date(a.availableDate) >= fromDate);
    }

    if (this.availabilityFilters.dateTo) {
      const toDate = new Date(this.availabilityFilters.dateTo);
      filtered = filtered.filter(a => new Date(a.availableDate) <= toDate);
    }

    // Search by bus registration
    if (this.availabilitySearchTerm.trim()) {
      const searchLower = this.availabilitySearchTerm.toLowerCase().trim();
      filtered = filtered.filter(a => {
        const busReg = this.getBusRegistration(a.busId).toLowerCase();
        const route = this.getBusRoute(a.busId).toLowerCase();
        return busReg.includes(searchLower) || route.includes(searchLower);
      });
    }

    this.filteredAvailabilities = filtered;
    this.paginatedAvailability = {
      items: [],
      currentPage: 1,
      pageSize: 10,
      totalItems: this.filteredAvailabilities.length,
      totalPages: Math.ceil(this.filteredAvailabilities.length / 10)
    };
    this.updateAvailabilityPageItems();
  }

  clearAvailabilityFilters(): void {
    this.availabilityFilters = {
      busId: '',
      status: '',
      dateFrom: '',
      dateTo: ''
    };
    this.availabilitySearchTerm = '';
    this.applyAvailabilityFilters();
  }

  updateAvailabilityPageItems(): void {
    const startIndex = (this.paginatedAvailability.currentPage - 1) * this.paginatedAvailability.pageSize;
    const endIndex = startIndex + this.paginatedAvailability.pageSize;
    this.paginatedAvailability = {
      ...this.paginatedAvailability,
      items: this.filteredAvailabilities.slice(startIndex, endIndex)
    };
    this.cdr.markForCheck();
  }

  goToAvailabilityPage(page: number): void {
    if (page >= 1 && page <= this.paginatedAvailability.totalPages) {
      this.paginatedAvailability = {
        ...this.paginatedAvailability,
        currentPage: page
      };
      this.updateAvailabilityPageItems();
    }
  }

  openCreateAvailabilityModal(): void {
    this.isEditMode = false;
    this.selectedAvailabilityId = null;
    this.availabilityForm.reset({ isActive: true });
    this.showAvailabilityModal = true;
    this.errorMessage = '';
    
    // Set up listener for bus selection to auto-populate seats
    this.availabilityForm.get('busId')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(busId => {
        if (busId) {
          const selectedBus = this.buses.find(b => b.id === parseInt(busId));
          if (selectedBus && selectedBus.seatingCapacity) {
            this.availabilityForm.patchValue({
              totalSeats: selectedBus.seatingCapacity,
              availableSeats: selectedBus.seatingCapacity
            });
          }
        }
      });
  }

  openEditAvailabilityModal(availability: BusAvailabilityDto): void {
    this.isEditMode = true;
    this.selectedAvailabilityId = availability.id;
    
    // Format date for input[type="date"]
    const dateStr = new Date(availability.availableDate).toISOString().split('T')[0];
    
    // Format time for input[type="time"] (HH:mm format)
    const pickupTime = availability.pickupTime ? this.formatTimeForInput(availability.pickupTime) : '';
    const dropTime = availability.dropTime ? this.formatTimeForInput(availability.dropTime) : '';
    
    this.availabilityForm.patchValue({
      busId: availability.busId,
      availableDate: dateStr,
      totalSeats: availability.totalSeats,
      availableSeats: availability.availableSeats,
      pickupTime: pickupTime,
      dropTime: dropTime,
      journeyDurationHours: availability.journeyDurationHours || '',
      isActive: availability.isActive
    });
    this.errorMessage = '';
    this.showAvailabilityModal = true;
    this.cdr.markForCheck();
  }

  closeAvailabilityModal(): void {
    this.showAvailabilityModal = false;
    this.availabilityForm.reset();
    this.errorMessage = '';
  }

  saveAvailability(): void {
    if (!this.availabilityForm.valid) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    // Validate available seats <= total seats
    const totalSeats = parseInt(this.availabilityForm.value.totalSeats);
    const availableSeats = parseInt(this.availabilityForm.value.availableSeats);
    
    if (availableSeats > totalSeats) {
      this.errorMessage = 'Available seats cannot exceed total seats';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const formValue = this.availabilityForm.value;
    
    // Convert time strings to TimeSpan format (HH:mm:ss)
    const pickupTime = formValue.pickupTime ? `${formValue.pickupTime}:00` : undefined;
    const dropTime = formValue.dropTime ? `${formValue.dropTime}:00` : undefined;
    
    const availabilityData = {
      busId: parseInt(formValue.busId),
      availableDate: formValue.availableDate,
      totalSeats: totalSeats,
      availableSeats: availableSeats,
      isActive: formValue.isActive,
      pickupTime: pickupTime,
      dropTime: dropTime,
      journeyDurationHours: formValue.journeyDurationHours ? parseFloat(formValue.journeyDurationHours) : undefined
    };

    if (this.isEditMode && this.selectedAvailabilityId) {
      this.busAvailabilityService.updateAvailability(this.selectedAvailabilityId, availabilityData as UpdateBusAvailabilityDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Availability updated successfully';
            this.closeAvailabilityModal();
            this.loadAvailability();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to update availability';
            this.isSubmitting = false;
          }
        });
    } else {
      this.busAvailabilityService.createAvailability(availabilityData as CreateBusAvailabilityDto)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.successMessage = 'Availability created successfully';
            this.closeAvailabilityModal();
            this.loadAvailability();
            this.isSubmitting = false;
            setTimeout(() => this.successMessage = '', 3000);
          },
          error: (error) => {
            this.errorMessage = error.message || 'Failed to create availability';
            this.isSubmitting = false;
          }
        });
    }
  }

  deleteAvailability(id: number): void {
    if (!confirm('Are you sure you want to delete this availability record?')) {
      return;
    }

    this.busAvailabilityService.deleteAvailability(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Availability deleted successfully';
          this.loadAvailability();
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = error.message || 'Failed to delete availability';
        }
      });
  }

  getBusRegistration(busId: number): string {
    const bus = this.buses.find(b => b.id === busId);
    return bus ? bus.registrationNumber : 'N/A';
  }

  getBusRoute(busId: number): string {
    const bus = this.buses.find(b => b.id === busId);
    if (!bus) return 'N/A';
    return `${bus.sourceCity} → ${bus.destinationCity}`;
  }

  formatTime(timeString: string | undefined): string {
    if (!timeString) return 'N/A';
    
    // Handle TimeSpan format (HH:mm:ss or HH:mm:ss.fffffff)
    let timeParts: string[];
    
    if (timeString.includes('.')) {
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

  formatTimeForInput(timeString: string): string {
    if (!timeString) return '';
    
    // Handle TimeSpan format and convert to HH:mm for input[type="time"]
    if (timeString.includes('.')) {
      timeString = timeString.split('.')[0];
    }
    
    const timeParts = timeString.split(':');
    if (timeParts.length >= 2) {
      return `${timeParts[0]}:${timeParts[1]}`;
    }
    
    return timeString;
  }

  getTodayDate(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }
}
