using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace BusBookingAPI.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ValidatePasswordAsync(int userId, string password);
    }

    public class UserService : IUserService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(BusBookingDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching user with ID {id}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found");
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            return MapToDto(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _context.Users.ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation($"Fetching user with email {email}");

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                _logger.LogWarning($"User with email {email} not found");
                throw new KeyNotFoundException($"User with email {email} not found");
            }

            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation($"Creating new user with email {createUserDto.Email}");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(createUserDto.FullName))
            {
                throw new ArgumentException("Full name is required");
            }

            if (string.IsNullOrWhiteSpace(createUserDto.Email))
            {
                throw new ArgumentException("Email is required");
            }

            if (string.IsNullOrWhiteSpace(createUserDto.Password))
            {
                throw new ArgumentException("Password is required");
            }

            if (createUserDto.Password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long");
            }

            // Check if email already exists
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == createUserDto.Email.ToLower());
            if (emailExists)
            {
                throw new ArgumentException($"A user with email {createUserDto.Email} already exists");
            }

            // Check if phone number already exists (if provided)
            if (!string.IsNullOrWhiteSpace(createUserDto.PhoneNumber))
            {
                var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == createUserDto.PhoneNumber);
                if (phoneExists)
                {
                    throw new ArgumentException($"A user with phone number {createUserDto.PhoneNumber} already exists");
                }
            }

            // Hash password
            var passwordHash = HashPassword(createUserDto.Password);

            var user = new User
            {
                FullName = createUserDto.FullName,
                Email = createUserDto.Email,
                PhoneNumber = createUserDto.PhoneNumber,
                PasswordHash = passwordHash,
                DateOfBirth = createUserDto.DateOfBirth,
                Address = createUserDto.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User created successfully with ID {user.Id}");

            return await GetUserByIdAsync(user.Id);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation($"Updating user with ID {id}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found");
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(updateUserDto.FullName))
            {
                throw new ArgumentException("Full name is required");
            }

            // Check if phone number is being changed and if it already exists
            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber) &&
                user.PhoneNumber != updateUserDto.PhoneNumber)
            {
                var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == updateUserDto.PhoneNumber && u.Id != id);
                if (phoneExists)
                {
                    throw new ArgumentException($"A user with phone number {updateUserDto.PhoneNumber} already exists");
                }
            }

            user.FullName = updateUserDto.FullName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.DateOfBirth = updateUserDto.DateOfBirth;
            user.Address = updateUserDto.Address;
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {id} updated successfully");

            return await GetUserByIdAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogInformation($"Deleting user with ID {id}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found");
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            // Check if user has active bookings
            var hasActiveBookings = await _context.Bookings.AnyAsync(b => b.UserId == id && b.TravelStatus == "active");
            if (hasActiveBookings)
            {
                throw new InvalidOperationException($"Cannot delete user with ID {id} because they have active bookings");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with ID {id} deleted successfully");

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.LogInformation($"Changing password for user with ID {userId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            // Verify current password
            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                throw new ArgumentException("Current password is incorrect");
            }

            if (newPassword.Length < 6)
            {
                throw new ArgumentException("New password must be at least 6 characters long");
            }

            // Hash new password
            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password changed successfully for user with ID {userId}");

            return true;
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            _logger.LogInformation($"Validating password for user with ID {userId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            return VerifyPassword(password, user.PasswordHash);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
