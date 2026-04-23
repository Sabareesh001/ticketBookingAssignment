# Logging Troubleshooting Guide

## Common Issues and Solutions

### Issue 1: Logs Not Appearing in Console

**Symptoms:**
- No logs visible when running the application
- Console output is empty

**Possible Causes:**
1. Serilog not configured correctly
2. Log level set too high
3. Application not running in debug mode

**Solutions:**

1. **Verify Serilog Configuration**
   ```csharp
   // In Program.cs, check:
   Log.Logger = new LoggerConfiguration()
       .MinimumLevel.Debug()  // Should be Debug or lower
       .WriteTo.Console()     // Should be present
       .CreateLogger();
   
   builder.Host.UseSerilog();  // Should be present
   ```

2. **Check Log Level**
   ```csharp
   // In Program.cs, change to:
   .MinimumLevel.Debug()  // Not Information or higher
   ```

3. **Verify Application is Running**
   - Check that the application started without errors
   - Look for startup logs

4. **Check for Exceptions**
   - Look for any startup exceptions
   - Check that all NuGet packages are installed

**Verification:**
```bash
# Run the application
dotnet run

# Make a test request
curl -X GET http://localhost:5000/api/users

# Check console output for logs
```

---

### Issue 2: Logs Not Appearing in Files

**Symptoms:**
- Console logs appear but no file logs
- `logs/` directory doesn't exist
- File logs are empty

**Possible Causes:**
1. `logs/` directory doesn't exist
2. No write permissions
3. File sink not configured
4. Incorrect file path

**Solutions:**

1. **Create logs Directory**
   ```bash
   # Windows
   mkdir logs
   
   # Linux/Mac
   mkdir -p logs
   ```

2. **Check Permissions**
   ```bash
   # Windows - should be writable
   dir logs
   
   # Linux/Mac - check permissions
   ls -la logs
   chmod 755 logs
   ```

3. **Verify File Sink Configuration**
   ```csharp
   // In Program.cs, check:
   .WriteTo.File(
       path: "logs/app-.txt",
       rollingInterval: RollingInterval.Day,
       outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
       retainedFileCountLimit: 30)
   ```

4. **Check File Path**
   - Ensure path is relative to application root
   - Or use absolute path: `Path.Combine(AppContext.BaseDirectory, "logs", "app-.txt")`

**Verification:**
```bash
# Check if logs directory exists
ls -la logs/

# Check if log files are being created
ls -la logs/app-*.txt

# Check file size
du -h logs/app-*.txt

# View latest logs
tail -f logs/app-*.txt
```

---

### Issue 3: Too Many Logs / Performance Issues

**Symptoms:**
- Application running slowly
- Disk space filling up quickly
- Log files are very large

**Possible Causes:**
1. Log level set too low (Debug)
2. Logging too much data
3. Large request/response bodies
4. Log file retention too high

**Solutions:**

1. **Increase Log Level**
   ```csharp
   // In Program.cs, change from:
   .MinimumLevel.Debug()
   
   // To:
   .MinimumLevel.Information()  // Or Warning
   ```

2. **Reduce Request/Response Body Logging**
   ```csharp
   // In RequestResponseLoggingMiddleware.cs, modify:
   if (!string.IsNullOrEmpty(body) && body.Length < 1000)  // Reduce from 5000
   {
       logMessage.AppendLine($"Body: {body}");
   }
   ```

3. **Reduce Log Retention**
   ```csharp
   // In Program.cs, change from:
   retainedFileCountLimit: 30
   
   // To:
   retainedFileCountLimit: 7  // Keep only 7 days
   ```

4. **Disable Request/Response Logging**
   ```csharp
   // In Program.cs, comment out:
   // app.UseMiddleware<RequestResponseLoggingMiddleware>();
   ```

**Verification:**
```bash
# Check disk usage
du -sh logs/

# Check log file sizes
ls -lh logs/app-*.txt

# Count log entries
wc -l logs/app-*.txt

# Check for large bodies
grep -c "Body:" logs/app-*.txt
```

---

### Issue 4: Sensitive Data in Logs

**Symptoms:**
- Passwords appearing in logs
- API keys visible in logs
- Personal information logged

**Possible Causes:**
1. Logging request/response bodies
2. Not redacting sensitive headers
3. Logging sensitive parameters

**Solutions:**

1. **Verify Authorization Header Redaction**
   ```csharp
   // In RequestResponseLoggingMiddleware.cs, check:
   if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
   {
       logMessage.AppendLine($"  {header.Key}: [REDACTED]");
   }
   ```

2. **Don't Log Passwords**
   ```csharp
   // Bad:
   LoggingHelper.LogMethodEntry(_logger, nameof(CreateUser), 
       new Dictionary<string, object?> { { "password", password } });
   
   // Good:
   LoggingHelper.LogMethodEntry(_logger, nameof(CreateUser), 
       new Dictionary<string, object?> { { "email", email } });
   ```

3. **Disable Body Logging for Sensitive Endpoints**
   ```csharp
   // In RequestResponseLoggingMiddleware.cs:
   if (request.Path.StartsWithSegments("/api/auth/login"))
   {
       // Don't log body for login endpoint
       body = "[REDACTED]";
   }
   ```

**Verification:**
```bash
# Search for passwords in logs
grep -i "password" logs/app-*.txt

# Search for API keys
grep -i "api.key\|apikey" logs/app-*.txt

# Search for tokens
grep -i "token" logs/app-*.txt
```

---

### Issue 5: Request ID Not Tracking Properly

**Symptoms:**
- Request ID changes between logs
- Can't trace entire request flow
- Request ID is empty or null

**Possible Causes:**
1. Middleware not generating ID
2. ID not being passed to context
3. ID not being logged

**Solutions:**

1. **Verify Middleware is Registered**
   ```csharp
   // In Program.cs, check:
   app.UseMiddleware<RequestResponseLoggingMiddleware>();
   ```

2. **Check ID Generation**
   ```csharp
   // In RequestResponseLoggingMiddleware.cs:
   var requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
   context.Items["RequestId"] = requestId;
   ```

3. **Verify ID is Logged**
   ```csharp
   // In RequestResponseLoggingMiddleware.cs:
   logMessage.AppendLine($"[{requestId}] ===== HTTP REQUEST =====");
   ```

**Verification:**
```bash
# Search for request IDs
grep "\[.*\] ===== HTTP REQUEST" logs/app-*.txt | head -5

# Trace specific request
grep "a1b2c3d4" logs/app-*.txt
```

---

### Issue 6: Database Operations Not Logged

**Symptoms:**
- No database operation logs
- Can't see SQL queries
- Database errors not logged

**Possible Causes:**
1. LoggingHelper not being used
2. Database operation logging commented out
3. Service not using LoggingHelper

**Solutions:**

1. **Verify LoggingHelper Usage**
   ```csharp
   // In service method:
   LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "Users", "WHERE Id = 123");
   ```

2. **Check Service Implementation**
   ```csharp
   // Ensure service is using LoggingHelper:
   using BusBookingAPI.Utilities;
   
   public class MyService
   {
       public async Task<MyDto> GetByIdAsync(int id)
       {
           LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "MyTable", $"WHERE Id = {id}");
           // ... rest of method
       }
   }
   ```

3. **Verify Log Level**
   ```csharp
   // Database operations are logged at Debug level
   // Ensure log level is Debug or lower:
   .MinimumLevel.Debug()
   ```

**Verification:**
```bash
# Search for database operations
grep "📊 Database operation" logs/app-*.txt

# Count database operations
grep -c "📊" logs/app-*.txt

# Find specific table operations
grep "📊.*Users" logs/app-*.txt
```

---

### Issue 7: Performance Metrics Not Showing

**Symptoms:**
- No performance logs
- Can't see execution times
- Slow operations not identified

**Possible Causes:**
1. Stopwatch not being used
2. LogPerformance not being called
3. Stopwatch not being stopped

**Solutions:**

1. **Verify Stopwatch Usage**
   ```csharp
   // In service method:
   var stopwatch = Stopwatch.StartNew();
   
   // ... method logic ...
   
   stopwatch.Stop();
   LoggingHelper.LogPerformance(_logger, "MethodName", stopwatch.ElapsedMilliseconds);
   ```

2. **Check LogPerformance Call**
   ```csharp
   // Ensure LogPerformance is called:
   LoggingHelper.LogPerformance(_logger, "GetUserByIdAsync", stopwatch.ElapsedMilliseconds);
   ```

3. **Verify Stopwatch is Stopped**
   ```csharp
   // Always stop before logging:
   stopwatch.Stop();
   LoggingHelper.LogPerformance(_logger, "MethodName", stopwatch.ElapsedMilliseconds);
   ```

**Verification:**
```bash
# Search for performance logs
grep "⏱️" logs/app-*.txt

# Find slow operations (>1 second)
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt

# Find operations taking >500ms
grep "⏱️.*took.*[5-9][0-9][0-9]ms\|⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt
```

---

### Issue 8: Exceptions Not Logged

**Symptoms:**
- Exceptions not appearing in logs
- Error logs are empty
- Can't see exception details

**Possible Causes:**
1. Try-catch not implemented
2. LogException not being called
3. Exception being swallowed

**Solutions:**

1. **Verify Try-Catch Block**
   ```csharp
   // Ensure try-catch is present:
   try
   {
       // ... method logic ...
   }
   catch (Exception ex)
   {
       LoggingHelper.LogException(_logger, ex, "OperationName", context);
       throw;
   }
   ```

2. **Check LogException Call**
   ```csharp
   // Ensure LogException is called:
   LoggingHelper.LogException(_logger, ex, "GetUserById", 
       new Dictionary<string, object?> { { "id", id } });
   ```

3. **Verify Exception is Re-thrown**
   ```csharp
   // Always re-throw after logging:
   catch (Exception ex)
   {
       LoggingHelper.LogException(_logger, ex, "OperationName");
       throw;  // Important!
   }
   ```

**Verification:**
```bash
# Search for exceptions
grep "\[ERR\]" logs/app-*.txt

# Find specific exception type
grep "NullReferenceException\|ArgumentException" logs/app-*.txt

# Find exceptions in specific operation
grep "❌.*CreateUser" logs/app-*.txt
```

---

### Issue 9: Logs Not Rotating Daily

**Symptoms:**
- All logs in single file
- Old logs not being cleaned up
- Disk space not being freed

**Possible Causes:**
1. Rolling interval not set
2. Retention limit not set
3. File path not using rolling placeholder

**Solutions:**

1. **Verify Rolling Interval**
   ```csharp
   // In Program.cs, check:
   .WriteTo.File(
       path: "logs/app-.txt",
       rollingInterval: RollingInterval.Day,  // Should be present
       retainedFileCountLimit: 30)
   ```

2. **Check File Path Format**
   ```csharp
   // Must use dash before extension:
   path: "logs/app-.txt"  // Correct
   
   // Not:
   path: "logs/app.txt"   // Wrong - won't rotate
   ```

3. **Verify Retention Limit**
   ```csharp
   // Ensure retention is set:
   retainedFileCountLimit: 30  // Keep 30 days
   ```

**Verification:**
```bash
# Check log files
ls -la logs/app-*.txt

# Check file dates
ls -lh logs/app-*.txt | awk '{print $6, $7, $8, $9}'

# Count log files
ls logs/app-*.txt | wc -l
```

---

### Issue 10: Application Won't Start

**Symptoms:**
- Application crashes on startup
- Startup errors related to logging
- NuGet packages not found

**Possible Causes:**
1. Serilog NuGet packages not installed
2. Namespace imports missing
3. Configuration syntax error

**Solutions:**

1. **Install NuGet Packages**
   ```bash
   dotnet restore
   ```

2. **Verify Packages Installed**
   ```bash
   # Check csproj file
   cat BusBookingAPI.csproj | grep -i serilog
   ```

3. **Check Namespace Imports**
   ```csharp
   // In Program.cs, ensure:
   using Serilog;
   using BusBookingAPI.Middleware;
   ```

4. **Verify Configuration Syntax**
   ```csharp
   // Check for syntax errors in Program.cs
   // Ensure all parentheses and semicolons are correct
   ```

**Verification:**
```bash
# Try to build
dotnet build

# Check for errors
dotnet build 2>&1 | grep -i error

# Run with verbose output
dotnet run --verbose
```

---

## Debugging Workflow

### Step 1: Identify the Problem
- What's not working?
- When did it start?
- What changed?

### Step 2: Check Logs
```bash
# View recent logs
tail -f logs/app-*.txt

# Search for errors
grep "\[ERR\]" logs/app-*.txt

# Search for warnings
grep "\[WRN\]" logs/app-*.txt
```

### Step 3: Find Request ID
```bash
# If you know the request time:
grep "2024-04-23 14:30" logs/app-*.txt | head -1

# Extract request ID from first log
REQUEST_ID=$(grep "2024-04-23 14:30" logs/app-*.txt | head -1 | grep -oP '\[\K[a-z0-9]+(?=\])')

# Trace entire request
grep "$REQUEST_ID" logs/app-*.txt
```

### Step 4: Analyze Flow
- Check request details
- Check service method logs
- Check database operations
- Check validation/business logic
- Check response

### Step 5: Identify Root Cause
- Is it a validation error?
- Is it a database error?
- Is it a business logic error?
- Is it a performance issue?

### Step 6: Fix and Test
- Make the fix
- Test with sample request
- Verify logs show success

---

## Quick Reference

| Issue | Solution |
|-------|----------|
| No console logs | Check log level, verify Serilog config |
| No file logs | Create logs/ directory, check permissions |
| Too many logs | Increase log level, reduce body logging |
| Sensitive data logged | Verify redaction, don't log passwords |
| Request ID not tracking | Verify middleware registered, check ID generation |
| Database ops not logged | Use LoggingHelper, check log level |
| Performance metrics missing | Use Stopwatch, call LogPerformance |
| Exceptions not logged | Implement try-catch, call LogException |
| Logs not rotating | Check rolling interval, verify file path |
| App won't start | Install packages, check imports, verify syntax |

---

## Getting Help

1. **Check Documentation**
   - LOGGING_GUIDE.md
   - LOGGING_QUICK_REFERENCE.md
   - LOGGING_ARCHITECTURE.md

2. **Review Examples**
   - UserService.cs (fully implemented)
   - RequestResponseLoggingMiddleware.cs

3. **Search Logs**
   - Use grep to find relevant logs
   - Search for request ID
   - Search for error messages

4. **Test Locally**
   - Run application
   - Make test requests
   - Check logs in real-time

---

**Last Updated**: April 23, 2026
**Status**: ✅ Ready to use
