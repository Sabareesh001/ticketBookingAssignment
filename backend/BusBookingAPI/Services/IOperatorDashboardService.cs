using BusBookingAPI.DTOs;

namespace BusBookingAPI.Services
{
    public interface IOperatorDashboardService
    {
        Task<List<LocationDto>> GetOperatorLocationsAsync(int operatorId);
        Task<LocationDto> GetLocationByIdAsync(int locationId, int operatorId);
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto, int operatorId);
        Task<LocationDto> UpdateLocationAsync(int locationId, UpdateLocationDto updateLocationDto, int operatorId);
        Task DeleteLocationAsync(int locationId, int operatorId);
    }
}
