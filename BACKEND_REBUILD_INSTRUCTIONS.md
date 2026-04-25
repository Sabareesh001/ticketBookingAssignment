# Backend Rebuild Instructions

## Quick Fix for 500 Error

The Bus Availability Manager feature requires the backend to be recompiled. Follow these steps:

### Option 1: Using Command Line (Recommended)

```bash
# Navigate to backend directory
cd backend/BusBookingAPI

# Clean previous builds
dotnet clean

# Rebuild the project
dotnet build

# Run the backend
dotnet run
```

Wait for the output to show:
```
Now listening on: http://localhost:5266
Application started. Press Ctrl+C to shut down.
```

### Option 2: Using Visual Studio

1. Open the solution in Visual Studio
2. Right-click on `BusBookingAPI` project
3. Select "Clean"
4. Right-click again and select "Rebuild"
5. Press F5 or click "Start Debugging"

### Option 3: Using Visual Studio Code

1. Open the integrated terminal
2. Run: `dotnet clean && dotnet build && dotnet run`

## Verification

After the backend restarts:

1. Check that the backend is running on `http://localhost:5266`
2. Open the frontend application
3. Log in as an operator
4. Navigate to the "Bus Availability" tab
5. The tab should load without errors

## If Still Getting 500 Error

### Check 1: Verify Backend is Running
```bash
# In a new terminal, check if port 5266 is listening
netstat -ano | findstr :5266  # Windows
lsof -i :5266                  # Mac/Linux
```

### Check 2: Review Backend Logs
Look for error messages in the backend console output. Common errors:
- `DbContext not initialized` - Database connection issue
- `Method not found` - Code not recompiled
- `Unauthorized` - JWT token issue

### Check 3: Verify Database Connection
Check `appsettings.json` for correct database connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=busBooking;Username=postgres;Password=..."
  }
}
```

### Check 4: Clear Browser Cache
1. Open Developer Tools (F12)
2. Go to Application tab
3. Clear all storage
4. Refresh the page

## Files That Were Modified

The following backend files were updated and need recompilation:

1. `backend/BusBookingAPI/Controllers/OperatorDashboardController.cs`
   - Added 5 new availability endpoints

2. `backend/BusBookingAPI/Services/OperatorDashboardService.cs`
   - Added 5 new availability methods

3. `backend/BusBookingAPI/Services/IOperatorDashboardService.cs`
   - Added 5 new method signatures

4. `backend/BusBookingAPI/DTOs/BusAvailabilityDto.cs`
   - Added CreateBusAvailabilityDto
   - Added UpdateBusAvailabilityDto

## Expected Behavior After Fix

Once the backend is rebuilt and running:

✅ GET `/api/operator-dashboard/availability` - Returns list of availability records
✅ GET `/api/operator-dashboard/availability/{id}` - Returns single record
✅ POST `/api/operator-dashboard/availability` - Creates new record
✅ PUT `/api/operator-dashboard/availability/{id}` - Updates record
✅ DELETE `/api/operator-dashboard/availability/{id}` - Deletes record

All endpoints require:
- Valid JWT token in Authorization header
- Operator must own the bus

## Troubleshooting Build Issues

### Issue: "The type or namespace name 'CreateBusAvailabilityDto' could not be found"
**Solution:** Make sure you saved the changes to `BusAvailabilityDto.cs` and ran `dotnet clean`

### Issue: "Cannot find method 'GetOperatorBusAvailabilityAsync'"
**Solution:** Verify the method was added to both the interface and the service class

### Issue: "The project file could not be loaded"
**Solution:** Check for syntax errors in `.csproj` file or run `dotnet restore`

### Issue: Build hangs or takes very long
**Solution:** 
- Kill the process and try again
- Check disk space
- Try `dotnet clean --force`

## Performance Notes

- First build after clean may take 30-60 seconds
- Subsequent builds are faster (10-20 seconds)
- If using `dotnet watch run`, changes are auto-compiled

## Next Steps

After successful rebuild:

1. Test the Bus Availability Manager in the frontend
2. Create test availability records
3. Verify all CRUD operations work
4. Test with different operators to verify authorization
5. Check pagination and filtering

## Support

If you continue to experience issues:

1. Check the backend console for specific error messages
2. Review the logs in `backend/BusBookingAPI/logs/`
3. Verify all files were properly saved
4. Ensure you're using .NET 10.0 or compatible version
5. Check that PostgreSQL database is running and accessible
