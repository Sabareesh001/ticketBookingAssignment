using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public class OperatorDashboardService : IOperatorDashboardService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<OperatorDashboardService> _logger;

        public OperatorDashboardService(BusBookingDbContext context, ILogger<OperatorDashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<LocationDto>> GetOperatorLocationsAsync(int operatorId)
        {
            var locations = await _context.Locations
                .Where(l => l.OperatorId == operatorId)
                .Include(l => l.District)
                .Include(l => l.State)
                .Include(l => l.Country)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return locations.Select(MapToLocationDto).ToList();
        }

        public async Task<LocationDto> GetLocationByIdAsync(int locationId, int operatorId)
        {
            var location = await _context.Locations
                .Where(l => l.Id == locationId && l.OperatorId == operatorId)
                .Include(l => l.District)
                .Include(l => l.State)
                .Include(l => l.Country)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {locationId} not found or does not belong to this operator");
            }

            return MapToLocationDto(location);
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto, int operatorId)
        {
            // Validate that the district, state, and country exist
            var district = await _context.Districts.FindAsync(createLocationDto.DistrictId);
            if (district == null)
            {
                throw new ArgumentException($"District with ID {createLocationDto.DistrictId} not found");
            }

            var state = await _context.States.FindAsync(createLocationDto.StateId);
            if (state == null)
            {
                throw new ArgumentException($"State with ID {createLocationDto.StateId} not found");
            }

            var country = await _context.Countries.FindAsync(createLocationDto.CountryId);
            if (country == null)
            {
                throw new ArgumentException($"Country with ID {createLocationDto.CountryId} not found");
            }

            var location = new Location
            {
                StreetAddress = createLocationDto.StreetAddress,
                DistrictId = createLocationDto.DistrictId,
                City = createLocationDto.City,
                StateId = createLocationDto.StateId,
                CountryId = createLocationDto.CountryId,
                PostalCode = createLocationDto.PostalCode,
                Latitude = createLocationDto.Latitude,
                Longitude = createLocationDto.Longitude,
                OperatorId = operatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location created with ID {location.Id} for operator {operatorId}");

            // Reload with related data
            await _context.Entry(location).Reference(l => l.District).LoadAsync();
            await _context.Entry(location).Reference(l => l.State).LoadAsync();
            await _context.Entry(location).Reference(l => l.Country).LoadAsync();

            return MapToLocationDto(location);
        }

        public async Task<LocationDto> UpdateLocationAsync(int locationId, UpdateLocationDto updateLocationDto, int operatorId)
        {
            var location = await _context.Locations
                .Where(l => l.Id == locationId && l.OperatorId == operatorId)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {locationId} not found or does not belong to this operator");
            }

            // Validate that the district, state, and country exist
            var district = await _context.Districts.FindAsync(updateLocationDto.DistrictId);
            if (district == null)
            {
                throw new ArgumentException($"District with ID {updateLocationDto.DistrictId} not found");
            }

            var state = await _context.States.FindAsync(updateLocationDto.StateId);
            if (state == null)
            {
                throw new ArgumentException($"State with ID {updateLocationDto.StateId} not found");
            }

            var country = await _context.Countries.FindAsync(updateLocationDto.CountryId);
            if (country == null)
            {
                throw new ArgumentException($"Country with ID {updateLocationDto.CountryId} not found");
            }

            location.StreetAddress = updateLocationDto.StreetAddress;
            location.DistrictId = updateLocationDto.DistrictId;
            location.City = updateLocationDto.City;
            location.StateId = updateLocationDto.StateId;
            location.CountryId = updateLocationDto.CountryId;
            location.PostalCode = updateLocationDto.PostalCode;
            location.Latitude = updateLocationDto.Latitude;
            location.Longitude = updateLocationDto.Longitude;
            location.UpdatedAt = DateTime.UtcNow;

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location {locationId} updated for operator {operatorId}");

            // Reload with related data
            await _context.Entry(location).Reference(l => l.District).LoadAsync();
            await _context.Entry(location).Reference(l => l.State).LoadAsync();
            await _context.Entry(location).Reference(l => l.Country).LoadAsync();

            return MapToLocationDto(location);
        }

        public async Task DeleteLocationAsync(int locationId, int operatorId)
        {
            var location = await _context.Locations
                .Where(l => l.Id == locationId && l.OperatorId == operatorId)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {locationId} not found or does not belong to this operator");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location {locationId} deleted for operator {operatorId}");
        }

        private LocationDto MapToLocationDto(Location location)
        {
            return new LocationDto
            {
                Id = location.Id,
                StreetAddress = location.StreetAddress,
                DistrictId = location.DistrictId,
                City = location.City,
                StateId = location.StateId,
                CountryId = location.CountryId,
                PostalCode = location.PostalCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                OperatorId = location.OperatorId,
                CreatedAt = location.CreatedAt,
                UpdatedAt = location.UpdatedAt
            };
        }
    }
}
