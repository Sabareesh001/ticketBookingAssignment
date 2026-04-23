# Logging Quick Reference

## Quick Start

### 1. Inject Logger
```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
}
```

### 2. Use LoggingHelper
```csharp
using BusBookingAPI.Utilities;
using System.Diagnostics;

// Track execution time
var stopwatch = Stopwatch.StartNew();

// Log method entry
LoggingHelper.LogMethodEntry(_logger, nameof(MyMethod), 
    new Dictionary<string, object?> { { "id", 123 } });

// Log database operations
LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", "WHERE Id = 123");

// Log validation errors
LoggingHelper.LogValidationError(_logger, "Email", "Invalid format");

// Log business logic errors
LoggingHelper.LogBusinessLogicError(_logger, "CreateUser", "Email already exists");

// Log success
LoggingHelper.LogSuccess(_logger, "CreateUser", "User ID: 1");

// Log performance
stopwatch.Stop();
LoggingHelper.LogPerformance(_logger, "MyMethod", stopwatch.ElapsedMilliseconds);

// Log method exit
LoggingHelper.LogMethodExit(_logger, nameof(MyMethod), result);

// Log exceptions
try { /* ... */ }
catch (Exception ex)
{
    LoggingHelper.LogException(_logger, ex, "MyOperation", 
        new Dictionary<string, object?> { { "id", 123 } });
    throw;
}
```

## Common Patterns

### Pattern 1: Simple CRUD Operation
```csharp
public async Task<UserDto> GetUserByIdAsync(int id)
{
    var stopwatch = Stopwatch.StartNew();
    LoggingHelper.LogMethodEntry(_logger, nameof(GetUserByIdAsync), 
        new Dictionary<string, object?> { { "id", id } });

    try
    {
        LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", $"WHERE Id = {id}");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            stopwatch.Stop();
            LoggingHelper.LogBusinessLogicError(_logger, "GetUserById", $"User {id} not found");
            throw new KeyNotFoundException($"User {id} not found");
        }

        stopwatch.Stop();
        LoggingHelper.LogSuccess(_logger, "GetUserById", $"User ID: {user.Id}");
        LoggingHelper.LogPerformance(_logger, "GetUserByIdAsync", stopwatch.ElapsedMilliseconds);
        LoggingHelper.LogMethodExit(_logger, nameof(GetUserByIdAsync), user.Id);

        return MapToDto(user);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        LoggingHelper.LogException(_logger, ex, "GetUserById", 
            new Dictionary<string, object?> { { "id", id } });
        throw;
    }
}
```

### Pattern 2: Create with Validation
```csharp
public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
{
    var stopwatch = Stopwatch.StartNew();
    LoggingHelper.LogMethodEntry(_logger, nameof(CreateUserAsync), 
        new Dictionary<string, object?> { { "email", dto.Email } });

    try
    {
        // Validate
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            LoggingHelper.LogValidationError(_logger, "Email", "Email is required");
            throw new ArgumentException("Email is required");
        }

        // Check duplicates
        LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", 
            $"Check if email exists: {dto.Email}");
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
        {
            LoggingHelper.LogBusinessLogicError(_logger, "CreateUser", 
                $"Email {dto.Email} already exists");
            throw new ArgumentException("Email already exists");
        }

        // Create
        var user = new User { Email = dto.Email };
        LoggingHelper.LogDatabaseOperation(_logger, "INSERT", "Users", 
            $"Creating user: {dto.Email}");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        stopwatch.Stop();
        LoggingHelper.LogSuccess(_logger, "CreateUser", $"User ID: {user.Id}");
        LoggingHelper.LogPerformance(_logger, "CreateUserAsync", stopwatch.ElapsedMilliseconds);
        LoggingHelper.LogMethodExit(_logger, nameof(CreateUserAsync), user.Id);

        return MapToDto(user);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        LoggingHelper.LogException(_logger, ex, "CreateUser", 
            new Dictionary<string, object?> { { "email", dto.Email } });
        throw;
    }
}
```

## Viewing Logs

### Console Output
Logs appear in real-time in the console when running the application.

### File Logs
Located in `logs/app-YYYY-MM-DD.txt`

### Search Examples
```bash
# Find all operations for a specific user
grep "user@example.com" logs/app-*.txt

# Find all errors
grep "\[ERR\]" logs/app-*.txt

# Find slow operations (>1 second)
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt

# Find database operations
grep "📊" logs/app-*.txt

# Find validation errors
grep "⚠️" logs/app-*.txt

# Find specific request by ID
grep "a1b2c3d4" logs/app-*.txt
```

## Log Symbols
- `→` Method entry
- `←` Method exit
- `📊` Database operation
- `⚠️` Validation error
- `❌` Business logic error or exception
- `✅` Successful operation
- `⏱️` Performance metric

## Tips

1. **Always wrap in try-catch** - Ensures exceptions are logged with context
2. **Use stopwatch for performance** - Helps identify slow operations
3. **Include relevant context** - Pass parameters to LogMethodEntry
4. **Log before database calls** - Helps debug query issues
5. **Log business logic errors** - Helps understand why operations failed
6. **Keep messages concise** - Easier to search and read
7. **Use consistent patterns** - Makes logs predictable and searchable

## Performance Impact

- Minimal - Logging is asynchronous
- Large bodies (>5000 chars) are truncated
- Sensitive data (Authorization headers) is redacted
- Can be disabled by changing log level to Warning or Error
