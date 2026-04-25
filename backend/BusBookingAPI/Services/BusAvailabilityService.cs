using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public class BusAvailabilityService : IBusAvailabilityService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<BusAvailabilityService> _logger;

        public BusAvailabilityService(BusBookingDbContext context, ILogger<BusAvailabilityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CRUD Operations
        public async Task<List<BusAvailabilityDto>> GetAllAvailabilitiesAsync()
        {
            try
            {
                var availabilities = await _context.BusAvailabilities
                    .OrderByDescending(ba => ba.AvailableDate)
                    .ToListAsync();

                return availabilities.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all availabilities: {ex.Message}");
                throw;
            }
        }

        public async Task<BusAvailabilityDto> GetAvailabilityByIdAsync(int id)
        {
            try
            {
                var availability = await _context.BusAvailabilities.FindAsync(id);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                return MapToDto(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availability by ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BusAvailabilityDto>> GetAvailabilitiesByBusAsync(int busId)
        {
            try
            {
                var availabilities = await _context.BusAvailabilities
                    .Where(ba => ba.BusId == busId)
                    .OrderByDescending(ba => ba.AvailableDate)
                    .ToListAsync();

                return availabilities.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availabilities for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<BusAvailabilityDto> CreateAvailabilityAsync(CreateBusAvailabilityDto createDto)
        {
            try
            {
                // Verify bus exists
                var bus = await _context.Buses.FindAsync(createDto.BusId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {createDto.BusId} not found");
                }

                var availability = new BusAvailability
                {
                    BusId = createDto.BusId,
                    AvailableDate = createDto.AvailableDate,
                    TotalSeats = createDto.TotalSeats,
                    AvailableSeats = createDto.AvailableSeats,
                    IsActive = createDto.IsActive,
                    PickupTime = createDto.PickupTime,
                    DropTime = createDto.DropTime,
                    JourneyDurationHours = createDto.JourneyDurationHours,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BusAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created availability record {availability.Id} for bus {createDto.BusId}");
                return MapToDto(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating availability: {ex.Message}");
                throw;
            }
        }

        public async Task<BusAvailabilityDto> UpdateAvailabilityAsync(int id, UpdateBusAvailabilityDto updateDto)
        {
            try
            {
                var availability = await _context.BusAvailabilities.FindAsync(id);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                availability.BusId = updateDto.BusId;
                availability.AvailableDate = updateDto.AvailableDate;
                availability.TotalSeats = updateDto.TotalSeats;
                availability.AvailableSeats = updateDto.AvailableSeats;
                availability.IsActive = updateDto.IsActive;
                availability.PickupTime = updateDto.PickupTime;
                availability.DropTime = updateDto.DropTime;
                availability.JourneyDurationHours = updateDto.JourneyDurationHours;
                availability.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated availability record {id}");
                return MapToDto(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating availability {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAvailabilityAsync(int id)
        {
            try
            {
                var availability = await _context.BusAvailabilities.FindAsync(id);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                _context.BusAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted availability record {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting availability {id}: {ex.Message}");
                throw;
            }
        }

        // Existing Methods

        public async Task<AvailableDatesResponse> GetAvailableDatesAsync(int busId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Today;
                var end = endDate ?? DateTime.Today.AddDays(90);

                var availabilities = await _context.BusAvailabilities
                    .Where(ba => ba.BusId == busId &&
                                ba.AvailableDate >= start &&
                                ba.AvailableDate <= end &&
                                ba.IsActive &&
                                ba.AvailableSeats > 0)
                    .OrderBy(ba => ba.AvailableDate)
                    .ToListAsync();

                var availableDates = availabilities.Select(a => new AvailableDateInfo
                {
                    Date = a.AvailableDate,
                    AvailableSeats = a.AvailableSeats
                }).ToList();

                var response = new AvailableDatesResponse
                {
                    AvailableDates = availableDates,
                    DateAvailability = availableDates.ToDictionary(a => a.Date, a => a)
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available dates for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BusAvailabilityDto>> GetBusAvailabilityAsync(int busId, DateTime date)
        {
            try
            {
                var availabilities = await _context.BusAvailabilities
                    .Where(ba => ba.BusId == busId && ba.AvailableDate.Date == date.Date)
                    .ToListAsync();

                return availabilities.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting bus availability for bus {busId} on {date}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsDateAvailableAsync(int busId, DateTime date, int requiredSeats)
        {
            try
            {
                var availability = await _context.BusAvailabilities
                    .FirstOrDefaultAsync(ba => ba.BusId == busId &&
                                             ba.AvailableDate.Date == date.Date &&
                                             ba.IsActive);

                return availability != null && availability.AvailableSeats >= requiredSeats;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking date availability for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task GenerateBusAvailabilityAsync(int busId)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null || !bus.IsActive)
                {
                    throw new KeyNotFoundException($"Bus with ID {busId} not found or inactive");
                }

                var operatingDays = bus.OperatingDays.Split(',').Select(int.Parse).ToList();
                var startDate = DateTime.Today;
                var endDate = startDate.AddDays(bus.AdvanceBookingDays);

                var existingAvailabilities = await _context.BusAvailabilities
                    .Where(ba => ba.BusId == busId && ba.AvailableDate >= startDate)
                    .ToListAsync();

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayOfWeek = (int)date.DayOfWeek;
                    var adjustedDayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek; // Convert Sunday from 0 to 7

                    if (operatingDays.Contains(adjustedDayOfWeek))
                    {
                        var existing = existingAvailabilities.FirstOrDefault(ea => ea.AvailableDate.Date == date.Date);

                        if (existing == null)
                        {
                            var availability = new BusAvailability
                            {
                                BusId = busId,
                                AvailableDate = date,
                                TotalSeats = bus.SeatingCapacity ?? 40,
                                AvailableSeats = bus.SeatingCapacity ?? 40,
                                IsActive = true
                            };

                            _context.BusAvailabilities.Add(availability);
                        }
                        else
                        {
                            existing.TotalSeats = bus.SeatingCapacity ?? 40;
                            existing.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Generated availability for bus {busId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating availability for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAvailabilityOnBookingAsync(int busId, DateTime travelDate, int seatCount, bool isBooking = true)
        {
            try
            {
                var availability = await _context.BusAvailabilities
                    .FirstOrDefaultAsync(ba => ba.BusId == busId && ba.AvailableDate.Date == travelDate.Date);

                if (availability == null)
                {
                    _logger.LogWarning($"No availability record found for bus {busId} on {travelDate}");
                    return false;
                }

                if (isBooking)
                {
                    availability.AvailableSeats -= seatCount;
                }
                else
                {
                    availability.AvailableSeats += seatCount;
                }

                availability.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating availability for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<BusScheduleDto> GetBusScheduleAsync(int busId)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {busId} not found");
                }

                return new BusScheduleDto
                {
                    Id = 0, // This is for the bus's default schedule
                    BusId = bus.Id,
                    ScheduleName = "Default Schedule",
                    IsActive = bus.IsActive,
                    OperatingDays = bus.OperatingDays,
                    EffectiveFrom = DateTime.Today,
                    EffectiveTo = DateTime.Today.AddDays(bus.AdvanceBookingDays)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting bus schedule for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<AvailableDatesResponse> GetAvailableDatesWithTimingAsync(int busId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Today;
                var end = endDate ?? DateTime.Today.AddDays(90);

                var availabilities = await _context.BusAvailabilities
                    .Where(ba => ba.BusId == busId &&
                                ba.AvailableDate >= start &&
                                ba.AvailableDate <= end &&
                                ba.IsActive &&
                                ba.AvailableSeats > 0)
                    .OrderBy(ba => ba.AvailableDate)
                    .ToListAsync();

                var availableDates = availabilities.Select(a => new AvailableDateInfo
                {
                    Date = a.AvailableDate,
                    AvailableSeats = a.AvailableSeats,
                    PickupTime = a.PickupTime,
                    DropTime = a.DropTime,
                    JourneyDurationHours = a.JourneyDurationHours
                }).ToList();

                var response = new AvailableDatesResponse
                {
                    AvailableDates = availableDates,
                    DateAvailability = availableDates.ToDictionary(a => a.Date, a => a)
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available dates with timing for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAvailabilityTimingAsync(UpdateBusAvailabilityTimingDto updateDto)
        {
            try
            {
                var availability = await _context.BusAvailabilities
                    .FirstOrDefaultAsync(ba => ba.BusId == updateDto.BusId &&
                                             ba.AvailableDate.Date == updateDto.AvailableDate.Date);

                if (availability == null)
                {
                    throw new KeyNotFoundException($"No availability record found for bus {updateDto.BusId} on {updateDto.AvailableDate}");
                }

                availability.PickupTime = updateDto.PickupTime;
                availability.DropTime = updateDto.DropTime;
                availability.JourneyDurationHours = updateDto.JourneyDurationHours;
                availability.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated timing for bus {updateDto.BusId} on {updateDto.AvailableDate}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating availability timing: {ex.Message}");
                throw;
            }
        }

        public async Task<BulkUpdateResult> BulkUpdateAvailabilityTimingAsync(BulkUpdateAvailabilityTimingDto bulkUpdateDto)
        {
            var result = new BulkUpdateResult();

            try
            {
                foreach (var date in bulkUpdateDto.Dates)
                {
                    try
                    {
                        var availability = await _context.BusAvailabilities
                            .FirstOrDefaultAsync(ba => ba.BusId == bulkUpdateDto.BusId &&
                                                     ba.AvailableDate.Date == date.Date);

                        if (availability != null)
                        {
                            availability.PickupTime = bulkUpdateDto.PickupTime;
                            availability.DropTime = bulkUpdateDto.DropTime;
                            availability.JourneyDurationHours = bulkUpdateDto.JourneyDurationHours;
                            availability.UpdatedAt = DateTime.UtcNow;
                            result.UpdatedCount++;
                        }
                        else
                        {
                            result.FailedDates.Add(date);
                            _logger.LogWarning($"No availability record found for bus {bulkUpdateDto.BusId} on {date}");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedDates.Add(date);
                        _logger.LogError($"Error updating timing for bus {bulkUpdateDto.BusId} on {date}: {ex.Message}");
                    }
                }

                if (result.UpdatedCount > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Bulk updated timing for {result.UpdatedCount} dates for bus {bulkUpdateDto.BusId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in bulk update availability timing: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateBusScheduleAsync(int busId, BusScheduleDto scheduleDto)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {busId} not found");
                }

                bus.OperatingDays = scheduleDto.OperatingDays;
                bus.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Regenerate availability after schedule update
                await GenerateBusAvailabilityAsync(busId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bus schedule for bus {busId}: {ex.Message}");
                throw;
            }
        }

        private BusAvailabilityDto MapToDto(BusAvailability availability)
        {
            return new BusAvailabilityDto
            {
                Id = availability.Id,
                BusId = availability.BusId,
                AvailableDate = availability.AvailableDate,
                TotalSeats = availability.TotalSeats,
                AvailableSeats = availability.AvailableSeats,
                IsActive = availability.IsActive,
                ScheduleId = availability.ScheduleId,
                PickupTime = availability.PickupTime,
                DropTime = availability.DropTime,
                JourneyDurationHours = availability.JourneyDurationHours,
                CreatedAt = availability.CreatedAt,
                UpdatedAt = availability.UpdatedAt
            };
        }
    }
}