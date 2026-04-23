using BusBookingAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BusBookingAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        public AuthService(IUserService userService, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _jwtSecret = configuration["Jwt:Secret"] ?? "your-secret-key-change-this-in-production-at-least-32-characters-long";
            _jwtIssuer = configuration["Jwt:Issuer"] ?? "BusBookingAPI";
            _jwtAudience = configuration["Jwt:Audience"] ?? "BusBookingClient";
            _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {request.Email}");

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new ArgumentException("Email and password are required");
                }

                var user = await _userService.GetUserByEmailAsync(request.Email);
                
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                var isPasswordValid = await _userService.ValidatePasswordAsync(user.Id, request.Password);
                
                if (!isPasswordValid)
                {
                    throw new UnauthorizedAccessException("Invalid email or password");
                }

                var token = GenerateToken(user);

                return new AuthResponse
                {
                    Token = token,
                    User = user,
                    Message = "Login successful"
                };
            }
            catch (KeyNotFoundException)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthResponse> SignupAsync(CreateUserDto request)
        {
            try
            {
                _logger.LogInformation($"Signup attempt for email: {request.Email}");

                var user = await _userService.CreateUserAsync(request);
                var token = GenerateToken(user);

                return new AuthResponse
                {
                    Token = token,
                    User = user,
                    Message = "Signup successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Signup error: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                _logger.LogInformation($"Forgot password request for email: {request.Email}");

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    throw new ArgumentException("Email is required");
                }

                var user = await _userService.GetUserByEmailAsync(request.Email);
                
                if (user == null)
                {
                    // For security, don't reveal if email exists
                    return new AuthResponse
                    {
                        Message = "If an account exists with this email, a password reset link will be sent"
                    };
                }

                // Generate a reset token (in production, this should be stored in database with expiration)
                var resetToken = GenerateResetToken(user.Id);

                // In production, send this token via email
                _logger.LogInformation($"Reset token generated for user {user.Id}: {resetToken}");

                return new AuthResponse
                {
                    Message = "If an account exists with this email, a password reset link will be sent"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Forgot password error: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                _logger.LogInformation($"Reset password request for email: {request.Email}");

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    throw new ArgumentException("Email and new password are required");
                }

                var user = await _userService.GetUserByEmailAsync(request.Email);
                
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                // In production, validate the reset token here
                // For now, we'll just reset the password
                var result = await _userService.ChangePasswordAsync(user.Id, "", request.NewPassword);

                if (!result)
                {
                    throw new InvalidOperationException("Failed to reset password");
                }

                var updatedUser = await _userService.GetUserByIdAsync(user.Id);
                var token = GenerateToken(updatedUser);

                return new AuthResponse
                {
                    Token = token,
                    User = updatedUser,
                    Message = "Password reset successful"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reset password error: {ex.Message}");
                throw;
            }
        }

        public string GenerateToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("role", "user") // Default role, can be extended
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

        private string GenerateResetToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("type", "reset")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Reset token valid for 1 hour
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
