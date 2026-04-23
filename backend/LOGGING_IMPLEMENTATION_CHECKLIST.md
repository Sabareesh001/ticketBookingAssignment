# Logging Implementation Checklist

Use this checklist when adding detailed logging to other services.

## Setup

- [ ] Add `using BusBookingAPI.Utilities;` at the top
- [ ] Add `using System.Diagnostics;` for stopwatch
- [ ] Ensure `ILogger<ServiceName>` is injected in constructor
- [ ] Store logger as `private readonly ILogger<ServiceName> _logger;`

## For Each Public Method

### Entry & Exit
- [ ] Add `LoggingHelper.LogMethodEntry()` at method start with parameters
- [ ] Add `LoggingHelper.LogMethodExit()` before each return statement
- [ ] Create `Stopwatch` at method start: `var stopwatch = Stopwatch.StartNew();`
- [ ] Call `stopwatch.Stop()` before logging exit

### Database Operations
- [ ] Log before each database query: `LoggingHelper.LogDatabaseOperation()`
- [ ] Include operation type: SELECT, INSERT, UPDATE, DELETE
- [ ] Include table name and relevant WHERE clause or details

### Validation
- [ ] Log each validation error: `LoggingHelper.LogValidationError()`
- [ ] Include field name and error message
- [ ] Throw exception after logging

### Business Logic
- [ ] Log business logic errors: `LoggingHelper.LogBusinessLogicError()`
- [ ] Include operation name and reason for failure
- [ ] Throw exception after logging

### Success
- [ ] Log successful operations: `LoggingHelper.LogSuccess()`
- [ ] Include relevant identifiers (ID, email, etc.)
- [ ] Log before returning

### Performance
- [ ] Log performance metrics: `LoggingHelper.LogPerformance()`
- [ ] Include method name and elapsed milliseconds
- [ ] Log after stopwatch.Stop()

### Exception Handling
- [ ] Wrap method in try-catch
- [ ] Log exceptions: `LoggingHelper.LogException()`
- [ ] Include operation name and context
- [ ] Re-throw exception

## Example Template

```csharp
public async Task<ResultDto> MyMethodAsync(int id, string name)
{
    var stopwatch = Stopwatch.StartNew();
    LoggingHelper.LogMethodEntry(_logger, nameof(MyMethodAsync), 
        new Dictionary<string, object?> { { "id", id }, { "name", name } });

    try
    {
        // Validation
        if (string.IsNullOrWhiteSpace(name))
        {
            LoggingHelper.LogValidationError(_logger, "name", "Name is required");
            throw new ArgumentException("Name is required");
        }

        // Database operation
        LoggingHelper.LogDatabaseOperation(_logger, "SELECT", "MyTable", $"WHERE Id = {id}");
        var item = await _context.MyTable.FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            stopwatch.Stop();
            LoggingHelper.LogBusinessLogicError(_logger, "MyMethod", $"Item {id} not found");
            throw new KeyNotFoundException($"Item {id} not found");
        }

        // Update operation
        item.Name = name;
        LoggingHelper.LogDatabaseOperation(_logger, "UPDATE", "MyTable", $"Item ID: {id}");
        _context.MyTable.Update(item);
        await _context.SaveChangesAsync();

        stopwatch.Stop();
        LoggingHelper.LogSuccess(_logger, "MyMethod", $"Item ID: {item.Id}");
        LoggingHelper.LogPerformance(_logger, "MyMethodAsync", stopwatch.ElapsedMilliseconds);
        LoggingHelper.LogMethodExit(_logger, nameof(MyMethodAsync), item.Id);

        return MapToDto(item);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        LoggingHelper.LogException(_logger, ex, "MyMethod", 
            new Dictionary<string, object?> { { "id", id }, { "name", name } });
        throw;
    }
}
```

## Services to Update

- [ ] AuthService
- [ ] BookingService
- [ ] BusService
- [ ] CountryService
- [ ] DistrictService
- [ ] LocationService
- [ ] OperatorService
- [ ] RouteService
- [ ] StateService

## Testing

After implementing logging:

1. [ ] Run the application
2. [ ] Make a test request to the endpoint
3. [ ] Check console output for logs
4. [ ] Check `logs/app-YYYY-MM-DD.txt` file
5. [ ] Verify all log levels appear (Debug, Info, Warning, Error)
6. [ ] Verify request/response logging middleware works
7. [ ] Verify performance metrics are logged
8. [ ] Verify exceptions are logged with context

## Verification Commands

```bash
# Check if logs directory exists
ls -la logs/

# View latest log file
tail -f logs/app-*.txt

# Search for specific operation
grep "MyOperation" logs/app-*.txt

# Find errors
grep "\[ERR\]" logs/app-*.txt

# Find slow operations
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt
```

## Common Mistakes to Avoid

❌ **Don't:**
- Forget to stop the stopwatch before logging performance
- Log sensitive data (passwords, tokens)
- Use string interpolation in log messages (use structured logging)
- Forget try-catch blocks
- Log the same information twice
- Use generic error messages

✅ **Do:**
- Always include context in exception logs
- Use consistent method names in logs
- Include relevant IDs and identifiers
- Stop stopwatch before logging performance
- Use LoggingHelper methods for consistency
- Include operation names in all logs

## Performance Considerations

- Logging is asynchronous - minimal performance impact
- Large response bodies are truncated (>5000 chars)
- Sensitive headers are redacted
- File rotation happens daily
- Old logs are cleaned up after 30 days
- Can adjust log level if needed (change MinimumLevel in Program.cs)

## Debugging Workflow

1. **Identify the issue** - What went wrong?
2. **Find the request ID** - Search logs for the request
3. **Trace the flow** - Follow method entry/exit logs
4. **Check database operations** - Look for SELECT/INSERT/UPDATE/DELETE logs
5. **Review validation** - Check for validation error logs
6. **Check performance** - Look for slow operations
7. **Review exceptions** - Check error logs with full context

## Questions?

Refer to:
- `LOGGING_GUIDE.md` - Comprehensive logging documentation
- `LOGGING_QUICK_REFERENCE.md` - Quick reference for common patterns
- `UserService.cs` - Example of fully implemented logging
