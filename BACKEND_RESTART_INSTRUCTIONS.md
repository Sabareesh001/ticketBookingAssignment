# Backend API Restart Instructions

## Issue
Getting 404 error when accessing `http://localhost:5266/api/busavailability`

## Solution
The backend API needs to be restarted to register the BusAvailabilityController properly.

## Steps to Restart Backend

### Option 1: Using Terminal (Recommended)

1. **Stop the current backend process** (if running):
   - Press `Ctrl+C` in the terminal where the backend is running
   - Or close the terminal window

2. **Navigate to backend directory**:
   ```bash
   cd backend/BusBookingAPI
   ```

3. **Clean and rebuild**:
   ```bash
   dotnet clean
   dotnet build
   ```

4. **Run the backend**:
   ```bash
   dotnet run
   ```

5. **Verify the API is running**:
   - Open browser to: `http://localhost:5266/swagger`
   - You should see the Swagger UI with all endpoints including BusAvailability

### Option 2: Using Visual Studio / Rider

1. Stop the current debug session
2. Clean Solution (Build → Clean Solution)
3. Rebuild Solution (Build → Rebuild Solution)
4. Start Debugging (F5)

### Option 3: Using VS Code

1. Stop the current debug session (Shift+F5)
2. Open terminal in VS Code
3. Navigate to backend folder:
   ```bash
   cd backend/BusBookingAPI
   ```
4. Run:
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

## Verification Steps

After restarting, verify the endpoints are working:

### 1. Check Swagger UI
Open: `http://localhost:5266/swagger`

You should see these BusAvailability endpoints:
- GET `/api/BusAvailability` - Get all availabilities
- GET `/api/BusAvailability/{id}` - Get by ID
- GET `/api/BusAvailability/bus/{busId}` - Get by bus
- POST `/api/BusAvailability` - Create availability
- PUT `/api/BusAvailability/{id}` - Update availability
- DELETE `/api/BusAvailability/{id}` - Delete availability
- And many more...

### 2. Test the GET endpoint directly
Open in browser or use curl:
```bash
curl http://localhost:5266/api/busavailability
```

Expected response: `[]` (empty array) or list of availability records

### 3. Test from Frontend
1. Make sure frontend is running: `npm start` (in frontend/bus-booking directory)
2. Login as operator
3. Navigate to "Bus Availability" tab
4. Should load without 404 errors

## Common Issues & Solutions

### Issue: Port 5266 already in use
**Solution**: 
- Find and kill the process using port 5266
- Windows: `netstat -ano | findstr :5266` then `taskkill /PID <PID> /F`
- Linux/Mac: `lsof -ti:5266 | xargs kill -9`

### Issue: Database connection error
**Solution**:
- Verify PostgreSQL is running
- Check connection string in `appsettings.json` or environment variables
- Verify database exists: `busBooking`

### Issue: Still getting 404 after restart
**Solution**:
1. Check the terminal output for any startup errors
2. Verify the controller is being registered (look for "Now listening on: http://localhost:5266")
3. Check if CORS is properly configured
4. Try accessing: `http://localhost:5266/api/bus` (another endpoint) to verify API is running

### Issue: Swagger shows but endpoints don't work
**Solution**:
- Check if authentication is required
- Verify the operator is logged in on the frontend
- Check browser console for CORS errors

## Backend Configuration Checklist

✅ **Program.cs** - Service registered:
```csharp
builder.Services.AddScoped<IBusAvailabilityService, BusAvailabilityService>();
```

✅ **BusAvailabilityController.cs** - Controller exists with proper routing:
```csharp
[ApiController]
[Route("api/[controller]")]
public class BusAvailabilityController : ControllerBase
```

✅ **BusAvailabilityService.cs** - Service implements interface

✅ **BusAvailabilityDto.cs** - All DTOs defined

✅ **BusBookingDbContext.cs** - BusAvailabilities DbSet exists

## Testing the API

Once the backend is running, you can test with these curl commands:

### Get all availabilities
```bash
curl http://localhost:5266/api/busavailability
```

### Create availability (requires authentication)
```bash
curl -X POST http://localhost:5266/api/busavailability \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "busId": 1,
    "availableDate": "2026-04-30",
    "totalSeats": 45,
    "availableSeats": 45,
    "isActive": true
  }'
```

### Get availability by bus
```bash
curl http://localhost:5266/api/busavailability/bus/1
```

## Next Steps

After successfully restarting the backend:

1. ✅ Verify Swagger UI loads
2. ✅ Test GET endpoint returns data (or empty array)
3. ✅ Login to frontend as operator
4. ✅ Navigate to Bus Availability tab
5. ✅ Try creating a new availability record
6. ✅ Test filtering and search functionality

## Need More Help?

If issues persist:
1. Check backend terminal for error messages
2. Check browser console for frontend errors
3. Verify database has BusAvailabilities table
4. Check if other API endpoints work (Bus, Location, etc.)
5. Review the logs in `backend/BusBookingAPI/logs/`
