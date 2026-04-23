using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services
{
    public interface IStateService
    {
        Task<StateDto> GetStateByIdAsync(int id);
        Task<List<StateDto>> GetAllStatesAsync();
        Task<StateDto> CreateStateAsync(CreateStateDto createStateDto);
        Task<StateDto> UpdateStateAsync(int id, UpdateStateDto updateStateDto);
        Task<bool> DeleteStateAsync(int id);
    }

    public class StateService : IStateService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<StateService> _logger;

        public StateService(BusBookingDbContext context, ILogger<StateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StateDto> GetStateByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching state with ID {id}");

            var state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null)
            {
                _logger.LogWarning($"State with ID {id} not found");
                throw new KeyNotFoundException($"State with ID {id} not found");
            }

            return MapToDto(state);
        }

        public async Task<List<StateDto>> GetAllStatesAsync()
        {
            _logger.LogInformation("Fetching all states");

            var states = await _context.States
                .Include(s => s.Country)
                .ToListAsync();

            return states.Select(MapToDto).ToList();
        }

        public async Task<StateDto> CreateStateAsync(CreateStateDto createStateDto)
        {
            _logger.LogInformation($"Creating new state: {createStateDto.StateName}");

            // Check if country exists
            var countryExists = await _context.Countries
                .AnyAsync(c => c.Id == createStateDto.CountryId);

            if (!countryExists)
            {
                throw new ArgumentException($"Country with ID {createStateDto.CountryId} not found");
            }

            // Check for duplicate state name in the same country
            var stateExists = await _context.States
                .AnyAsync(s => s.StateName == createStateDto.StateName && s.CountryId == createStateDto.CountryId);

            if (stateExists)
            {
                throw new ArgumentException($"State '{createStateDto.StateName}' already exists in this country");
            }

            var state = new State
            {
                StateName = createStateDto.StateName,
                CountryId = createStateDto.CountryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.States.Add(state);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"State created successfully with ID {state.Id}");

            // Reload to get the Country navigation property
            await _context.Entry(state).Reference(s => s.Country).LoadAsync();

            return MapToDto(state);
        }

        public async Task<StateDto> UpdateStateAsync(int id, UpdateStateDto updateStateDto)
        {
            _logger.LogInformation($"Updating state with ID {id}");

            var state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null)
            {
                _logger.LogWarning($"State with ID {id} not found");
                throw new KeyNotFoundException($"State with ID {id} not found");
            }

            // Check if country exists
            var countryExists = await _context.Countries
                .AnyAsync(c => c.Id == updateStateDto.CountryId);

            if (!countryExists)
            {
                throw new ArgumentException($"Country with ID {updateStateDto.CountryId} not found");
            }

            // Check for duplicate state name (excluding current state)
            var duplicateState = await _context.States
                .AnyAsync(s => s.Id != id && s.StateName == updateStateDto.StateName && s.CountryId == updateStateDto.CountryId);

            if (duplicateState)
            {
                throw new ArgumentException($"State '{updateStateDto.StateName}' already exists in this country");
            }

            state.StateName = updateStateDto.StateName;
            state.CountryId = updateStateDto.CountryId;

            _context.States.Update(state);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"State with ID {id} updated successfully");

            // Reload to get the updated Country navigation property
            await _context.Entry(state).Reference(s => s.Country).LoadAsync();

            return MapToDto(state);
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            _logger.LogInformation($"Deleting state with ID {id}");

            var state = await _context.States.FirstOrDefaultAsync(s => s.Id == id);

            if (state == null)
            {
                _logger.LogWarning($"State with ID {id} not found");
                throw new KeyNotFoundException($"State with ID {id} not found");
            }

            // Check if there are any districts associated with this state
            var hasDistricts = await _context.Districts.AnyAsync(d => d.StateId == id);
            if (hasDistricts)
            {
                throw new InvalidOperationException($"Cannot delete state with ID {id} as it has associated districts");
            }

            _context.States.Remove(state);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"State with ID {id} deleted successfully");

            return true;
        }

        private StateDto MapToDto(State state)
        {
            return new StateDto
            {
                Id = state.Id,
                StateName = state.StateName,
                CountryId = state.CountryId,
                CountryName = state.Country?.CountryName,
                CreatedAt = state.CreatedAt
            };
        }
    }
}
