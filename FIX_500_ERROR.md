# Fixing the 500 Error on Bus Availability Endpoint

## Problem
The `/api/operator-dashboard/availability` endpoint is returning a 500 Internal Server Error.

## Root Cause
The backend code has been updated with new availability endpoints, but the backend application needs to be recompiled and restarted to load the changes.

## Solution

### Step 1: Stop the Backend Server
If the backend is currently running, stop it:
- Press `Ctrl+C` in the terminal where the backend is running
- Or kill the process running on port 5266

### Step 2: Rebuild the Backend
Navigate to the backend directory and rebuild:

```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
```

### Step 3: Restart the Backend
Start the backend server again:

```bash
dotnet run
```

Or if using the development server:

```bash
dotnet watch run
```

### Step 4: Verify the Fix
1. Wait for the backend to fully start (you should see "Application started" in the logs)
2. Go back to the frontend
3. Click on the "Bus Availability" tab
4. The availability records should now load successfully

## What Was Changed

The following files were modified to add Bus Availability Manager functionality:

### Backend Files
1. **OperatorDashboardController.cs**
   - Added 5 new endpoints for availability CRUD operations
   - All endpoints require `[Authorize]` attribute

2. **OperatorDashboardService.cs**
   - Added 5 new methods for availability management
   - Includes proper authorization checks and validation

3. **IOperatorDashboardService.cs**
   - Added 5 new method signatures to the interface

4. **BusAvailabilityDto.cs**
   - Added `CreateBusAvailabilityDto` class
   - Added `UpdateBusAvailabilityDto` class

### Frontend Files
1. **operator-dashboard.component.ts**
   - Added availability tab and state management
   - Added availability form with validation
   - Added CRUD methods for availability

2. **operator-dashboard.component.html**
   - Added "Bus Availability" tab button
   - Added availability table section
   - Added availability modal for create/edit

3. **operator-dashboard.component.css**
   - Added styling for availability table

4. **operator-dashboard.service.ts**
   - Added availability service methods
   - Added availability DTOs

## Troubleshooting

### If you still get 500 error after restart:

1. **Check the backend logs** for specific error messages
2. **Verify the database connection** is working
3. **Check that all DTOs are properly defined** in BusAvailabilityDto.cs
4. **Ensure the service is registered** in the dependency injection container

### Common Issues:

**Issue: "Method not found" error**
- Solution: Make sure you did a full `dotnet clean` and `dotnet build`

**Issue: "DbContext not initialized" error**
- Solution: Verify the database connection string in appsettings.json

**Issue: "Unauthorized" error (401)**
- Solution: Make sure you're logged in as an operator and have a valid JWT token

**Issue: "Bus not found" error (400)**
- Solution: Create at least one bus before creating availability records

## Verification Checklist

After restarting the backend, verify:

- [ ] Backend is running on http://localhost:5266
- [ ] Frontend can access the operator dashboard
- [ ] "Bus Availability" tab is visible
- [ ] Clicking the tab loads availability records (or shows empty state)
- [ ] Can create a new availability record
- [ ] Can edit an existing record
- [ ] Can delete a record
- [ ] Pagination works correctly

## Next Steps

Once the 500 error is fixed:

1. Create some test availability records
2. Verify all CRUD operations work
3. Test with different buses and dates
4. Verify authorization (operators can only see their own data)
5. Test error scenarios (duplicate dates, invalid data, etc.)

## Additional Notes

- The availability records are tied to buses
- Each bus can have multiple availability records for different dates
- Operators can only manage availability for their own buses
- The system prevents duplicate availability records for the same bus on the same date
- All timing fields (pickup time, drop time, duration) are optional
