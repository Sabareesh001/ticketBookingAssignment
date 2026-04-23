using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services
{
    public interface ICountryService
    {
        Task<CountryDto> GetCountryByIdAsync(int id);
        Task<List<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto> CreateCountryAsync(CreateCountryDto createCountryDto);
        Task<CountryDto> UpdateCountryAsync(int id, UpdateCountryDto updateCountryDto);
        Task<bool> DeleteCountryAsync(int id);
    }

    public class CountryService : ICountryService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<CountryService> _logger;

        public CountryService(BusBookingDbContext context, ILogger<CountryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching country with ID {id}");

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                _logger.LogWarning($"Country with ID {id} not found");
                throw new KeyNotFoundException($"Country with ID {id} not found");
            }

            return MapToDto(country);
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            _logger.LogInformation("Fetching all countries");

            var countries = await _context.Countries.ToListAsync();

            return countries.Select(MapToDto).ToList();
        }

        public async Task<CountryDto> CreateCountryAsync(CreateCountryDto createCountryDto)
        {
            _logger.LogInformation($"Creating new country: {createCountryDto.CountryName}");

            // Check for duplicate country name
            var countryNameExists = await _context.Countries
                .AnyAsync(c => c.CountryName == createCountryDto.CountryName);

            if (countryNameExists)
            {
                throw new ArgumentException($"Country '{createCountryDto.CountryName}' already exists");
            }

            // Check for duplicate country code
            var countryCodeExists = await _context.Countries
                .AnyAsync(c => c.CountryCode == createCountryDto.CountryCode);

            if (countryCodeExists)
            {
                throw new ArgumentException($"Country code '{createCountryDto.CountryCode}' already exists");
            }

            var country = new Country
            {
                CountryName = createCountryDto.CountryName,
                CountryCode = createCountryDto.CountryCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Country created successfully with ID {country.Id}");

            return MapToDto(country);
        }

        public async Task<CountryDto> UpdateCountryAsync(int id, UpdateCountryDto updateCountryDto)
        {
            _logger.LogInformation($"Updating country with ID {id}");

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                _logger.LogWarning($"Country with ID {id} not found");
                throw new KeyNotFoundException($"Country with ID {id} not found");
            }

            // Check for duplicate country name (excluding current country)
            var countryNameExists = await _context.Countries
                .AnyAsync(c => c.Id != id && c.CountryName == updateCountryDto.CountryName);

            if (countryNameExists)
            {
                throw new ArgumentException($"Country '{updateCountryDto.CountryName}' already exists");
            }

            // Check for duplicate country code (excluding current country)
            var countryCodeExists = await _context.Countries
                .AnyAsync(c => c.Id != id && c.CountryCode == updateCountryDto.CountryCode);

            if (countryCodeExists)
            {
                throw new ArgumentException($"Country code '{updateCountryDto.CountryCode}' already exists");
            }

            country.CountryName = updateCountryDto.CountryName;
            country.CountryCode = updateCountryDto.CountryCode;

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Country with ID {id} updated successfully");

            return MapToDto(country);
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            _logger.LogInformation($"Deleting country with ID {id}");

            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                _logger.LogWarning($"Country with ID {id} not found");
                throw new KeyNotFoundException($"Country with ID {id} not found");
            }

            // Check if there are any states associated with this country
            var hasStates = await _context.States.AnyAsync(s => s.CountryId == id);
            if (hasStates)
            {
                throw new InvalidOperationException($"Cannot delete country with ID {id} as it has associated states");
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Country with ID {id} deleted successfully");

            return true;
        }

        private CountryDto MapToDto(Country country)
        {
            return new CountryDto
            {
                Id = country.Id,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CreatedAt = country.CreatedAt
            };
        }
    }
}
