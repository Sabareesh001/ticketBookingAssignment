import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BusService, BusDto, AvailableDateInfo, UpdateBusAvailabilityTimingDto, BulkUpdateAvailabilityTimingDto } from '../../services/bus.service';

@Component({
  selector: 'app-bus-timing-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bus-timing-management.component.html',
  styleUrls: ['./bus-timing-management.component.css']
})
export class BusTimingManagementComponent implements OnInit {
  buses: BusDto[] = [];
  busId: number = 0;
  availableDates: AvailableDateInfo[] = [];
  selectedDates: string[] = [];
  loading = false;
  error = '';
  success = '';

  // Form data for timing updates
  newPickupTime = '08:00';
  newDropTime = '18:00';
  newJourneyDuration = 10;

  // Single date update
  selectedDate = '';
  singlePickupTime = '08:00';
  singleDropTime = '18:00';
  singleJourneyDuration = 10;

  constructor(private busService: BusService) {}

  ngOnInit() {
    this.loadBuses();
  }

  loadBuses() {
    this.loading = true;
    this.busService.getAllBuses().subscribe({
      next: (buses) => {
        this.buses = buses.filter(bus => bus.isActive);
        this.loading = false;
        if (this.buses.length > 0) {
          this.busId = this.buses[0].id;
          this.loadAvailableDates();
        }
      },
      error: (error) => {
        this.error = 'Failed to load buses: ' + error.message;
        this.loading = false;
      }
    });
  }

  onBusChange() {
    if (this.busId > 0) {
      this.loadAvailableDates();
    }
  }

  loadAvailableDates() {
    if (this.busId === 0) return;
    
    this.loading = true;
    this.error = '';
    
    const startDate = new Date().toISOString().split('T')[0];
    const endDate = new Date(Date.now() + 90 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
    
    this.busService.getBusAvailableDates(this.busId, startDate, endDate).subscribe({
      next: (response) => {
        this.availableDates = response.availableDates;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load available dates: ' + error.message;
        this.loading = false;
      }
    });
  }

  onDateSelectionChange(date: string, event: any) {
    if (event.target.checked) {
      this.selectedDates.push(date);
    } else {
      this.selectedDates = this.selectedDates.filter(d => d !== date);
    }
  }

  selectAllDates() {
    this.selectedDates = this.availableDates.map(d => d.date);
  }

  clearSelection() {
    this.selectedDates = [];
  }

  updateSingleDateTiming() {
    if (!this.selectedDate) {
      this.error = 'Please select a date';
      return;
    }

    const updateDto: UpdateBusAvailabilityTimingDto = {
      busId: this.busId,
      availableDate: this.selectedDate,
      pickupTime: this.singlePickupTime + ':00',
      dropTime: this.singleDropTime + ':00',
      journeyDurationHours: this.singleJourneyDuration
    };

    this.loading = true;
    this.error = '';
    this.success = '';

    this.busService.updateAvailabilityTiming(updateDto).subscribe({
      next: (response) => {
        this.success = 'Timing updated successfully for selected date';
        this.loading = false;
        this.loadAvailableDates(); // Refresh the data
      },
      error: (error) => {
        this.error = 'Failed to update timing: ' + error.message;
        this.loading = false;
      }
    });
  }

  bulkUpdateTiming() {
    if (this.selectedDates.length === 0) {
      this.error = 'Please select at least one date';
      return;
    }

    const bulkUpdateDto: BulkUpdateAvailabilityTimingDto = {
      busId: this.busId,
      dates: this.selectedDates,
      pickupTime: this.newPickupTime + ':00',
      dropTime: this.newDropTime + ':00',
      journeyDurationHours: this.newJourneyDuration
    };

    this.loading = true;
    this.error = '';
    this.success = '';

    this.busService.bulkUpdateAvailabilityTiming(bulkUpdateDto).subscribe({
      next: (response) => {
        this.success = `Successfully updated timing for ${response.updatedCount} dates`;
        if (response.failedDates && response.failedDates.length > 0) {
          this.error = `Failed to update ${response.failedDates.length} dates`;
        }
        this.loading = false;
        this.selectedDates = [];
        this.loadAvailableDates(); // Refresh the data
      },
      error: (error) => {
        this.error = 'Failed to bulk update timing: ' + error.message;
        this.loading = false;
      }
    });
  }

  formatTime(timeString: string): string {
    return this.busService.formatTime(timeString);
  }

  calculateDuration(pickupTime: string, dropTime: string): string {
    return this.busService.calculateJourneyDuration(pickupTime, dropTime);
  }

  generateAvailability() {
    if (this.busId === 0) return;
    
    this.loading = true;
    this.error = '';
    this.success = '';

    this.busService.generateBusAvailability(this.busId).subscribe({
      next: (response) => {
        this.success = 'Availability generated successfully';
        this.loading = false;
        this.loadAvailableDates(); // Refresh the data
      },
      error: (error) => {
        this.error = 'Failed to generate availability: ' + error.message;
        this.loading = false;
      }
    });
  }
}