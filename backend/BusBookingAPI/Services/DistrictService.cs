using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services
{
    public interface IDistrictService
    {
        Task<DistrictDto> GetDistrictByIdAsync(int id);
        Task<List<DistrictDto>> GetAllDistrictsAsync();
        Task<List<DistrictDto>> GetDistrictsByStateIdAsync(int stateId);
        Task<DistrictDto> CreateDistrictAsync(CreateDistrictDto createDistrictDto);
        Task<DistrictDto> UpdateDistrictAsync(int id, UpdateDistrictDto updateDistrictDto);
        Task<bool> DeleteDistrictAsync(int id);
    }

    public class DistrictService : IDistrictService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<DistrictService> _logger;

        public DistrictService(BusBookingDbContext context, ILogger<DistrictService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DistrictDto> GetDistrictByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching district with ID {id}");

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == id);

            if (district == null)
            {
                _logger.LogWarning($"District with ID {id} not found");
                throw new KeyNotFoundException($"District with ID {id} not found");
            }

            return MapToDto(district);
        }

        public async Task<List<DistrictDto>> GetAllDistrictsAsync()
        {
            _logger.LogInformation("Fetching all districts");

            var districts = await _context.Districts.ToListAsync();

            return districts.Select(MapToDto).ToList();
        }

        public async Task<List<DistrictDto>> GetDistrictsByStateIdAsync(int stateId)
        {
            _logger.LogInformation($"Fetching all districts for state ID {stateId}");

            // Verify state exists
            var stateExists = await _context.States.AnyAsync(s => s.Id == stateId);
            if (!stateExists)
            {
                _logger.LogWarning($"State with ID {stateId} not found");
                throw new KeyNotFoundException($"State with ID {stateId} not found");
            }

            var districts = await _context.Districts
                .Where(d => d.StateId == stateId)
                .ToListAsync();

            return districts.Select(MapToDto).ToList();
        }

        public async Task<DistrictDto> CreateDistrictAsync(CreateDistrictDto createDistrictDto)
        {
            _logger.LogInformation($"Creating new district: {createDistrictDto.DistrictName}");

            // Validate that State exists
            var stateExists = await _context.States.AnyAsync(s => s.Id == createDistrictDto.StateId);
            if (!stateExists)
            {
                throw new ArgumentException($"State with ID {createDistrictDto.StateId} not found");
            }

            // Check for duplicate district name within the same state
            var districtExists = await _context.Districts
                .AnyAsync(d => d.DistrictName == createDistrictDto.DistrictName && d.StateId == createDistrictDto.StateId);

            if (districtExists)
            {
                throw new ArgumentException($"District '{createDistrictDto.DistrictName}' already exists in this state");
            }

            var district = new District
            {
                DistrictName = createDistrictDto.DistrictName,
                StateId = createDistrictDto.StateId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Districts.Add(district);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"District created successfully with ID {district.Id}");

            return MapToDto(district);
        }

        public async Task<DistrictDto> UpdateDistrictAsync(int id, UpdateDistrictDto updateDistrictDto)
        {
            _logger.LogInformation($"Updating district with ID {id}");

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == id);

            if (district == null)
            {
                _logger.LogWarning($"District with ID {id} not found");
                throw new KeyNotFoundException($"District with ID {id} not found");
            }

            // Validate that State exists
            var stateExists = await _context.States.AnyAsync(s => s.Id == updateDistrictDto.StateId);
            if (!stateExists)
            {
                throw new ArgumentException($"State with ID {updateDistrictDto.StateId} not found");
            }

            // Check for duplicate district name (excluding current district)
            var duplicateDistrict = await _context.Districts
                .AnyAsync(d => d.Id != id && d.DistrictName == updateDistrictDto.DistrictName && d.StateId == updateDistrictDto.StateId);

            if (duplicateDistrict)
            {
                throw new ArgumentException($"District '{updateDistrictDto.DistrictName}' already exists in this state");
            }

            district.DistrictName = updateDistrictDto.DistrictName;
            district.StateId = updateDistrictDto.StateId;

            _context.Districts.Update(district);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"District with ID {id} updated successfully");

            return MapToDto(district);
        }

        public async Task<bool> DeleteDistrictAsync(int id)
        {
            _logger.LogInformation($"Deleting district with ID {id}");

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == id);

            if (district == null)
            {
                _logger.LogWarning($"District with ID {id} not found");
                throw new KeyNotFoundException($"District with ID {id} not found");
            }

            // Check if there are any locations associated with this district
            var hasLocations = await _context.Locations.AnyAsync(l => l.DistrictId == id);
            if (hasLocations)
            {
                throw new InvalidOperationException($"Cannot delete district with ID {id} as it has associated locations");
            }

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"District with ID {id} deleted successfully");

            return true;
        }

        private DistrictDto MapToDto(District district)
        {
            return new DistrictDto
            {
                Id = district.Id,
                DistrictName = district.DistrictName,
                StateId = district.StateId,
                CreatedAt = district.CreatedAt
            };
        }
    }
}
