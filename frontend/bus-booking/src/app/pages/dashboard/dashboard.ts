import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LocationService, Country, State, District } from '../../services/location.service';
import { BusService, BusDto, AvailableDatesResponse } from '../../services/bus.service';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';

interface SearchCriteria {
  sourceDistrict: string;
  destinationDistrict: string;
  journeyDate: string;
}

interface BusWithAvailability extends BusDto {
  availableSeats?: number;
  isAvailableOnDate?: boolean;
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
  searchResults: BusWithAvailability[] = [];
  hasSearched: boolean = false;
  searchInProgress: boolean = false;

  // Date availability
  availableDates: string[] = [];
  dateAvailability: { [key: string]: number } = {};
  isCheckingAvailability: boolean = false;

  // Account menu
  showAccountMenu: boolean = false;

  // Seat selection
  showSeatModal: boolean = false;
  selectedBus: BusWithAvailability | null = null;
  selectedSeats: number[] = [];
  bookedSeats: number[] = [];
  seatLayout: number[][] = [];
  isBookingInProgress: boolean = false;
  bookingError: string = '';
  availableSeatsCount: number = 0;

  constructor(
    private locationService: LocationService,
    private busService: BusService,
    private bookingService: BookingService,
    private router: Router,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
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
    
    // Clear previous search results when date changes
    this.clearSearch();
  }

  onDateChange() {
    // Clear previous search results when date changes manually
    this.clearSearch();
  }

  isDateAvailable(date: string): boolean {
    return this.availableDates.includes(date);
  }

  getMinDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  getMaxDate(): string {
    const maxDate = new Date();
    maxDate.setDate(maxDate.getDate() + 90); // 90 days ahead
    return maxDate.toISOString().split('T')[0];
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

    // Validate selected date is not in the past
    const selectedDate = new Date(this.searchCriteria.journeyDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    if (selectedDate < today) {
      this.errorMessage = 'Please select a future date';
      return;
    }

    this.errorMessage = '';
    this.searchInProgress = true;
    this.hasSearched = true;
    this.searchResults = [];
    
    this.busService.getAvailableBuses(this.selectedFromDistrict, this.selectedToDistrict).subscribe({
      next: (response: any) => {
        const buses = response.Buses || response.buses || [];
        const success = response.Success !== undefined ? response.Success : response.success;
        const message = response.Message || response.message || 'No buses found for this route';
        
        if (!success || buses.length === 0) {
          this.searchResults = [];
          this.searchInProgress = false;
          this.errorMessage = message;
          this.cdr.detectChanges();
          return;
        }

        // Check availability for each bus on the selected date
        this.checkBusesAvailability(buses);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to search buses: ' + error.message;
        this.searchResults = [];
        this.searchInProgress = false;
        this.cdr.detectChanges();
      }
    });
  }

  private checkBusesAvailability(buses: BusDto[]) {
    const availabilityChecks = buses.map(bus => 
      this.busService.checkDateAvailability(bus.id, this.searchCriteria.journeyDate, 1)
        .toPromise()
        .then(response => ({
          bus,
          isAvailable: response?.isAvailable || false,
          availableSeats: 0 // Will be updated when getting detailed availability
        }))
        .catch(error => {
          console.log(`Availability check failed for bus ${bus.id}, trying to generate availability:`, error);
          
          // If availability check fails, try to generate availability first
          return this.busService.generateBusAvailability(bus.id)
            .toPromise()
            .then(() => {
              console.log(`Generated availability for bus ${bus.id}, rechecking...`);
              return this.busService.checkDateAvailability(bus.id, this.searchCriteria.journeyDate, 1)
                .toPromise()
                .then(response => ({
                  bus,
                  isAvailable: response?.isAvailable || false,
                  availableSeats: 0
                }))
                .catch(() => ({
                  bus,
                  isAvailable: false,
                  availableSeats: 0
                }));
            })
            .catch(() => ({
              bus,
              isAvailable: false,
              availableSeats: 0
            }));
        })
    );

    Promise.all(availabilityChecks).then(results => {
      console.log('Availability check results:', results);
      
      // Filter only available buses
      const availableBuses = results.filter(result => result.isAvailable);
      
      if (availableBuses.length === 0) {
        this.searchResults = [];
        this.searchInProgress = false;
        this.errorMessage = 'No buses available on the selected date. Availability records may need to be generated.';
        this.cdr.detectChanges();
        return;
      }

      // Get detailed availability for available buses
      const detailChecks = availableBuses.map(result => 
        this.busService.getBusAvailabilityDetails(result.bus.id, this.searchCriteria.journeyDate)
          .toPromise()
          .then(details => {
            console.log(`Availability details for bus ${result.bus.id}:`, details);
            const availability = details && details.length > 0 ? details[0] : null;
            return {
              ...result.bus,
              isAvailableOnDate: true,
              availableSeats: availability?.availableSeats || (result.bus.seatingCapacity || 40) // Fallback to full capacity
            } as BusWithAvailability;
          })
          .catch(error => {
            console.log(`Failed to get availability details for bus ${result.bus.id}:`, error);
            return {
              ...result.bus,
              isAvailableOnDate: true, // Still show the bus if basic availability check passed
              availableSeats: result.bus.seatingCapacity || 40 // Fallback to full capacity
            } as BusWithAvailability;
          })
      );

      Promise.all(detailChecks).then(busesWithAvailability => {
        this.searchResults = busesWithAvailability.filter(bus => 
          bus.isAvailableOnDate && (bus.availableSeats || 0) > 0
        );
        this.searchInProgress = false;
        
        console.log('Final search results:', this.searchResults);
        
        if (this.searchResults.length === 0) {
          this.errorMessage = 'No seats available on the selected date';
        } else {
          this.errorMessage = '';
        }
        
        this.cdr.detectChanges();
      });
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

  selectBus(bus: BusWithAvailability) {
    console.log('Selecting bus:', bus);
    
    // Double-check availability before opening seat selection
    this.isCheckingAvailability = true;
    this.busService.checkDateAvailability(bus.id, this.searchCriteria.journeyDate, 1).subscribe({
      next: (response) => {
        console.log('Availability check response:', response);
        this.isCheckingAvailability = false;
        
        if (!response?.isAvailable) {
          this.errorMessage = 'This bus is no longer available for the selected date';
          return;
        }

        this.selectedBus = bus;
        this.selectedSeats = [];
        this.bookedSeats = [];
        this.bookingError = '';
        this.availableSeatsCount = bus.availableSeats || 0;
        
        console.log('Available seats count:', this.availableSeatsCount);
        
        // Generate seat layout (2+2 layout: 2 seats on left, 2 seats on right)
        const capacity = bus.seatingCapacity || 40;
        const seatsPerRow = 4; // 2 on left + 2 on right
        const numberOfRows = Math.ceil(capacity / seatsPerRow);
        
        this.seatLayout = [];
        let seatNumber = 1;
        
        // Create rows with 4 seats each (2 left + 2 right)
        for (let i = 0; i < numberOfRows; i++) {
          const row = [];
          // Add seats for this row (up to 4 seats or remaining capacity)
          for (let j = 0; j < seatsPerRow && seatNumber <= capacity; j++) {
            row.push(seatNumber++);
          }
          this.seatLayout.push(row);
        }
        
        console.log('Generated seat layout:', this.seatLayout);
        
        // Fetch booked seats for this bus and date
        this.loadBookedSeats(bus.id);
        
        this.showSeatModal = true;
      },
      error: (error) => {
        console.error('Availability check error:', error);
        this.isCheckingAvailability = false;
        this.errorMessage = 'Failed to check bus availability: ' + error.message;
      }
    });
  }

  loadBookedSeats(busId: number) {
    console.log(`Loading booked seats for bus ${busId} on ${this.searchCriteria.journeyDate}`);
    
    this.bookingService.getBookedSeats(busId, this.searchCriteria.journeyDate).subscribe({
      next: (bookings: any[]) => {
        console.log('Booked seats response:', bookings);
        this.bookedSeats = [];
        
        if (bookings && Array.isArray(bookings)) {
          bookings.forEach((booking: any) => {
            if (booking.seatNumbers) {
              const seats = booking.seatNumbers.split(',').map((s: string) => parseInt(s.trim()));
              this.bookedSeats.push(...seats);
            }
          });
        }
        
        console.log('Processed booked seats:', this.bookedSeats);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading booked seats:', error);
        // Initialize with empty array if there's an error
        this.bookedSeats = [];
        this.cdr.detectChanges();
      }
    });
  }

  toggleSeat(seatNumber: number) {
    if (this.isSeatBooked(seatNumber)) {
      return;
    }

    const index = this.selectedSeats.indexOf(seatNumber);
    if (index > -1) {
      this.selectedSeats.splice(index, 1);
    } else {
      this.selectedSeats.push(seatNumber);
    }
    this.selectedSeats.sort((a, b) => a - b);
  }

  isSeatSelected(seatNumber: number): boolean {
    return this.selectedSeats.includes(seatNumber);
  }

  isSeatBooked(seatNumber: number): boolean {
    const isBooked = this.bookedSeats.includes(seatNumber);
    // console.log(`Seat ${seatNumber} is booked: ${isBooked}`, this.bookedSeats);
    return isBooked;
  }

  getSeatClass(seatNumber: number): string {
    if (this.isSeatBooked(seatNumber)) {
      return 'seat booked';
    }
    if (this.isSeatSelected(seatNumber)) {
      return 'seat selected';
    }
    return 'seat available';
  }

  getTotalFare(): number {
    if (!this.selectedBus) return 0;
    return this.selectedSeats.length * this.selectedBus.price;
  }

  closeSeatModal() {
    this.showSeatModal = false;
    this.selectedBus = null;
    this.selectedSeats = [];
    this.bookedSeats = [];
    this.bookingError = '';
  }

  confirmBooking() {
    if (this.selectedSeats.length === 0) {
      this.bookingError = 'Please select at least one seat';
      return;
    }

    if (!this.selectedBus) {
      this.bookingError = 'Bus information not available';
      return;
    }

    const user = this.authService.getCurrentUser();
    if (!user || !user.id) {
      this.bookingError = 'User not logged in';
      return;
    }

    // Final availability check before booking
    this.busService.checkDateAvailability(
      this.selectedBus.id, 
      this.searchCriteria.journeyDate, 
      this.selectedSeats.length
    ).subscribe({
      next: (availabilityResponse) => {
        if (!availabilityResponse?.isAvailable) {
          this.bookingError = 'Selected seats are no longer available. Please refresh and try again.';
          return;
        }

        this.isBookingInProgress = true;
        this.bookingError = '';

        const bookingData = {
          userId: user.id,
          busId: this.selectedBus!.id,
          travelDate: new Date(this.searchCriteria.journeyDate + 'T00:00:00.000Z').toISOString(),
          seatNumbers: this.selectedSeats.join(', '),
          totalFare: this.getTotalFare()
        };

        this.bookingService.createBooking(bookingData).subscribe({
          next: (response) => {
            this.isBookingInProgress = false;
            alert(`Booking confirmed! Booking ID: ${response.id}\nSeats: ${response.seatNumbers}\nTotal Fare: ₹${response.totalFare}`);
            this.closeSeatModal();
            
            // Refresh search results to show updated availability
            this.searchBuses();
          },
          error: (error) => {
            this.isBookingInProgress = false;
            this.bookingError = error.message || 'Failed to create booking';
            this.cdr.detectChanges();
          }
        });
      },
      error: (error) => {
        this.bookingError = 'Failed to verify availability: ' + error.message;
      }
    });
  }
}
