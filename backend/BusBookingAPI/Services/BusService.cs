using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;

namespace BusBookingAPI.Services
{
    public interface IBusService
    {
        Task<AvailableBusesResponse> GetAvailableBusesAsync(string sourceDistrict, string destinationDistrict);
    }

    public class BusService : IBusService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<BusService> _logger;

        public BusService(BusBookingDbContext context, ILogger<BusService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AvailableBusesResponse> GetAvailableBusesAsync(string sourceDistrict, string destinationDistrict)
        {
            try
            {
                var response = new AvailableBusesResponse();

                if (string.IsNullOrWhiteSpace(sourceDistrict) || string.IsNullOrWhiteSpace(destinationDistrict))
                {
                    response.Success = false;
                    response.Message = "Source and destination districts are required.";
                    return response;
                }

                // Find routes that match source and destination districts
                var buses = await _context.Buses
                    .Where(b => b.IsActive &&
                                b.Route.SourceLocation.District.DistrictName.ToLower() == sourceDistrict.ToLower() &&
                                b.Route.DestinationLocation.District.DistrictName.ToLower() == destinationDistrict.ToLower())
                    .Include(b => b.Operator)
                    .Include(b => b.Route)
                    .Include(b => b.Route.SourceLocation)
                    .Include(b => b.Route.SourceLocation.District)
                    .Include(b => b.Route.DestinationLocation)
                    .Include(b => b.Route.DestinationLocation.District)
                    .Select(b => new BusDto
                    {
                        Id = b.Id,
                        RegistrationNumber = b.RegistrationNumber,
                        OperatorId = b.OperatorId,
                        OperatorName = b.Operator.OperatorName,
                        RouteId = b.RouteId,
                        SeatingCapacity = b.SeatingCapacity,
                        IsActive = b.IsActive,
                        SourceCity = b.Route.SourceLocation.City,
                        DestinationCity = b.Route.DestinationLocation.City,
                        DistanceKm = b.Route.DistanceKm,
                        EstimatedDurationHours = b.Route.EstimatedDurationHours
                    })
                    .ToListAsync();

                response.Success = true;
                response.Buses = buses;
                response.Message = buses.Any()
                    ? $"Found {buses.Count} available buses."
                    : "No available buses found for the given route.";

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available buses");
                return new AvailableBusesResponse
                {
                    Success = false,
                    Message = "An error occurred while retrieving available buses."
                };
            }
        }
    }
}
