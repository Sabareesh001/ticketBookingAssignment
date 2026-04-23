# Backend Integration Guide - Authentication

## Overview

The frontend authentication system is ready and waiting for backend endpoints. This guide shows what endpoints need to be implemented.

## Required Endpoints

### 1. Login Endpoint
**Endpoint**: `POST /api/auth/login`

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response (Success - 200)**:
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "John Doe",
    "email": "user@example.com",
    "phoneNumber": "9876543210",
    "dateOfBirth": "1990-01-15",
    "address": "123 Main St",
    "isActive": true,
    "createdAt": "2026-04-23T10:00:00Z",
    "updatedAt": "2026-04-23T10:00:00Z"
  }
}
```

**Response (Error - 401)**:
```json
{
  "success": false,
  "message": "Invalid email or password"
}
```

### 2. Sign Up Endpoint
**Endpoint**: `POST /api/user`

**Request Body**:
```json
{
  "fullName": "John Doe",
  "email": "user@example.com",
  "phoneNumber": "9876543210",
  "password": "password123",
  "dateOfBirth": "1990-01-15",
  "address": "123 Main St"
}
```

**Response (Success - 201)**:
```json
{
  "success": true,
  "message": "User created successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "John Doe",
    "email": "user@example.com",
    "phoneNumber": "9876543210",
    "dateOfBirth": "1990-01-15",
    "address": "123 Main St",
    "isActive": true,
    "createdAt": "2026-04-23T10:00:00Z",
    "updatedAt": "2026-04-23T10:00:00Z"
  }
}
```

**Response (Error - 400)**:
```json
{
  "success": false,
  "message": "Email already exists"
}
```

### 3. Forgot Password Endpoint
**Endpoint**: `POST /api/auth/forgot-password`

**Request Body**:
```json
{
  "email": "user@example.com"
}
```

**Response (Success - 200)**:
```json
{
  "success": true,
  "message": "Password reset link sent to your email"
}
```

**Response (Error - 404)**:
```json
{
  "success": false,
  "message": "User not found"
}
```

**Backend Action**:
- Generate a reset token (valid for 24 hours)
- Send email with reset link: `http://frontend-url/reset-password?email=user@example.com&token=reset_token`
- Store reset token in database with expiration

### 4. Reset Password Endpoint
**Endpoint**: `POST /api/auth/reset-password`

**Request Body**:
```json
{
  "email": "user@example.com",
  "newPassword": "newpassword123"
}
```

**Response (Success - 200)**:
```json
{
  "success": true,
  "message": "Password reset successfully"
}
```

**Response (Error - 400)**:
```json
{
  "success": false,
  "message": "Invalid or expired reset token"
}
```

**Backend Action**:
- Validate reset token
- Check token expiration
- Hash new password
- Update user password
- Clear reset token

### 5. Change Password Endpoint
**Endpoint**: `POST /api/user/{id}/change-password`

**Headers**:
```
Authorization: Bearer {jwt_token}
```

**Request Body**:
```json
{
  "currentPassword": "oldpassword123",
  "newPassword": "newpassword123"
}
```

**Response (Success - 200)**:
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

**Response (Error - 401)**:
```json
{
  "success": false,
  "message": "Current password is incorrect"
}
```

## JWT Token Requirements

### Token Structure
The JWT token should include:
```json
{
  "sub": "1",
  "email": "user@example.com",
  "role": "user",
  "iat": 1682251200,
  "exp": 1682337600
}
```

### Token Claims
- `sub` - User ID (as string)
- `email` - User email
- `role` - User role (user, admin, bus_operator)
- `iat` - Issued at timestamp
- `exp` - Expiration timestamp (typically 24 hours)

### Token Validation
- Verify signature using your secret key
- Check expiration time
- Validate claims

## Implementation Checklist

### Authentication Controller
- [ ] Create `AuthController` class
- [ ] Implement `Login` action
- [ ] Implement `ForgotPassword` action
- [ ] Implement `ResetPassword` action

### Authentication Service
- [ ] Create `IAuthService` interface
- [ ] Create `AuthService` class
- [ ] Implement JWT token generation
- [ ] Implement password hashing (BCrypt recommended)
- [ ] Implement password verification
- [ ] Implement reset token generation
- [ ] Implement reset token validation

### Database Changes
- [ ] Add `reset_token` column to users table
- [ ] Add `reset_token_expiry` column to users table
- [ ] Create index on `reset_token` for performance

### Middleware/Filters
- [ ] Create JWT authentication middleware
- [ ] Create authorization filter for role-based access
- [ ] Add CORS configuration

### Email Service (Optional but Recommended)
- [ ] Create email service
- [ ] Implement password reset email template
- [ ] Send email on forgot password request

## Example Implementation (C#)

### AuthController
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (!response.Success)
            return Unauthorized(response);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<AuthResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await _authService.ForgotPasswordAsync(request);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<AuthResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var response = await _authService.ResetPasswordAsync(request);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }
}
```

### AuthService
```csharp
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return new AuthResponse 
            { 
                Success = false, 
                Message = "Invalid email or password" 
            };
        }

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = MapToUserDto(user)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", "user") // Determine role from user
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

## Frontend Configuration

### Update API URL
In `frontend/bus-booking/src/app/services/auth.service.ts`:
```typescript
private apiUrl = 'http://localhost:5001/api';
```

### CORS Configuration
Backend should allow requests from frontend:
```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Testing

### Test Login
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'
```

### Test Sign Up
```bash
curl -X POST http://localhost:5001/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "fullName":"John Doe",
    "email":"user@example.com",
    "phoneNumber":"9876543210",
    "password":"password123",
    "dateOfBirth":"1990-01-15",
    "address":"123 Main St"
  }'
```

### Test Protected Endpoint
```bash
curl -X GET http://localhost:5001/api/user/1 \
  -H "Authorization: Bearer {jwt_token}"
```

## Security Considerations

1. **Password Hashing** - Use BCrypt or similar
2. **JWT Secret** - Use strong, random secret key
3. **HTTPS** - Use HTTPS in production
4. **Token Expiration** - Set reasonable expiration (24 hours)
5. **Refresh Tokens** - Consider implementing refresh tokens
6. **Rate Limiting** - Limit login attempts
7. **CORS** - Configure CORS properly
8. **Input Validation** - Validate all inputs
9. **SQL Injection** - Use parameterized queries
10. **XSS Protection** - Sanitize outputs

## Database Schema Updates

### Add Reset Token Columns
```sql
ALTER TABLE users ADD COLUMN reset_token VARCHAR(255);
ALTER TABLE users ADD COLUMN reset_token_expiry TIMESTAMP;

CREATE INDEX idx_users_reset_token ON users(reset_token);
```

## Troubleshooting

### CORS Errors
- Check CORS configuration in backend
- Verify frontend URL is allowed
- Check request headers

### Token Not Working
- Verify JWT secret is correct
- Check token expiration
- Verify token format

### Login Failing
- Check password hashing
- Verify user exists
- Check database connection

### Email Not Sending
- Check email service configuration
- Verify SMTP settings
- Check email template

## Next Steps

1. Implement all required endpoints
2. Test with frontend
3. Add email service for password reset
4. Implement refresh token mechanism
5. Add 2FA for enhanced security
6. Set up logging and monitoring
7. Configure rate limiting
8. Deploy to production

## Support

For questions or issues:
1. Check the frontend documentation
2. Review the API response format
3. Check backend logs
4. Verify database schema
5. Test endpoints with curl or Postman
