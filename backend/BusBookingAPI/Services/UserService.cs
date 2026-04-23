using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using BusBookingAPI.Utilities;
using System.Diagnostics;
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
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(GetUserByIdAsync), new Dictionary<string, object?> { { "id", id } });

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {id}");
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "GetUserById", $"User with ID {id} not found");
                    throw new KeyNotFoundException($"User with ID {id} not found");
                }

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "GetUserById", $"User ID: {user.Id}, Email: {user.Email}");
                LoggingHelper.LogPerformance(_logger, "GetUserByIdAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(GetUserByIdAsync), user.Id);

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "GetUserById", new Dictionary<string, object?> { { "id", id } });
                throw;
            }
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(GetAllUsersAsync));

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", "Fetching all users");
                var users = await _context.Users.ToListAsync();

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "GetAllUsers", $"Retrieved {users.Count} users");
                LoggingHelper.LogPerformance(_logger, "GetAllUsersAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(GetAllUsersAsync), users.Count);

                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "GetAllUsers");
                throw;
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(GetUserByEmailAsync), new Dictionary<string, object?> { { "email", email } });

            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    LoggingHelper.LogValidationError(_logger, "email", "Email cannot be empty");
                    throw new ArgumentException("Email cannot be empty");
                }

                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Email = {email}");
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "GetUserByEmail", $"User with email {email} not found");
                    throw new KeyNotFoundException($"User with email {email} not found");
                }

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "GetUserByEmail", $"User ID: {user.Id}");
                LoggingHelper.LogPerformance(_logger, "GetUserByEmailAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(GetUserByEmailAsync), user.Id);

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "GetUserByEmail", new Dictionary<string, object?> { { "email", email } });
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(CreateUserAsync), new Dictionary<string, object?> { { "email", createUserDto.Email } });

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(createUserDto.FullName))
                {
                    LoggingHelper.LogValidationError(_logger, "FullName", "Full name is required");
                    throw new ArgumentException("Full name is required");
                }

                if (string.IsNullOrWhiteSpace(createUserDto.Email))
                {
                    LoggingHelper.LogValidationError(_logger, "Email", "Email is required");
                    throw new ArgumentException("Email is required");
                }

                if (string.IsNullOrWhiteSpace(createUserDto.Password))
                {
                    LoggingHelper.LogValidationError(_logger, "Password", "Password is required");
                    throw new ArgumentException("Password is required");
                }

                if (createUserDto.Password.Length < 6)
                {
                    LoggingHelper.LogValidationError(_logger, "Password", "Password must be at least 6 characters long");
                    throw new ArgumentException("Password must be at least 6 characters long");
                }

                // Check if email already exists
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"Check if email exists: {createUserDto.Email}");
                var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == createUserDto.Email.ToLower());
                if (emailExists)
                {
                    LoggingHelper.LogBusinessLogicError(_logger, "CreateUser", $"Email {createUserDto.Email} already exists");
                    throw new ArgumentException($"A user with email {createUserDto.Email} already exists");
                }

                // Check if phone number already exists (if provided)
                if (!string.IsNullOrWhiteSpace(createUserDto.PhoneNumber))
                {
                    LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"Check if phone exists: {createUserDto.PhoneNumber}");
                    var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == createUserDto.PhoneNumber);
                    if (phoneExists)
                    {
                        LoggingHelper.LogBusinessLogicError(_logger, "CreateUser", $"Phone {createUserDto.PhoneNumber} already exists");
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
                    DateOfBirth = createUserDto.DateOfBirth.Kind == DateTimeKind.Unspecified 
                        ? createUserDto.DateOfBirth.ToUniversalTime() 
                        : createUserDto.DateOfBirth,
                    Address = createUserDto.Address,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                LoggingHelper.LogDatabaseOperation(_logger, "INSERT", "Users", $"Creating user: {createUserDto.Email}");
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "CreateUser", $"User ID: {user.Id}, Email: {user.Email}");
                LoggingHelper.LogPerformance(_logger, "CreateUserAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(CreateUserAsync), user.Id);

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "CreateUser", new Dictionary<string, object?> { { "email", createUserDto.Email } });
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(UpdateUserAsync), new Dictionary<string, object?> { { "id", id } });

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {id}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "UpdateUser", $"User with ID {id} not found");
                    throw new KeyNotFoundException($"User with ID {id} not found");
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(updateUserDto.FullName))
                {
                    LoggingHelper.LogValidationError(_logger, "FullName", "Full name is required");
                    throw new ArgumentException("Full name is required");
                }

                // Check if phone number is being changed and if it already exists
                if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber) &&
                    user.PhoneNumber != updateUserDto.PhoneNumber)
                {
                    LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"Check if phone exists: {updateUserDto.PhoneNumber}");
                    var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == updateUserDto.PhoneNumber && u.Id != id);
                    if (phoneExists)
                    {
                        LoggingHelper.LogBusinessLogicError(_logger, "UpdateUser", $"Phone {updateUserDto.PhoneNumber} already exists");
                        throw new ArgumentException($"A user with phone number {updateUserDto.PhoneNumber} already exists");
                    }
                }

                user.FullName = updateUserDto.FullName;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.DateOfBirth = updateUserDto.DateOfBirth.Kind == DateTimeKind.Unspecified 
                    ? updateUserDto.DateOfBirth.ToUniversalTime() 
                    : updateUserDto.DateOfBirth;
                user.Address = updateUserDto.Address;
                user.IsActive = updateUserDto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                LoggingHelper.LogDatabaseOperation(_logger, "UPDATE", "Users", $"User ID: {id}");
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "UpdateUser", $"User ID: {id}");
                LoggingHelper.LogPerformance(_logger, "UpdateUserAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(UpdateUserAsync), id);

                return await GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "UpdateUser", new Dictionary<string, object?> { { "id", id } });
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(DeleteUserAsync), new Dictionary<string, object?> { { "id", id } });

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {id}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "DeleteUser", $"User with ID {id} not found");
                    throw new KeyNotFoundException($"User with ID {id} not found");
                }

                // Check if user has active bookings
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Bookings", $"Check active bookings for user {id}");
                var hasActiveBookings = await _context.Bookings.AnyAsync(b => b.UserId == id && b.TravelStatus == "active");
                if (hasActiveBookings)
                {
                    LoggingHelper.LogBusinessLogicError(_logger, "DeleteUser", $"User {id} has active bookings");
                    throw new InvalidOperationException($"Cannot delete user with ID {id} because they have active bookings");
                }

                LoggingHelper.LogDatabaseOperation(_logger, "DELETE", "Users", $"User ID: {id}");
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "DeleteUser", $"User ID: {id}");
                LoggingHelper.LogPerformance(_logger, "DeleteUserAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(DeleteUserAsync), true);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "DeleteUser", new Dictionary<string, object?> { { "id", id } });
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(ChangePasswordAsync), new Dictionary<string, object?> { { "userId", userId } });

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {userId}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "ChangePassword", $"User with ID {userId} not found");
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                // Verify current password
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "ChangePassword", $"Current password incorrect for user {userId}");
                    throw new ArgumentException("Current password is incorrect");
                }

                if (newPassword.Length < 6)
                {
                    LoggingHelper.LogValidationError(_logger, "NewPassword", "New password must be at least 6 characters long");
                    throw new ArgumentException("New password must be at least 6 characters long");
                }

                // Hash new password
                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                LoggingHelper.LogDatabaseOperation(_logger, "UPDATE", "Users", $"Password changed for user {userId}");
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "ChangePassword", $"User ID: {userId}");
                LoggingHelper.LogPerformance(_logger, "ChangePasswordAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(ChangePasswordAsync), true);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "ChangePassword", new Dictionary<string, object?> { { "userId", userId } });
                throw;
            }
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var stopwatch = Stopwatch.StartNew();
            LoggingHelper.LogMethodEntry(_logger, nameof(ValidatePasswordAsync), new Dictionary<string, object?> { { "userId", userId } });

            try
            {
                LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {userId}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    stopwatch.Stop();
                    LoggingHelper.LogBusinessLogicError(_logger, "ValidatePassword", $"User with ID {userId} not found");
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                var isValid = VerifyPassword(password, user.PasswordHash);
                stopwatch.Stop();
                LoggingHelper.LogSuccess(_logger, "ValidatePassword", $"User ID: {userId}, Valid: {isValid}");
                LoggingHelper.LogPerformance(_logger, "ValidatePasswordAsync", stopwatch.ElapsedMilliseconds);
                LoggingHelper.LogMethodExit(_logger, nameof(ValidatePasswordAsync), isValid);

                return isValid;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoggingHelper.LogException(_logger, ex, "ValidatePassword", new Dictionary<string, object?> { { "userId", userId } });
                throw;
            }
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
