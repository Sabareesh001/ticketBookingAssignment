# Troubleshooting: Blank Operator Dashboard Page

## Issues Found & Fixed

### 1. ✅ Backend Not Running
**Problem**: Backend API was not running, so frontend couldn't fetch data
**Solution**: Started backend with `dotnet run` on port 5266
**Status**: Backend is now running and responding to requests

### 2. ✅ Frontend Dev Server Port Issue
**Problem**: Frontend dev server was trying to use port 4200 which was already in use
**Solution**: Frontend dev server automatically switched to port 56684
**Status**: Frontend is now running on `http://localhost:56684`

### 3. ✅ OperatorAuthGuard Redirect Bug
**Problem**: The `OperatorAuthGuard` was redirecting unauthenticated operators to `/login` (user login) instead of `/operator-login`
**Solution**: Fixed the guard to redirect to `/operator-login`
**File**: `frontend/bus-booking/src/app/guards/operator-auth.guard.ts`
**Status**: Fixed

## What You Need to Do

### Step 1: Access the Correct URL
Navigate to: **`http://localhost:56684/operator-dashboard`**

(Not `http://localhost:4200/operator-dashboard` - that port is in use)

### Step 2: Login as Operator
If you're not logged in, you'll be redirected to the operator login page:
- Go to `http://localhost:56684/operator-login`
- Enter your operator credentials
- Click "Login"

### Step 3: Verify the Dashboard Loads
Once logged in, you should see:
- Header with "Bus Operator Dashboard" title
- Tab navigation (Locations, Buses, Bus Availability)
- The Locations tab content with a table of your locations
- "+ Add Location" button

## Running Services

Both services are now running in the background:

**Backend**: `http://localhost:5266`
- Running: `dotnet run` in `backend/BusBookingAPI`
- Status: ✅ Running and responding to requests

**Frontend**: `http://localhost:56684`
- Running: `npm run start` in `frontend/bus-booking`
- Status: ✅ Running and watching for changes

## If You Still See a Blank Page

1. **Check browser console** (F12 → Console tab)
   - Look for any error messages
   - Check Network tab to see if API calls are failing

2. **Verify you're logged in**
   - Check if you have an `operator_auth_token` in localStorage
   - If not, login at `/operator-login`

3. **Check backend logs**
   - Look for any 500 errors or exceptions
   - Verify database connection is working

4. **Clear browser cache**
   - Hard refresh: Ctrl+Shift+R (or Cmd+Shift+R on Mac)
   - Clear localStorage if needed

## Next Steps

1. Navigate to `http://localhost:56684/operator-login`
2. Login with your operator credentials
3. You'll be redirected to the operator dashboard
4. Test the Bus Availability Manager feature:
   - Click "Bus Availability" tab
   - Click "+ Add Availability" button
   - Fill in the form and submit
   - Verify the record appears in the table

## Important: Database Migration Still Pending

Remember: The database still needs the SQL migration executed to add the three missing columns:
```sql
ALTER TABLE bus_availability
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);
```

Execute this before testing the availability feature, or you'll get a 500 error.
