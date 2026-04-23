using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public interface IBusService
    {
        Task<BusDto> GetBusByIdAsync(int id);
        Task<List<BusDto>> GetAllBusesAsync();
        Task<BusDto> CreateBusAsync(CreateBusDto createBusDto);
        Task<BusDto> UpdateBusAsync(int id, UpdateBusDto updateBusDto);
        Task<bool> DeleteBusAsync(int id);
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

        public async Task<BusDto> GetBusByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching bus with ID {id}");

            var bus = await _context.Buses
                .Include(b => b.Operator)
                .Include(b => b.Route)
                .Include(b => b.Route.SourceLocation)
                .Include(b => b.Route.SourceLocation.District)
                .Include(b => b.Route.DestinationLocation)
                .Include(b => b.Route.DestinationLocation.District)
                .Include(b => b.SourceLocation)
                .Include(b => b.DestinationLocation)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bus == null)
            {
                _logger.LogWarning($"Bus with ID {id} not found");
                throw new KeyNotFoundException($"Bus with ID {id} not found");
            }

            return MapToDto(bus);
        }

        public async Task<List<BusDto>> GetAllBusesAsync()
        {
            _logger.LogInformation("Fetching all buses");

            var buses = await _context.Buses
                .Include(b => b.Operator)
                .Include(b => b.Route)
                .Include(b => b.Route.SourceLocation)
                .Include(b => b.Route.SourceLocation.District)
                .Include(b => b.Route.DestinationLocation)
                .Include(b => b.Route.DestinationLocation.District)
                .Include(b => b.SourceLocation)
                .Include(b => b.DestinationLocation)
                .ToListAsync();

            return buses.Select(MapToDto).ToList();
        }

        public async Task<BusDto> CreateBusAsync(CreateBusDto createBusDto)
        {
            _logger.LogInformation($"Creating new bus with registration number {createBusDto.RegistrationNumber}");

            // Validate that the operator exists
            var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == createBusDto.OperatorId);
            if (!operatorExists)
            {
                throw new ArgumentException($"Bus operator with ID {createBusDto.OperatorId} not found");
            }

            // Validate that the route exists
            var routeExists = await _context.Routes.AnyAsync(r => r.Id == createBusDto.RouteId);
            if (!routeExists)
            {
                throw new ArgumentException($"Route with ID {createBusDto.RouteId} not found");
            }

            // Validate that source location exists
            var sourceLocationExists = await _context.Locations.AnyAsync(l => l.Id == createBusDto.SourceLocationId);
            if (!sourceLocationExists)
            {
                throw new ArgumentException($"Source location with ID {createBusDto.SourceLocationId} not found");
            }

            // Validate that destination location exists
            var destinationLocationExists = await _context.Locations.AnyAsync(l => l.Id == createBusDto.DestinationLocationId);
            if (!destinationLocationExists)
            {
                throw new ArgumentException($"Destination location with ID {createBusDto.DestinationLocationId} not found");
            }

            // Validate that registration number is unique
            var registrationExists = await _context.Buses.AnyAsync(b => b.RegistrationNumber == createBusDto.RegistrationNumber);
            if (registrationExists)
            {
                throw new ArgumentException($"A bus with registration number {createBusDto.RegistrationNumber} already exists");
            }

            var bus = new Bus
            {
                RegistrationNumber = createBusDto.RegistrationNumber,
                OperatorId = createBusDto.OperatorId,
                RouteId = createBusDto.RouteId,
                SourceLocationId = createBusDto.SourceLocationId,
                DestinationLocationId = createBusDto.DestinationLocationId,
                SeatingCapacity = createBusDto.SeatingCapacity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bus created successfully with ID {bus.Id}");

            return await GetBusByIdAsync(bus.Id);
        }

        public async Task<BusDto> UpdateBusAsync(int id, UpdateBusDto updateBusDto)
        {
            _logger.LogInformation($"Updating bus with ID {id}");

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == id);

            if (bus == null)
            {
                _logger.LogWarning($"Bus with ID {id} not found");
                throw new KeyNotFoundException($"Bus with ID {id} not found");
            }

            // Validate that the operator exists
            var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == updateBusDto.OperatorId);
            if (!operatorExists)
            {
                throw new ArgumentException($"Bus operator with ID {updateBusDto.OperatorId} not found");
            }

            // Validate that the route exists
            var routeExists = await _context.Routes.AnyAsync(r => r.Id == updateBusDto.RouteId);
            if (!routeExists)
            {
                throw new ArgumentException($"Route with ID {updateBusDto.RouteId} not found");
            }

            // Validate that source location exists
            var sourceLocationExists = await _context.Locations.AnyAsync(l => l.Id == updateBusDto.SourceLocationId);
            if (!sourceLocationExists)
            {
                throw new ArgumentException($"Source location with ID {updateBusDto.SourceLocationId} not found");
            }

            // Validate that destination location exists
            var destinationLocationExists = await _context.Locations.AnyAsync(l => l.Id == updateBusDto.DestinationLocationId);
            if (!destinationLocationExists)
            {
                throw new ArgumentException($"Destination location with ID {updateBusDto.DestinationLocationId} not found");
            }

            // Validate that registration number is unique (if changed)
            if (bus.RegistrationNumber != updateBusDto.RegistrationNumber)
            {
                var registrationExists = await _context.Buses.AnyAsync(b => b.RegistrationNumber == updateBusDto.RegistrationNumber);
                if (registrationExists)
                {
                    throw new ArgumentException($"A bus with registration number {updateBusDto.RegistrationNumber} already exists");
                }
            }

            bus.RegistrationNumber = updateBusDto.RegistrationNumber;
            bus.OperatorId = updateBusDto.OperatorId;
            bus.RouteId = updateBusDto.RouteId;
            bus.SourceLocationId = updateBusDto.SourceLocationId;
            bus.DestinationLocationId = updateBusDto.DestinationLocationId;
            bus.SeatingCapacity = updateBusDto.SeatingCapacity;
            bus.IsActive = updateBusDto.IsActive;
            bus.UpdatedAt = DateTime.UtcNow;

            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bus with ID {id} updated successfully");

            return await GetBusByIdAsync(id);
        }

        public async Task<bool> DeleteBusAsync(int id)
        {
            _logger.LogInformation($"Deleting bus with ID {id}");

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == id);

            if (bus == null)
            {
                _logger.LogWarning($"Bus with ID {id} not found");
                throw new KeyNotFoundException($"Bus with ID {id} not found");
            }

            // Check if bus has active bookings
            var hasActiveBookings = await _context.Bookings.AnyAsync(booking => booking.BusId == id && booking.TravelStatus == "active");
            if (hasActiveBookings)
            {
                throw new InvalidOperationException($"Cannot delete bus with ID {id} because it has active bookings");
            }

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Bus with ID {id} deleted successfully");

            return true;
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
                    .Include(b => b.SourceLocation)
                    .Include(b => b.DestinationLocation)
                    .Select(b => new BusDto
                    {
                        Id = b.Id,
                        RegistrationNumber = b.RegistrationNumber,
                        OperatorId = b.OperatorId,
                        OperatorName = b.Operator.OperatorName,
                        RouteId = b.RouteId,
                        SourceLocationId = b.SourceLocationId,
                        DestinationLocationId = b.DestinationLocationId,
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

        private BusDto MapToDto(Bus bus)
        {
            return new BusDto
            {
                Id = bus.Id,
                RegistrationNumber = bus.RegistrationNumber,
                OperatorId = bus.OperatorId,
                OperatorName = bus.Operator?.OperatorName,
                RouteId = bus.RouteId,
                SourceLocationId = bus.SourceLocationId,
                DestinationLocationId = bus.DestinationLocationId,
                SeatingCapacity = bus.SeatingCapacity,
                IsActive = bus.IsActive,
                SourceCity = bus.SourceLocation?.City,
                DestinationCity = bus.DestinationLocation?.City,
                DistanceKm = bus.Route?.DistanceKm,
                EstimatedDurationHours = bus.Route?.EstimatedDurationHours,
                CreatedAt = bus.CreatedAt,
                UpdatedAt = bus.UpdatedAt
            };
        }
    }
}
