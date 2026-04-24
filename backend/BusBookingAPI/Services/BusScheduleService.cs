using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public class BusScheduleService : IBusScheduleService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<BusScheduleService> _logger;

        public BusScheduleService(BusBookingDbContext context, ILogger<BusScheduleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BusScheduleDto>> GetBusSchedulesAsync(int busId)
        {
            try
            {
                var schedules = await _context.BusSchedules
                    .Where(bs => bs.BusId == busId)
                    .OrderBy(bs => bs.PickupTime)
                    .ToListAsync();

                return schedules.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedules for bus {busId}: {ex.Message}");
                throw;
            }
        }

        public async Task<BusScheduleDto> GetBusScheduleByIdAsync(int scheduleId)
        {
            try
            {
                var schedule = await _context.BusSchedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");
                }

                return MapToDto(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedule {scheduleId}: {ex.Message}");
                throw;
            }
        }

        public async Task<BusScheduleDto> CreateBusScheduleAsync(CreateBusScheduleDto createScheduleDto)
        {
            try
            {
                // Validate bus exists
                var busExists = await _context.Buses.AnyAsync(b => b.Id == createScheduleDto.BusId);
                if (!busExists)
                {
                    throw new ArgumentException($"Bus with ID {createScheduleDto.BusId} not found");
                }

                var schedule = new BusSchedule
                {
                    BusId = createScheduleDto.BusId,
                    ScheduleName = createScheduleDto.ScheduleName,
                    PickupTime = createScheduleDto.PickupTime,
                    DropTime = createScheduleDto.DropTime,
                    OperatingDays = createScheduleDto.OperatingDays,
                    EffectiveFrom = createScheduleDto.EffectiveFrom ?? DateTime.Today,
                    EffectiveTo = createScheduleDto.EffectiveTo ?? DateTime.Today.AddYears(1),
                    IsActive = true
                };

                _context.BusSchedules.Add(schedule);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created schedule {schedule.Id} for bus {createScheduleDto.BusId}");
                return MapToDto(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating schedule: {ex.Message}");
                throw;
            }
        }

        public async Task<BusScheduleDto> UpdateBusScheduleAsync(int scheduleId, UpdateBusScheduleDto updateScheduleDto)
        {
            try
            {
                var schedule = await _context.BusSchedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");
                }

                schedule.ScheduleName = updateScheduleDto.ScheduleName;
                schedule.PickupTime = updateScheduleDto.PickupTime;
                schedule.DropTime = updateScheduleDto.DropTime;
                schedule.IsActive = updateScheduleDto.IsActive;
                schedule.OperatingDays = updateScheduleDto.OperatingDays;
                schedule.EffectiveFrom = updateScheduleDto.EffectiveFrom;
                schedule.EffectiveTo = updateScheduleDto.EffectiveTo;
                schedule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated schedule {scheduleId}");
                return MapToDto(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating schedule {scheduleId}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBusScheduleAsync(int scheduleId)
        {
            try
            {
                var schedule = await _context.BusSchedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");
                }

                _context.BusSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted schedule {scheduleId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting schedule {scheduleId}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BusScheduleDto>> GetActiveSchedulesForDateAsync(int busId, DateTime date)
        {
            try
            {
                var dayOfWeek = (int)date.DayOfWeek;
                var adjustedDayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek; // Convert Sunday from 0 to 7

                var schedules = await _context.BusSchedules
                    .Where(bs => bs.BusId == busId &&
                                bs.IsActive &&
                                bs.EffectiveFrom <= date &&
                                bs.EffectiveTo >= date &&
                                bs.OperatingDays.Contains(adjustedDayOfWeek.ToString()))
                    .OrderBy(bs => bs.PickupTime)
                    .ToListAsync();

                return schedules.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active schedules for bus {busId} on {date}: {ex.Message}");
                throw;
            }
        }

        private BusScheduleDto MapToDto(BusSchedule schedule)
        {
            return new BusScheduleDto
            {
                Id = schedule.Id,
                BusId = schedule.BusId,
                ScheduleName = schedule.ScheduleName,
                PickupTime = schedule.PickupTime,
                DropTime = schedule.DropTime,
                IsActive = schedule.IsActive,
                OperatingDays = schedule.OperatingDays,
                EffectiveFrom = schedule.EffectiveFrom,
                EffectiveTo = schedule.EffectiveTo
            };
        }
    }
}