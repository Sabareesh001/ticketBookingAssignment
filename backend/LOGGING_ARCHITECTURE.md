# Logging Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        HTTP Request                              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│         RequestResponseLoggingMiddleware                         │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ • Generate unique request ID                            │   │
│  │ • Log incoming request (method, path, headers, body)    │   │
│  │ • Capture response stream                               │   │
│  │ • Measure request duration                              │   │
│  │ • Log outgoing response (status, headers, body)         │   │
│  └─────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Controller Layer                              │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ UserController, BookingController, etc.                 │   │
│  │ • Receive request                                       │   │
│  │ • Call service methods                                  │   │
│  │ • Return response                                       │   │
│  └─────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Service Layer                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ UserService, BookingService, etc.                       │   │
│  │ ┌──────────────────────────────────────────────────┐   │   │
│  │ │ LoggingHelper Methods:                           │   │   │
│  │ │ • LogMethodEntry()                               │   │   │
│  │ │ • LogDatabaseOperation()                         │   │   │
│  │ │ • LogValidationError()                           │   │   │
│  │ │ • LogBusinessLogicError()                        │   │   │
│  │ │ • LogSuccess()                                   │   │   │
│  │ │ • LogPerformance()                               │   │   │
│  │ │ • LogMethodExit()                                │   │   │
│  │ │ • LogException()                                 │   │   │
│  │ └──────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Data Layer                                    │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ BusBookingDbContext                                     │   │
│  │ • Execute database queries                              │   │
│  │ • Return data                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Logging Flow

```
Request Arrives
    │
    ▼
RequestResponseLoggingMiddleware
    │
    ├─→ Generate Request ID (8 chars)
    │
    ├─→ Log Request Details
    │   ├─ Method & Path
    │   ├─ Headers (Authorization redacted)
    │   ├─ Body (if POST/PUT/PATCH)
    │   ├─ Remote IP
    │   └─ User
    │
    ├─→ Call Next Middleware/Controller
    │   │
    │   ▼
    │   Controller
    │   │
    │   ├─→ Call Service Method
    │   │   │
    │   │   ▼
    │   │   Service Method
    │   │   │
    │   │   ├─→ LogMethodEntry (with parameters)
    │   │   │
    │   │   ├─→ Validate Input
    │   │   │   └─→ LogValidationError (if invalid)
    │   │   │
    │   │   ├─→ LogDatabaseOperation (SELECT)
    │   │   ├─→ Query Database
    │   │   │
    │   │   ├─→ Check Business Logic
    │   │   │   └─→ LogBusinessLogicError (if invalid)
    │   │   │
    │   │   ├─→ LogDatabaseOperation (INSERT/UPDATE/DELETE)
    │   │   ├─→ Modify Database
    │   │   │
    │   │   ├─→ LogSuccess (operation completed)
    │   │   ├─→ LogPerformance (execution time)
    │   │   ├─→ LogMethodExit (with result)
    │   │   │
    │   │   └─→ Return Result
    │   │
    │   └─→ Return Response
    │
    ├─→ Log Response Details
    │   ├─ Status Code
    │   ├─ Duration
    │   ├─ Headers
    │   └─ Body (if not too large)
    │
    └─→ Send Response to Client
```

## Logging Components

### 1. RequestResponseLoggingMiddleware
```
┌──────────────────────────────────────────┐
│ RequestResponseLoggingMiddleware         │
├──────────────────────────────────────────┤
│ • Intercepts all HTTP requests           │
│ • Generates unique request ID            │
│ • Logs request details                   │
│ • Captures response stream               │
│ • Measures duration                      │
│ • Logs response details                  │
│ • Handles exceptions                     │
└──────────────────────────────────────────┘
```

### 2. LoggingHelper
```
┌──────────────────────────────────────────┐
│ LoggingHelper (Static Methods)           │
├──────────────────────────────────────────┤
│ • LogMethodEntry()                       │
│ • LogMethodExit()                        │
│ • LogDatabaseOperation()                 │
│ • LogValidationError()                   │
│ • LogBusinessLogicError()                │
│ • LogSuccess()                           │
│ • LogPerformance()                       │
│ • LogException()                         │
└──────────────────────────────────────────┘
```

### 3. Serilog Configuration
```
┌──────────────────────────────────────────┐
│ Serilog Logger                           │
├──────────────────────────────────────────┤
│ Sinks:                                   │
│ • Console (real-time output)             │
│ • File (daily rotation)                  │
│                                          │
│ Enrichers:                               │
│ • Timestamp                              │
│ • Log Level                              │
│ • Application Name                       │
│ • Log Context                            │
└──────────────────────────────────────────┘
```

## Data Flow Example: Create User

```
POST /api/users
{
  "email": "user@example.com",
  "fullName": "John Doe",
  "password": "password123"
}
    │
    ▼
[a1b2c3d4] ===== HTTP REQUEST =====
Method: POST
Path: /api/users
Body: {...}
    │
    ▼
UserController.CreateUserAsync()
    │
    ▼
UserService.CreateUserAsync()
    │
    ├─→ → Entering method: CreateUserAsync
    │
    ├─→ ⚠️ Validate FullName
    ├─→ ⚠️ Validate Email
    ├─→ ⚠️ Validate Password
    │
    ├─→ 📊 Database operation: SELECT (check email exists)
    │
    ├─→ 📊 Database operation: INSERT (create user)
    │
    ├─→ ✅ CreateUser completed successfully
    ├─→ ⏱️ CreateUserAsync took 150ms
    ├─→ ← Exiting method: CreateUserAsync
    │
    ▼
[a1b2c3d4] ===== HTTP RESPONSE =====
Status Code: 201
Duration: 333ms
Body: {"id": 1, "email": "user@example.com", ...}
```

## Log File Structure

```
logs/
├── app-2024-04-23.txt
├── app-2024-04-22.txt
├── app-2024-04-21.txt
└── ... (up to 30 days)

Each file contains:
[2024-04-23 14:30:45.123 +00:00] [INF] [RequestId] Message
[2024-04-23 14:30:45.200 +00:00] [DBG] → Entering method
[2024-04-23 14:30:45.210 +00:00] [DBG] 📊 Database operation
...
```

## Request ID Tracking

```
Request arrives
    │
    ▼
Generate ID: a1b2c3d4
    │
    ├─→ [a1b2c3d4] ===== HTTP REQUEST =====
    │
    ├─→ [a1b2c3d4] Service method logs (inherited from context)
    │
    ├─→ [a1b2c3d4] ===== HTTP RESPONSE =====
    │
    └─→ All logs for this request can be found by searching for "a1b2c3d4"
```

## Performance Monitoring

```
Service Method Execution
    │
    ├─→ Start: Stopwatch.StartNew()
    │
    ├─→ Execute business logic
    │
    ├─→ Stop: stopwatch.Stop()
    │
    ├─→ Log: ⏱️ MethodName took XXXms
    │
    └─→ Analyze:
        • < 100ms: Debug level
        • 100-500ms: Information level
        • 500-1000ms: Warning level
        • > 1000ms: Warning level (slow operation)
```

## Error Handling Flow

```
Exception Occurs
    │
    ▼
Catch Block
    │
    ├─→ Stop Stopwatch
    │
    ├─→ LogException()
    │   ├─ Exception details
    │   ├─ Operation name
    │   ├─ Context (parameters)
    │   └─ Stack trace
    │
    ├─→ Re-throw Exception
    │
    ▼
Middleware Catches
    │
    ├─→ Log unhandled exception
    │
    └─→ Return error response
```

## Log Level Distribution

```
Debug (→, ←, 📊, ⏱️)
    ↓
Information (✅, HTTP requests/responses)
    ↓
Warning (⚠️, ❌, slow operations)
    ↓
Error (❌ exceptions)
```

## Integration Points

```
Program.cs
    │
    ├─→ Configure Serilog
    │   ├─ Console sink
    │   ├─ File sink
    │   └─ Enrichers
    │
    ├─→ Add RequestResponseLoggingMiddleware
    │
    └─→ Register Services with ILogger injection

Services
    │
    ├─→ Inject ILogger<ServiceName>
    │
    └─→ Use LoggingHelper methods

Controllers
    │
    └─→ Automatically logged by middleware
```

## Debugging Workflow

```
Issue Detected
    │
    ▼
Find Request ID in logs
    │
    ▼
Grep for Request ID
    │
    ├─→ Find HTTP REQUEST log
    ├─→ Find Service method logs
    ├─→ Find Database operations
    ├─→ Find Validation errors
    ├─→ Find Business logic errors
    ├─→ Find Performance metrics
    └─→ Find HTTP RESPONSE log
    │
    ▼
Analyze Flow
    │
    ├─→ Check request parameters
    ├─→ Check database queries
    ├─→ Check validation logic
    ├─→ Check business logic
    ├─→ Check performance
    └─→ Check response
    │
    ▼
Identify Root Cause
    │
    └─→ Fix Issue
```

---

This architecture ensures comprehensive logging at every layer of the application, making debugging and monitoring straightforward and efficient.
