# Backend Logging Guide

## Overview

The backend now includes comprehensive logging for debugging and monitoring. Logs are written to both console and file with detailed request/response information, database operations, and performance metrics.

## Logging Configuration

### Log Files Location
- **Daily logs**: `logs/app-YYYY-MM-DD.txt`
- **Retention**: 30 days of logs are kept
- **Format**: `[Timestamp] [Level] Message`

### Log Levels
- **Debug**: Detailed method entry/exit, database operations
- **Information**: Successful operations, business logic flow
- **Warning**: Validation errors, business logic issues
- **Error**: Exceptions and critical failures

## Features

### 1. Request/Response Logging Middleware
Every HTTP request and response is automatically logged with:
- **Request ID**: Unique 8-character ID for tracking
- **Method & Path**: HTTP method and endpoint
- **Headers**: All headers (Authorization redacted for security)
- **Body**: Request/response body (limited to 5000 chars)
- **Duration**: Total request processing time
- **Status Code**: HTTP response status
- **Remote IP**: Client IP address
- **User**: Authenticated user (if available)

**Example Log Output:**
```
[2024-04-23 14:30:45.123 +00:00] [INF] [a1b2c3d4] ===== HTTP REQUEST =====
Method: POST
Path: /api/users
Host: localhost:5000
Headers:
  Content-Type: application/json
  Authorization: [REDACTED]
Body: {"email":"user@example.com","fullName":"John Doe"}
Remote IP: 127.0.0.1
User: Anonymous

[2024-04-23 14:30:45.456 +00:00] [INF] [a1b2c3d4] ===== HTTP RESPONSE =====
Status Code: 201
Duration: 333ms
Headers:
  Content-Type: application/json
Body: {"id":1,"email":"user@example.com",...}
```

### 2. Service Method Logging
Each service method logs:
- **Method Entry**: Method name and parameters
- **Database Operations**: SELECT, INSERT, UPDATE, DELETE with details
- **Validation Errors**: Field name and error message
- **Business Logic Errors**: Operation and reason
- **Success**: Operation completion with relevant data
- **Performance**: Execution time in milliseconds
- **Method Exit**: Return value

**Example Service Log:**
```
[2024-04-23 14:30:45.200 +00:00] [DBG] → Entering method: CreateUserAsync with parameters: email=user@example.com
[2024-04-23 14:30:45.210 +00:00] [DBG] 📊 Database operation: SELECT on Users - Check if email exists: user@example.com
[2024-04-23 14:30:45.220 +00:00] [DBG] 📊 Database operation: INSERT on Users - Creating user: user@example.com
[2024-04-23 14:30:45.350 +00:00] [INF] ✅ CreateUser completed successfully - User ID: 1, Email: user@example.com
[2024-04-23 14:30:45.350 +00:00] [DBG] ⏱️ CreateUserAsync took 150ms
[2024-04-23 14:30:45.350 +00:00] [DBG] ← Exiting method: CreateUserAsync with result: 1
```

## Using LoggingHelper

The `LoggingHelper` utility provides consistent logging patterns:

### Method Entry/Exit
```csharp
LoggingHelper.LogMethodEntry(_logger, nameof(MyMethod), 
    new Dictionary<string, object?> { { "id", 123 } });

// ... method logic ...

LoggingHelper.LogMethodExit(_logger, nameof(MyMethod), result);
```

### Database Operations
```csharp
LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", "WHERE Id = 123");
LoggingHelper.LogDatabaseOperation(_logger, "INSERT", "Bookings", "New booking created");
```

### Validation Errors
```csharp
LoggingHelper.LogValidationError(_logger, "Email", "Email format is invalid");
```

### Business Logic Errors
```csharp
LoggingHelper.LogBusinessLogicError(_logger, "CreateUser", "Email already exists");
```

### Success Operations
```csharp
LoggingHelper.LogSuccess(_logger, "CreateUser", "User ID: 1, Email: user@example.com");
```

### Performance Metrics
```csharp
var stopwatch = Stopwatch.StartNew();
// ... operation ...
stopwatch.Stop();
LoggingHelper.LogPerformance(_logger, "GetAllUsers", stopwatch.ElapsedMilliseconds);
```

### Exception Logging
```csharp
try
{
    // ... operation ...
}
catch (Exception ex)
{
    LoggingHelper.LogException(_logger, ex, "CreateUser", 
        new Dictionary<string, object?> { { "email", email } });
    throw;
}
```

## Debugging Tips

### 1. Find Request by ID
Search logs for the request ID to trace the entire request lifecycle:
```
grep "a1b2c3d4" logs/app-2024-04-23.txt
```

### 2. Find Slow Operations
Search for performance warnings:
```
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-2024-04-23.txt
```

### 3. Find Errors
```
grep "\[ERR\]" logs/app-2024-04-23.txt
```

### 4. Find Specific User Operations
```
grep "user@example.com" logs/app-2024-04-23.txt
```

### 5. Find Database Operations
```
grep "📊 Database operation" logs/app-2024-04-23.txt
```

## Adding Logging to New Services

When creating a new service, follow this pattern:

```csharp
using BusBookingAPI.Utilities;
using System.Diagnostics;

public class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public async Task<MyDto> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        LoggingHelper.LogMethodEntry(_logger, nameof(GetByIdAsync), 
            new Dictionary<string, object?> { { "id", id } });

        try
        {
            LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "MyTable", $"WHERE Id = {id}");
            var item = await _context.MyTable.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                stopwatch.Stop();
                LoggingHelper.LogBusinessLogicError(_logger, "GetById", $"Item {id} not found");
                throw new KeyNotFoundException($"Item {id} not found");
            }

            stopwatch.Stop();
            LoggingHelper.LogSuccess(_logger, "GetById", $"Item ID: {item.Id}");
            LoggingHelper.LogPerformance(_logger, "GetByIdAsync", stopwatch.ElapsedMilliseconds);
            LoggingHelper.LogMethodExit(_logger, nameof(GetByIdAsync), item.Id);

            return MapToDto(item);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LoggingHelper.LogException(_logger, ex, "GetById", 
                new Dictionary<string, object?> { { "id", id } });
            throw;
        }
    }
}
```

## Log Symbols Reference

- `→` Method entry
- `←` Method exit
- `📊` Database operation
- `⚠️` Validation error
- `❌` Business logic error or exception
- `✅` Successful operation
- `⏱️` Performance metric

## Performance Considerations

- Logs are written asynchronously to minimize impact
- Large response bodies (>5000 chars) are truncated
- Sensitive headers (Authorization) are redacted
- Log files are rotated daily and old logs are cleaned up after 30 days

## Troubleshooting

### Logs Not Appearing
1. Check that `logs/` directory exists and is writable
2. Verify Serilog is configured in `Program.cs`
3. Check application log level in `appsettings.json`

### Too Many Logs
Adjust the minimum log level in `Program.cs`:
```csharp
.MinimumLevel.Information()  // Change from Debug to Information
```

### Performance Issues
If logging is impacting performance:
1. Reduce log level to Information or Warning
2. Disable request/response body logging for large payloads
3. Increase log file rotation interval
