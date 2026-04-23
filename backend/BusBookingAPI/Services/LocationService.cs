using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services
{
    public interface ILocationService
    {
        Task<LocationDto> GetLocationByIdAsync(int id);
        Task<List<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto);
        Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto);
        Task<bool> DeleteLocationAsync(int id);
    }

    public class LocationService : ILocationService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<LocationService> _logger;

        public LocationService(BusBookingDbContext context, ILogger<LocationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LocationDto> GetLocationByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching location with ID {id}");

            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                _logger.LogWarning($"Location with ID {id} not found");
                throw new KeyNotFoundException($"Location with ID {id} not found");
            }

            return MapToDto(location);
        }

        public async Task<List<LocationDto>> GetAllLocationsAsync()
        {
            _logger.LogInformation("Fetching all locations");

            var locations = await _context.Locations.ToListAsync();

            return locations.Select(MapToDto).ToList();
        }

        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto createLocationDto)
        {
            _logger.LogInformation($"Creating new location in {createLocationDto.City}");

            // Validate that District, State, and Country exist
            var districtExists = await _context.Districts.AnyAsync(d => d.Id == createLocationDto.DistrictId);
            if (!districtExists)
            {
                throw new ArgumentException($"District with ID {createLocationDto.DistrictId} not found");
            }

            var stateExists = await _context.States.AnyAsync(s => s.Id == createLocationDto.StateId);
            if (!stateExists)
            {
                throw new ArgumentException($"State with ID {createLocationDto.StateId} not found");
            }

            var countryExists = await _context.Countries.AnyAsync(c => c.Id == createLocationDto.CountryId);
            if (!countryExists)
            {
                throw new ArgumentException($"Country with ID {createLocationDto.CountryId} not found");
            }

            if (createLocationDto.OperatorId.HasValue)
            {
                var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == createLocationDto.OperatorId);
                if (!operatorExists)
                {
                    throw new ArgumentException($"Bus Operator with ID {createLocationDto.OperatorId} not found");
                }
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
                OperatorId = createLocationDto.OperatorId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location created successfully with ID {location.Id}");

            return MapToDto(location);
        }

        public async Task<LocationDto> UpdateLocationAsync(int id, UpdateLocationDto updateLocationDto)
        {
            _logger.LogInformation($"Updating location with ID {id}");

            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                _logger.LogWarning($"Location with ID {id} not found");
                throw new KeyNotFoundException($"Location with ID {id} not found");
            }

            // Validate that District, State, and Country exist
            var districtExists = await _context.Districts.AnyAsync(d => d.Id == updateLocationDto.DistrictId);
            if (!districtExists)
            {
                throw new ArgumentException($"District with ID {updateLocationDto.DistrictId} not found");
            }

            var stateExists = await _context.States.AnyAsync(s => s.Id == updateLocationDto.StateId);
            if (!stateExists)
            {
                throw new ArgumentException($"State with ID {updateLocationDto.StateId} not found");
            }

            var countryExists = await _context.Countries.AnyAsync(c => c.Id == updateLocationDto.CountryId);
            if (!countryExists)
            {
                throw new ArgumentException($"Country with ID {updateLocationDto.CountryId} not found");
            }

            if (updateLocationDto.OperatorId.HasValue)
            {
                var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == updateLocationDto.OperatorId);
                if (!operatorExists)
                {
                    throw new ArgumentException($"Bus Operator with ID {updateLocationDto.OperatorId} not found");
                }
            }

            location.StreetAddress = updateLocationDto.StreetAddress;
            location.DistrictId = updateLocationDto.DistrictId;
            location.City = updateLocationDto.City;
            location.StateId = updateLocationDto.StateId;
            location.CountryId = updateLocationDto.CountryId;
            location.PostalCode = updateLocationDto.PostalCode;
            location.Latitude = updateLocationDto.Latitude;
            location.Longitude = updateLocationDto.Longitude;
            location.OperatorId = updateLocationDto.OperatorId;
            location.UpdatedAt = DateTime.UtcNow;

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location with ID {id} updated successfully");

            return MapToDto(location);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            _logger.LogInformation($"Deleting location with ID {id}");

            var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                _logger.LogWarning($"Location with ID {id} not found");
                throw new KeyNotFoundException($"Location with ID {id} not found");
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Location with ID {id} deleted successfully");

            return true;
        }

        private LocationDto MapToDto(Location location)
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
