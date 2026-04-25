using BusBookingAPI.DTOs;

namespace BusBookingAPI.Services
{
    public interface IBusAvailabilityService
    {
        // CRUD operations
        Task<List<BusAvailabilityDto>> GetAllAvailabilitiesAsync();
        Task<BusAvailabilityDto> GetAvailabilityByIdAsync(int id);
        Task<List<BusAvailabilityDto>> GetAvailabilitiesByBusAsync(int busId);
        Task<BusAvailabilityDto> CreateAvailabilityAsync(CreateBusAvailabilityDto createDto);
        Task<BusAvailabilityDto> UpdateAvailabilityAsync(int id, UpdateBusAvailabilityDto updateDto);
        Task DeleteAvailabilityAsync(int id);
        
        // Existing methods
        Task<AvailableDatesResponse> GetAvailableDatesAsync(int busId, DateTime? startDate = null, DateTime? endDate = null);
        Task<AvailableDatesResponse> GetAvailableDatesWithTimingAsync(int busId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<BusAvailabilityDto>> GetBusAvailabilityAsync(int busId, DateTime date);
        Task<bool> IsDateAvailableAsync(int busId, DateTime date, int requiredSeats);
        Task GenerateBusAvailabilityAsync(int busId);
        Task<bool> UpdateAvailabilityOnBookingAsync(int busId, DateTime travelDate, int seatCount, bool isBooking = true);
        Task<BusScheduleDto> GetBusScheduleAsync(int busId);
        Task<bool> UpdateBusScheduleAsync(int busId, BusScheduleDto scheduleDto);
        
        // New methods for timing management
        Task<bool> UpdateAvailabilityTimingAsync(UpdateBusAvailabilityTimingDto updateDto);
        Task<BulkUpdateResult> BulkUpdateAvailabilityTimingAsync(BulkUpdateAvailabilityTimingDto bulkUpdateDto);
    }

    public class BulkUpdateResult
    {
        public int UpdatedCount { get; set; }
        public List<DateTime> FailedDates { get; set; } = new List<DateTime>();
    }
}