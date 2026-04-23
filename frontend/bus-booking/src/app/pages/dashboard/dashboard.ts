import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LocationService, Country, State, District } from '../../services/location.service';
import { BusService, BusDto } from '../../services/bus.service';
import { AuthService } from '../../services/auth.service';

interface SearchCriteria {
  sourceDistrict: string;
  destinationDistrict: string;
  journeyDate: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  fromCountries: Country[] = [];
  fromStates: State[] = [];
  fromDistricts: District[] = [];

  toCountries: Country[] = [];
  toStates: State[] = [];
  toDistricts: District[] = [];

  searchCriteria: SearchCriteria = {
    sourceDistrict: '',
    destinationDistrict: '',
    journeyDate: new Date().toISOString().split('T')[0],
  };

  selectedFromCountry: number | null = null;
  selectedFromState: number | null = null;
  selectedFromDistrict: string = '';

  selectedToCountry: number | null = null;
  selectedToState: number | null = null;
  selectedToDistrict: string = '';

  isToday: boolean = true;
  isLoading: boolean = false;
  errorMessage: string = '';
  isInitializing: boolean = true;

  // Search results
  searchResults: BusDto[] = [];
  hasSearched: boolean = false;
  searchInProgress: boolean = false;

  // Account menu
  showAccountMenu: boolean = false;

  constructor(
    private locationService: LocationService,
    private busService: BusService,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.isInitializing = true;
    this.locationService.getCountries().subscribe({
      next: (countries) => {
        this.fromCountries = countries;
        this.toCountries = countries;
        this.isInitializing = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load countries';
        console.error('Error loading countries:', error);
        this.isInitializing = false;
      }
    });
  }

  onFromCountryChange(countryId: number | null) {
    this.selectedFromCountry = countryId;
    this.selectedFromState = null;
    this.selectedFromDistrict = '';
    this.fromStates = [];
    this.fromDistricts = [];

    if (countryId) {
      this.isLoading = true;
      this.locationService.getStatesByCountry(countryId).subscribe({
        next: (states) => {
          this.fromStates = states;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to load states';
          console.error('Error loading states:', error);
          this.isLoading = false;
        }
      });
    }
  }

  onFromStateChange(stateId: number | null) {
    this.selectedFromState = stateId;
    this.selectedFromDistrict = '';
    this.fromDistricts = [];

    if (stateId) {
      this.isLoading = true;
      this.locationService.getDistrictsByState(stateId).subscribe({
        next: (districts) => {
          this.fromDistricts = districts;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to load districts';
          console.error('Error loading districts:', error);
          this.isLoading = false;
        }
      });
    }
  }

  onToCountryChange(countryId: number | null) {
    this.selectedToCountry = countryId;
    this.selectedToState = null;
    this.selectedToDistrict = '';
    this.toStates = [];
    this.toDistricts = [];

    if (countryId) {
      this.isLoading = true;
      this.locationService.getStatesByCountry(countryId).subscribe({
        next: (states) => {
          this.toStates = states;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to load states';
          console.error('Error loading states:', error);
          this.isLoading = false;
        }
      });
    }
  }

  onToStateChange(stateId: number | null) {
    this.selectedToState = stateId;
    this.selectedToDistrict = '';
    this.toDistricts = [];

    if (stateId) {
      this.isLoading = true;
      this.locationService.getDistrictsByState(stateId).subscribe({
        next: (districts) => {
          this.toDistricts = districts;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to load districts';
          console.error('Error loading districts:', error);
          this.isLoading = false;
        }
      });
    }
  }

  setDateTab(isToday: boolean) {
    this.isToday = isToday;
    const date = new Date();
    if (!isToday) {
      date.setDate(date.getDate() + 1);
    }
    this.searchCriteria.journeyDate = date.toISOString().split('T')[0];
  }

  searchBuses() {
    if (!this.selectedFromDistrict || !this.selectedToDistrict) {
      this.errorMessage = 'Please select both From and To districts';
      return;
    }

    if (this.selectedFromDistrict === this.selectedToDistrict) {
      this.errorMessage = 'From and To districts cannot be the same';
      return;
    }

    this.errorMessage = '';
    this.searchCriteria.sourceDistrict = this.selectedFromDistrict;
    this.searchCriteria.destinationDistrict = this.selectedToDistrict;

    this.searchInProgress = true;
    this.busService.getAvailableBuses(this.selectedFromDistrict, this.selectedToDistrict).subscribe({
      next: (response) => {
        this.searchResults = response.buses;
        this.hasSearched = true;
        this.searchInProgress = false;
        
        if (!response.success || this.searchResults.length === 0) {
          this.errorMessage = response.message || 'No buses found for this route';
        }
      },
      error: (error) => {
        this.errorMessage = 'Failed to search buses: ' + error.message;
        this.searchInProgress = false;
        this.hasSearched = true;
      }
    });
  }

  clearSearch() {
    this.hasSearched = false;
    this.searchResults = [];
    this.errorMessage = '';
  }

  toggleAccountMenu() {
    this.showAccountMenu = !this.showAccountMenu;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
