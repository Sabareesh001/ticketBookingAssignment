# DateTime UTC Conversion Fix

## Issue
When creating or updating users, the application was throwing a 500 error:

```
Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', 
only UTC is supported.
```

## Root Cause
PostgreSQL's `timestamp with time zone` type requires all DateTime values to be in UTC. When DateTime values are sent from the client without explicit timezone information, they are treated as "Unspecified" kind, which PostgreSQL rejects.

## Solution
Convert all DateTime values to UTC before saving to the database:

```csharp
// For DateOfBirth (non-nullable)
DateOfBirth = createUserDto.DateOfBirth.Kind == DateTimeKind.Unspecified 
    ? createUserDto.DateOfBirth.ToUniversalTime() 
    : createUserDto.DateOfBirth,

// For CreatedAt and UpdatedAt (always use UTC)
CreatedAt = DateTime.UtcNow,
UpdatedAt = DateTime.UtcNow
```

## Files Modified
- `Services/UserService.cs`
  - `CreateUserAsync()` - Convert DateOfBirth to UTC
  - `UpdateUserAsync()` - Convert DateOfBirth to UTC

## Testing
After the fix, user creation and updates should work correctly:

```bash
# Test user creation
curl -X POST http://localhost:5266/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "password": "password123",
    "dateOfBirth": "1990-01-15",
    "address": "123 Main St"
  }'
```

Expected response: 201 Created with user data and auth token

## Best Practices
1. Always use `DateTime.UtcNow` for server-generated timestamps
2. Convert client-provided DateTime values to UTC before database operations
3. Store all DateTime values in UTC in the database
4. Convert to local time only when displaying to users

## Related Issues
- PostgreSQL timestamp with time zone requires UTC
- Entity Framework Core with Npgsql enforces this requirement
- Client applications should send ISO 8601 formatted dates

## Prevention
For future DateTime fields:
1. Always use `DateTime.UtcNow` for server timestamps
2. Add UTC conversion for client-provided dates
3. Use `DateTimeKind.Utc` when creating DateTime values
4. Consider using `DateTimeOffset` for better timezone handling

---

**Status**: ✅ FIXED
**Build Status**: ✅ SUCCEEDED
**Test Status**: Ready to test
