# Logging System Index

## Quick Navigation

### 📚 Documentation Files

| File | Purpose | Audience |
|------|---------|----------|
| **LOGGING_SETUP_SUMMARY.md** | Overview of what was implemented | Everyone |
| **LOGGING_GUIDE.md** | Comprehensive guide with examples | Developers |
| **LOGGING_QUICK_REFERENCE.md** | Quick reference for common patterns | Developers |
| **LOGGING_IMPLEMENTATION_CHECKLIST.md** | Checklist for adding logging to services | Developers |
| **LOGGING_ARCHITECTURE.md** | System architecture and data flow | Architects, Senior Devs |
| **LOGGING_INDEX.md** | This file - navigation guide | Everyone |

### 💻 Code Files

| File | Purpose |
|------|---------|
| `Middleware/RequestResponseLoggingMiddleware.cs` | HTTP request/response logging |
| `Utilities/LoggingHelper.cs` | Logging helper methods |
| `Services/UserService.cs` | Example of fully implemented logging |
| `Program.cs` | Serilog configuration |
| `BusBookingAPI.csproj` | Serilog NuGet packages |

## Getting Started

### For First-Time Users
1. Read **LOGGING_SETUP_SUMMARY.md** (5 min)
2. Check **LOGGING_QUICK_REFERENCE.md** (5 min)
3. Review `UserService.cs` for examples (10 min)
4. Run the application and check logs (5 min)

### For Developers Adding Logging
1. Read **LOGGING_QUICK_REFERENCE.md** (5 min)
2. Follow **LOGGING_IMPLEMENTATION_CHECKLIST.md** (15 min)
3. Copy pattern from `UserService.cs` (10 min)
4. Test with sample requests (10 min)

### For Debugging Issues
1. Check **LOGGING_GUIDE.md** - Debugging Tips section
2. Search logs using grep commands
3. Use request ID to trace entire flow
4. Review **LOGGING_ARCHITECTURE.md** for data flow

### For Understanding Architecture
1. Read **LOGGING_ARCHITECTURE.md** (15 min)
2. Review system diagrams
3. Understand data flow
4. Check integration points

## Common Tasks

### View Logs
```bash
# Real-time console output
# Appears when running the application

# View file logs
tail -f logs/app-*.txt

# Search for specific user
grep "user@example.com" logs/app-*.txt

# Find errors
grep "\[ERR\]" logs/app-*.txt

# Find slow operations
grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt
```

### Add Logging to New Service
1. Copy pattern from `UserService.cs`
2. Use `LoggingHelper` methods
3. Follow `LOGGING_IMPLEMENTATION_CHECKLIST.md`
4. Test with sample request

### Debug an Issue
1. Find request ID in logs
2. Grep for request ID to see entire flow
3. Check database operations
4. Review validation and business logic
5. Check performance metrics

### Configure Logging
Edit `Program.cs`:
- Change log level: `.MinimumLevel.Information()`
- Change log file location: `.WriteTo.File(path: "...")`
- Change retention: `retainedFileCountLimit: 30`

## Log Symbols Reference

| Symbol | Meaning | Example |
|--------|---------|---------|
| `→` | Method entry | `→ Entering method: CreateUserAsync` |
| `←` | Method exit | `← Exiting method: CreateUserAsync` |
| `📊` | Database operation | `📊 Database operation: SELECT on Users` |
| `⚠️` | Validation error | `⚠️ Validation error on field 'Email'` |
| `❌` | Business logic error or exception | `❌ Business logic error during CreateUser` |
| `✅` | Successful operation | `✅ CreateUser completed successfully` |
| `⏱️` | Performance metric | `⏱️ CreateUserAsync took 150ms` |

## File Locations

```
backend/
├── BusBookingAPI/
│   ├── Middleware/
│   │   └── RequestResponseLoggingMiddleware.cs
│   ├── Utilities/
│   │   └── LoggingHelper.cs
│   ├── Services/
│   │   ├── UserService.cs (example)
│   │   └── ... (other services to update)
│   ├── Program.cs (modified)
│   └── BusBookingAPI.csproj (modified)
│
├── LOGGING_SETUP_SUMMARY.md
├── LOGGING_GUIDE.md
├── LOGGING_QUICK_REFERENCE.md
├── LOGGING_IMPLEMENTATION_CHECKLIST.md
├── LOGGING_ARCHITECTURE.md
└── LOGGING_INDEX.md (this file)

logs/
├── app-2024-04-23.txt
├── app-2024-04-22.txt
└── ... (daily rotation, 30 day retention)
```

## Key Features

✅ **Request/Response Logging**
- Unique request ID for tracking
- HTTP method, path, headers, body
- Response status, duration, headers, body
- Sensitive data redaction

✅ **Service Method Logging**
- Method entry/exit with parameters
- Database operations (SELECT, INSERT, UPDATE, DELETE)
- Validation errors with field names
- Business logic errors with reasons
- Success operations with relevant data
- Performance metrics (execution time)
- Exception logging with context

✅ **Structured Logging**
- Consistent format across all logs
- Easy to search and filter
- Timestamps for all entries
- Log levels (Debug, Info, Warning, Error)

✅ **Performance Monitoring**
- Execution time tracking
- Slow operation detection
- Performance metrics logging

✅ **Error Tracking**
- Exception logging with full context
- Stack traces
- Operation context
- Parameter values

## Documentation Structure

### LOGGING_SETUP_SUMMARY.md
- What was implemented
- Files created/modified
- Features overview
- Quick start guide
- Configuration options

### LOGGING_GUIDE.md
- Comprehensive overview
- Request/response logging details
- Service method logging details
- Using LoggingHelper
- Debugging tips
- Adding logging to new services
- Performance considerations

### LOGGING_QUICK_REFERENCE.md
- Quick start code snippets
- Common patterns
- Viewing logs
- Log symbols
- Tips and best practices
- Performance impact

### LOGGING_IMPLEMENTATION_CHECKLIST.md
- Setup checklist
- Per-method checklist
- Example template
- Services to update
- Testing checklist
- Verification commands
- Common mistakes
- Debugging workflow

### LOGGING_ARCHITECTURE.md
- System overview diagram
- Logging flow diagram
- Component descriptions
- Data flow examples
- Log file structure
- Request ID tracking
- Performance monitoring
- Error handling flow
- Integration points
- Debugging workflow

## Next Steps

1. **Install packages**: `dotnet restore`
2. **Test application**: Make a request and check logs
3. **Update services**: Add logging to other services
4. **Monitor performance**: Use logs to identify slow operations
5. **Debug issues**: Use logs to trace problems

## Support Resources

- **Examples**: See `UserService.cs` for fully implemented logging
- **Patterns**: Check `LOGGING_QUICK_REFERENCE.md` for common patterns
- **Checklist**: Use `LOGGING_IMPLEMENTATION_CHECKLIST.md` when adding logging
- **Architecture**: Review `LOGGING_ARCHITECTURE.md` for system design
- **Debugging**: See `LOGGING_GUIDE.md` - Debugging Tips section

## FAQ

**Q: Where are the logs stored?**
A: In `logs/app-YYYY-MM-DD.txt` files (daily rotation, 30 day retention)

**Q: How do I search logs?**
A: Use grep: `grep "search_term" logs/app-*.txt`

**Q: How do I trace a specific request?**
A: Search for the request ID: `grep "a1b2c3d4" logs/app-*.txt`

**Q: How do I find slow operations?**
A: Search for performance warnings: `grep "⏱️.*took.*[1-9][0-9][0-9][0-9]ms" logs/app-*.txt`

**Q: How do I add logging to a new service?**
A: Follow the pattern in `UserService.cs` and use the `LOGGING_IMPLEMENTATION_CHECKLIST.md`

**Q: Can I disable logging?**
A: Yes, change the log level in `Program.cs` or comment out the middleware

**Q: What's the performance impact?**
A: Minimal - logging is asynchronous and optimized

**Q: Are sensitive data logged?**
A: No - Authorization headers are redacted, passwords are hashed

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2024-04-23 | Initial implementation |

---

**Last Updated**: April 23, 2026
**Status**: ✅ Ready to use
**Maintainer**: Development Team
