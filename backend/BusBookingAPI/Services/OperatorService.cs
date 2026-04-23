using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public interface IOperatorService
    {
        Task<OperatorDto> GetOperatorByIdAsync(int id);
        Task<List<OperatorDto>> GetAllOperatorsAsync();
        Task<OperatorDto> CreateOperatorAsync(CreateOperatorDto createOperatorDto);
        Task<OperatorDto> UpdateOperatorAsync(int id, UpdateOperatorDto updateOperatorDto);
        Task<bool> DeleteOperatorAsync(int id);
    }

    public class OperatorService : IOperatorService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<OperatorService> _logger;

        public OperatorService(BusBookingDbContext context, ILogger<OperatorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperatorDto> GetOperatorByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching operator with ID {id}");

            var op = await _context.BusOperators.FirstOrDefaultAsync(o => o.Id == id);

            if (op == null)
            {
                _logger.LogWarning($"Operator with ID {id} not found");
                throw new KeyNotFoundException($"Operator with ID {id} not found");
            }

            return MapToDto(op);
        }

        public async Task<List<OperatorDto>> GetAllOperatorsAsync()
        {
            _logger.LogInformation("Fetching all operators");

            var operators = await _context.BusOperators.ToListAsync();

            return operators.Select(MapToDto).ToList();
        }

        public async Task<OperatorDto> CreateOperatorAsync(CreateOperatorDto createOperatorDto)
        {
            _logger.LogInformation($"Creating new operator with name {createOperatorDto.OperatorName}");

            // Validate that email is unique
            var emailExists = await _context.BusOperators.AnyAsync(o => o.Email == createOperatorDto.Email);
            if (emailExists)
            {
                throw new ArgumentException($"An operator with email {createOperatorDto.Email} already exists");
            }

            // Validate that license number is unique
            var licenseExists = await _context.BusOperators.AnyAsync(o => o.LicenseNumber == createOperatorDto.LicenseNumber);
            if (licenseExists)
            {
                throw new ArgumentException($"An operator with license number {createOperatorDto.LicenseNumber} already exists");
            }

            var op = new BusOperator
            {
                OperatorName = createOperatorDto.OperatorName,
                Email = createOperatorDto.Email,
                PhoneNumber = createOperatorDto.PhoneNumber,
                LicenseNumber = createOperatorDto.LicenseNumber,
                Address = createOperatorDto.Address,
                PasswordHash = createOperatorDto.PasswordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BusOperators.Add(op);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Operator created successfully with ID {op.Id}");

            return await GetOperatorByIdAsync(op.Id);
        }

        public async Task<OperatorDto> UpdateOperatorAsync(int id, UpdateOperatorDto updateOperatorDto)
        {
            _logger.LogInformation($"Updating operator with ID {id}");

            var op = await _context.BusOperators.FirstOrDefaultAsync(o => o.Id == id);

            if (op == null)
            {
                _logger.LogWarning($"Operator with ID {id} not found");
                throw new KeyNotFoundException($"Operator with ID {id} not found");
            }

            // Validate that email is unique (if changed)
            if (op.Email != updateOperatorDto.Email)
            {
                var emailExists = await _context.BusOperators.AnyAsync(o => o.Email == updateOperatorDto.Email);
                if (emailExists)
                {
                    throw new ArgumentException($"An operator with email {updateOperatorDto.Email} already exists");
                }
            }

            // Validate that license number is unique (if changed)
            if (op.LicenseNumber != updateOperatorDto.LicenseNumber)
            {
                var licenseExists = await _context.BusOperators.AnyAsync(o => o.LicenseNumber == updateOperatorDto.LicenseNumber);
                if (licenseExists)
                {
                    throw new ArgumentException($"An operator with license number {updateOperatorDto.LicenseNumber} already exists");
                }
            }

            op.OperatorName = updateOperatorDto.OperatorName;
            op.Email = updateOperatorDto.Email;
            op.PhoneNumber = updateOperatorDto.PhoneNumber;
            op.LicenseNumber = updateOperatorDto.LicenseNumber;
            op.Address = updateOperatorDto.Address;
            op.IsActive = updateOperatorDto.IsActive;
            op.UpdatedAt = DateTime.UtcNow;

            _context.BusOperators.Update(op);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Operator with ID {id} updated successfully");

            return await GetOperatorByIdAsync(id);
        }

        public async Task<bool> DeleteOperatorAsync(int id)
        {
            _logger.LogInformation($"Deleting operator with ID {id}");

            var op = await _context.BusOperators.FirstOrDefaultAsync(o => o.Id == id);

            if (op == null)
            {
                _logger.LogWarning($"Operator with ID {id} not found");
                throw new KeyNotFoundException($"Operator with ID {id} not found");
            }

            // Check if operator has buses
            var hasBuses = await _context.Buses.AnyAsync(b => b.OperatorId == id);
            if (hasBuses)
            {
                throw new InvalidOperationException($"Cannot delete operator with ID {id} because it has buses");
            }

            // Check if operator has locations
            var hasLocations = await _context.Locations.AnyAsync(l => l.OperatorId == id);
            if (hasLocations)
            {
                throw new InvalidOperationException($"Cannot delete operator with ID {id} because it has locations");
            }

            _context.BusOperators.Remove(op);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Operator with ID {id} deleted successfully");

            return true;
        }

        private OperatorDto MapToDto(BusOperator op)
        {
            return new OperatorDto
            {
                Id = op.Id,
                OperatorName = op.OperatorName,
                Email = op.Email,
                PhoneNumber = op.PhoneNumber,
                LicenseNumber = op.LicenseNumber,
                Address = op.Address,
                IsActive = op.IsActive,
                CreatedAt = op.CreatedAt,
                UpdatedAt = op.UpdatedAt
            };
        }
    }
}
