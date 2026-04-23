using BusBookingAPI.DTOs;
using BusBookingAPI.Data;
using BusBookingAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BusBookingAPI.Services
{
    public interface IOperatorAuthService
    {
        Task<OperatorAuthResponse> LoginAsync(OperatorLoginRequest request);
        Task<OperatorAuthResponse> SignupAsync(OperatorSignupRequest request);
        string GenerateToken(OperatorDto operatorDto);
    }

    public class OperatorAuthService : IOperatorAuthService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<OperatorAuthService> _logger;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        public OperatorAuthService(BusBookingDbContext context, ILogger<OperatorAuthService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _jwtSecret = configuration["Jwt:Secret"] ?? "your-secret-key-change-this-in-production-at-least-32-characters-long";
            _jwtIssuer = configuration["Jwt:Issuer"] ?? "BusBookingAPI";
            _jwtAudience = configuration["Jwt:Audience"] ?? "BusBookingClient";
            _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public async Task<OperatorAuthResponse> LoginAsync(OperatorLoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Operator login attempt for email: {request.Email}");

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ArgumentException("Email and password are required");
                }

                var operatorEntity = await _context.BusOperators.FirstOrDefaultAsync(o => o.Email == request.Email);

                if (operatorEntity == null)
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                if (!VerifyPassword(request.Password, operatorEntity.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                var operatorDto = MapToDto(operatorEntity);
                var token = GenerateToken(operatorDto);

                return new OperatorAuthResponse
                {
                    Token = token,
                    Operator = operatorDto,
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Operator login error: {ex.Message}");
                throw;
            }
        }

        public async Task<OperatorAuthResponse> SignupAsync(OperatorSignupRequest request)
        {
            try
            {
                _logger.LogInformation($"Operator signup attempt for email: {request.Email}");

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ArgumentException("Email and password are required");
                }

                // Check if email already exists
                var emailExists = await _context.BusOperators.AnyAsync(o => o.Email == request.Email);
                if (emailExists)
                {
                    throw new ArgumentException("An operator with this email already exists");
                }

                // Check if license number already exists
                var licenseExists = await _context.BusOperators.AnyAsync(o => o.LicenseNumber == request.LicenseNumber);
                if (licenseExists)
                {
                    throw new ArgumentException("An operator with this license number already exists");
                }

                var passwordHash = HashPassword(request.Password);

                var operatorEntity = new BusOperator
                {
                    OperatorName = request.OperatorName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    LicenseNumber = request.LicenseNumber,
                    Address = request.Address,
                    PasswordHash = passwordHash,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BusOperators.Add(operatorEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Operator created successfully with ID {operatorEntity.Id}");

                var operatorDto = MapToDto(operatorEntity);
                var token = GenerateToken(operatorDto);

                return new OperatorAuthResponse
                {
                    Token = token,
                    Operator = operatorDto,
                    Message = "Signup successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Operator signup error: {ex.Message}");
                throw;
            }
        }

        public string GenerateToken(OperatorDto operatorDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, operatorDto.Id.ToString()),
                new Claim(ClaimTypes.Email, operatorDto.Email),
                new Claim(ClaimTypes.Name, operatorDto.OperatorName),
                new Claim("role", "bus_operator")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
            return hashOfInput.Equals(hash);
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
