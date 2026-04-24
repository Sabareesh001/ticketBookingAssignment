using BusBookingAPI.DTOs;

namespace BusBookingAPI.Services
{
    public interface IBusScheduleService
    {
        Task<List<BusScheduleDto>> GetBusSchedulesAsync(int busId);
        Task<BusScheduleDto> GetBusScheduleByIdAsync(int scheduleId);
        Task<BusScheduleDto> CreateBusScheduleAsync(CreateBusScheduleDto createScheduleDto);
        Task<BusScheduleDto> UpdateBusScheduleAsync(int scheduleId, UpdateBusScheduleDto updateScheduleDto);
        Task<bool> DeleteBusScheduleAsync(int scheduleId);
        Task<List<BusScheduleDto>> GetActiveSchedulesForDateAsync(int busId, DateTime date);
    }
}