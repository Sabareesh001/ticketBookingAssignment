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

                var response = new AvailableDatesResponse
                {
                    AvailableDates = availabilities.Select(a => a.AvailableDate).ToList(),
                    DateAvailability = availabilities.ToDictionary(a => a.AvailableDate, a => a.AvailableSeats)
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
                    BusId = bus.Id,
                    OperatingDays = bus.OperatingDays,
                    DepartureTime = bus.DepartureTime,
                    ArrivalTime = bus.ArrivalTime,
                    AdvanceBookingDays = bus.AdvanceBookingDays
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting bus schedule for bus {busId}: {ex.Message}");
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
                bus.DepartureTime = scheduleDto.DepartureTime;
                bus.ArrivalTime = scheduleDto.ArrivalTime;
                bus.AdvanceBookingDays = scheduleDto.AdvanceBookingDays;
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
                CreatedAt = availability.CreatedAt,
                UpdatedAt = availability.UpdatedAt
            };
        }
    }
}