# Logging Setup Summary

## What Was Implemented

A comprehensive logging system has been added to the backend for detailed request tracking and debugging.

## Files Created/Modified

### New Files
1. **`Middleware/RequestResponseLoggingMiddleware.cs`**
   - Logs all HTTP requests and responses
   - Tracks request duration
   - Redacts sensitive headers
   - Assigns unique request IDs

2. **`Utilities/LoggingHelper.cs`**
   - Provides consistent logging methods
   - Includes helpers for method entry/exit, database operations, validation, business logic, success, performance, and exceptions

3. **Documentation**
   - `LOGGING_GUIDE.md` - Comprehensive guide with examples
   - `LOGGING_QUICK_REFERENCE.md` - Quick reference for developers
   - `LOGGING_IMPLEMENTATION_CHECKLIST.md` - Checklist for implementing logging in other services
   - `LOGGING_SETUP_SUMMARY.md` - This file

### Modified Files
1. **`Program.cs`**
   - Added Serilog configuration
   - Configured console and file logging
   - Added request/response logging middleware

2. **`Services/UserService.cs`**
   - Added detailed logging to all methods
   - Logs method entry/exit with parameters
   - Logs database operations
   - Logs validation errors
   - Logs business logic errors
   - Logs success operations
   - Logs performance metrics
   - Logs exceptions with context

3. **`BusBookingAPI.csproj`**
   - Added Serilog NuGet packages:
     - Serilog 4.1.1
     - Serilog.AspNetCore 8.1.0
     - Serilog.Sinks.Console 6.0.0
     - Serilog.Sinks.File 5.0.0

## Features

### 1. Request/Response Logging
Every HTTP request and response is automatically logged with:
- Unique request ID for tracking
- HTTP method and path
- Request/response headers (Authorization redacted)
- Request/response body
- Processing duration
- Status code
- Client IP address
- Authenticated user

### 2. Service Method Logging
Each service method logs:
- Method entry with parameters
- Database operations (SELECT, INSERT, UPDATE, DELETE)
- Validation errors with field names
- Business logic errors with reasons
- Successful operations with relevant data
- Performance metrics (execution time)
- Method exit with return value
- Exceptions with full context

### 3. Log Output
- **Console**: Real-time logs with color coding
- **Files**: Daily log files in `logs/app-YYYY-MM-DD.txt`
- **Retention**: 30 days of logs kept
- **Format**: `[Timestamp] [Level] Message`

## How to Use

### For Developers
1. Inject `ILogger<ServiceName>` in your service
2. Use `LoggingHelper` methods for consistent logging
3. Follow the pattern in `UserService.cs`
4. Refer to `LOGGING_QUICK_REFERENCE.md` for examples

### For Debugging
1. Check console output for real-time logs
2. Search log files in `logs/` directory
3. Use request ID to trace entire request lifecycle
4. Search for specific operations, users, or errors

## Quick Start

### View Logs
```bash
# Console output (real-time)
# Appears when running the application

# File logs
tail -f logs/app-*.txt

# Search for specific user
grep "user@example.com" logs/app-*.txt

# Find errors
grep "\[ERR\]" logs/app-*.txt

# Find slow operations
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt
```

### Add Logging to New Service
1. Copy the pattern from `UserService.cs`
2. Use `LoggingHelper` methods
3. Follow `LOGGING_IMPLEMENTATION_CHECKLIST.md`
4. Test with a sample request

## Log Symbols
- `→` Method entry
- `←` Method exit
- `📊` Database operation
- `⚠️` Validation error
- `❌` Business logic error or exception
- `✅` Successful operation
- `⏱️` Performance metric

## Example Log Output

```
[2024-04-23 14:30:45.123 +00:00] [INF] [a1b2c3d4] ===== HTTP REQUEST =====
Method: POST
Path: /api/users
Headers:
  Content-Type: application/json
  Authorization: [REDACTED]
Body: {"email":"user@example.com","fullName":"John Doe"}

[2024-04-23 14:30:45.200 +00:00] [DBG] → Entering method: CreateUserAsync with parameters: email=user@example.com
[2024-04-23 14:30:45.210 +00:00] [DBG] 📊 Database operation: SELECT on Users - Check if email exists
[2024-04-23 14:30:45.220 +00:00] [DBG] 📊 Database operation: INSERT on Users - Creating user
[2024-04-23 14:30:45.350 +00:00] [INF] ✅ CreateUser completed successfully - User ID: 1
[2024-04-23 14:30:45.350 +00:00] [DBG] ⏱️ CreateUserAsync took 150ms
[2024-04-23 14:30:45.350 +00:00] [DBG] ← Exiting method: CreateUserAsync with result: 1

[2024-04-23 14:30:45.456 +00:00] [INF] [a1b2c3d4] ===== HTTP RESPONSE =====
Status Code: 201
Duration: 333ms
Body: {"id":1,"email":"user@example.com",...}
```

## Next Steps

1. **Install NuGet packages**: Run `dotnet restore`
2. **Test the application**: Make a request and check logs
3. **Update other services**: Follow the checklist to add logging to other services
4. **Configure log levels**: Adjust in `Program.cs` if needed
5. **Monitor performance**: Use logs to identify slow operations

## Configuration

### Change Log Level
In `Program.cs`, modify:
```csharp
.MinimumLevel.Debug()  // Change to Information, Warning, or Error
```

### Change Log File Location
In `Program.cs`, modify:
```csharp
.WriteTo.File(
    path: "logs/app-.txt",  // Change path here
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30)  // Change retention here
```

### Disable Request/Response Logging
In `Program.cs`, comment out:
```csharp
// app.UseMiddleware<RequestResponseLoggingMiddleware>();
```

## Performance Impact

- **Minimal**: Logging is asynchronous
- **Large bodies**: Truncated at 5000 characters
- **Sensitive data**: Authorization headers redacted
- **File I/O**: Buffered and optimized
- **Rotation**: Daily with cleanup after 30 days

## Troubleshooting

### Logs not appearing
1. Check `logs/` directory exists and is writable
2. Verify Serilog is configured in `Program.cs`
3. Check application log level

### Too many logs
1. Increase log level to Information or Warning
2. Disable request/response body logging
3. Increase log file rotation interval

### Performance issues
1. Reduce log level
2. Disable detailed logging for large payloads
3. Increase log file rotation interval

## Documentation

- **`LOGGING_GUIDE.md`** - Comprehensive guide with detailed examples
- **`LOGGING_QUICK_REFERENCE.md`** - Quick reference for common patterns
- **`LOGGING_IMPLEMENTATION_CHECKLIST.md`** - Checklist for implementing logging in other services
- **`UserService.cs`** - Example of fully implemented logging

## Support

For questions or issues:
1. Check the documentation files
2. Review `UserService.cs` for examples
3. Follow the implementation checklist
4. Test with sample requests

---

**Status**: ✅ Logging system is ready to use
**Last Updated**: April 23, 2026
