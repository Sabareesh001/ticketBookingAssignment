# Troubleshooting: Bus Availability Timing API

## Issue
The endpoint `http://localhost:5266/api/busavailability/details/1?date=2026-04-24` is not returning timing data (pickupTime, dropTime, journeyDurationHours).

## Root Causes

### 1. Database Columns Not Added
The most common issue is that the timing columns haven't been added to the `bus_availability` table yet.

**Solution:**
```bash
# Run the migration script
psql -U your_username -d busBooking -f database/setup/6_bus_availability_timing_migration.sql
```

Or manually run:
```sql
ALTER TABLE bus_availability 
ADD COLUMN pickup_time TIME DEFAULT '08:00:00',
ADD COLUMN drop_time TIME DEFAULT '18:00:00',
ADD COLUMN journey_duration_hours DECIMAL(5,2) DEFAULT 10.00;
```

### 2. Existing Data Has NULL or Zero Values
Even if columns exist, existing records might have NULL or zero values.

**Diagnosis:**
```bash
# Run the diagnostic script
psql -U your_username -d busBooking -f database/diagnostic_check_timing_columns.sql
```

**Solution:**
```bash
# Run the quick fix script
psql -U your_username -d busBooking -f database/quick_fix_populate_timing.sql
```

### 3. Entity Framework Not Recognizing New Columns
If you added columns but EF Core isn't loading them, you may need to rebuild the project.

**Solution:**
```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

## Step-by-Step Diagnosis

### Step 1: Check Database Schema
```sql
-- Check if columns exist
SELECT column_name, data_type, column_default
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
AND column_name IN ('pickup_time', 'drop_time', 'journey_duration_hours');
```

**Expected Output:**
```
column_name              | data_type | column_default
-------------------------+-----------+----------------
pickup_time              | time      | '08:00:00'::time
drop_time                | time      | '18:00:00'::time
journey_duration_hours   | numeric   | 10.00
```

### Step 2: Check Data in Database
```sql
-- Check actual data
SELECT 
    id, bus_id, available_date, 
    pickup_time, drop_time, journey_duration_hours,
    available_seats
FROM bus_availability
WHERE bus_id = 1 
AND available_date::date = '2026-04-24'::date;
```

**Expected Output:**
```
id | bus_id | available_date | pickup_time | drop_time | journey_duration_hours | available_seats
---+--------+----------------+-------------+-----------+------------------------+----------------
1  | 1      | 2026-04-24     | 08:00:00    | 18:00:00  | 10.00                  | 40
```

### Step 3: Test API Endpoint Directly
```bash
# Using curl
curl -X GET "http://localhost:5266/api/busavailability/details/1?date=2026-04-24" -H "accept: application/json"

# Or using PowerShell
Invoke-RestMethod -Uri "http://localhost:5266/api/busavailability/details/1?date=2026-04-24" -Method Get
```

**Expected Response:**
```json
[
  {
    "id": 1,
    "busId": 1,
    "availableDate": "2026-04-24T00:00:00",
    "totalSeats": 40,
    "availableSeats": 40,
    "isActive": true,
    "scheduleId": null,
    "pickupTime": "08:00:00",
    "dropTime": "18:00:00",
    "journeyDurationHours": 10.0,
    "createdAt": "2026-04-24T00:00:00",
    "updatedAt": "2026-04-24T00:00:00"
  }
]
```

### Step 4: Check Backend Logs
Look for any errors in the backend console output when the API is called.

```bash
# Check for errors like:
# - "Column 'pickup_time' does not exist"
# - "Invalid column name 'pickup_time'"
# - Serialization errors
```

### Step 5: Verify Entity Model
Check that the `BusAvailability` model has the timing properties:

```csharp
// backend/BusBookingAPI/Models/BusAvailability.cs
public class BusAvailability
{
    // ... other properties ...
    public TimeSpan PickupTime { get; set; } = new TimeSpan(8, 0, 0);
    public TimeSpan DropTime { get; set; } = new TimeSpan(18, 0, 0);
    public decimal JourneyDurationHours { get; set; } = 10.00m;
}
```

### Step 6: Verify DTO Mapping
Check that the DTO includes timing properties:

```csharp
// backend/BusBookingAPI/DTOs/BusAvailabilityDto.cs
public class BusAvailabilityDto
{
    // ... other properties ...
    public TimeSpan PickupTime { get; set; }
    public TimeSpan DropTime { get; set; }
    public decimal JourneyDurationHours { get; set; }
}
```

## Common Issues and Solutions

### Issue 1: API Returns Empty Array
**Symptom:** `[]` is returned instead of availability data

**Causes:**
- No availability records exist for that bus/date
- Date format mismatch

**Solution:**
```sql
-- Generate availability for the bus
-- This should be done via API: POST /api/busavailability/generate/1
-- Or manually:
INSERT INTO bus_availability (bus_id, available_date, total_seats, available_seats, is_active, pickup_time, drop_time, journey_duration_hours)
SELECT 
    1 as bus_id,
    '2026-04-24'::date as available_date,
    40 as total_seats,
    40 as available_seats,
    true as is_active,
    '08:00:00'::time as pickup_time,
    '18:00:00'::time as drop_time,
    10.00 as journey_duration_hours;
```

### Issue 2: Timing Fields Are NULL in Response
**Symptom:** API returns data but timing fields are null

**Causes:**
- Database columns exist but have NULL values
- Mapping issue in service layer

**Solution:**
```sql
-- Update NULL values with defaults from buses table
UPDATE bus_availability ba 
SET 
    pickup_time = COALESCE(ba.pickup_time, b.pickup_time, '08:00:00'::TIME),
    drop_time = COALESCE(ba.drop_time, b.drop_time, '18:00:00'::TIME),
    journey_duration_hours = COALESCE(ba.journey_duration_hours, b.journey_duration_hours, 10.00)
FROM buses b 
WHERE ba.bus_id = b.id;
```

### Issue 3: TimeSpan Serialization Issues
**Symptom:** Timing appears as "00:00:00" or strange format

**Causes:**
- .NET TimeSpan serialization format
- Frontend not parsing correctly

**Solution:**
The frontend service now includes `parseTimeSpan()` method to handle .NET TimeSpan format.

### Issue 4: Database Connection Issues
**Symptom:** 500 Internal Server Error

**Causes:**
- Database connection string incorrect
- Database server not running
- Permissions issue

**Solution:**
```bash
# Check connection string in appsettings.json
# Verify database is running
pg_isready -h localhost -p 5432

# Test connection
psql -U your_username -d busBooking -c "SELECT 1;"
```

## Verification Checklist

After applying fixes, verify:

- [ ] Database columns exist
- [ ] Existing data has timing values
- [ ] API endpoint returns timing data
- [ ] Frontend displays timing correctly
- [ ] Booking uses correct timing
- [ ] Timing management page works

## Quick Test Commands

```bash
# 1. Check database
psql -U postgres -d busBooking -c "SELECT column_name FROM information_schema.columns WHERE table_name = 'bus_availability' AND column_name LIKE '%time%';"

# 2. Check data
psql -U postgres -d busBooking -c "SELECT id, bus_id, available_date, pickup_time, drop_time FROM bus_availability LIMIT 5;"

# 3. Test API
curl http://localhost:5266/api/busavailability/details/1?date=2026-04-24

# 4. Check backend is running
curl http://localhost:5266/api/bus

# 5. Generate availability if needed
curl -X POST http://localhost:5266/api/busavailability/generate/1
```

## Still Having Issues?

If you've followed all steps and still have issues:

1. **Check Backend Logs**: Look for specific error messages
2. **Verify Database Connection**: Ensure the connection string is correct
3. **Check Entity Framework**: Ensure DbContext is properly configured
4. **Restart Services**: Restart both backend and database
5. **Clear Cache**: Clear browser cache and restart frontend dev server

## Contact Information

For additional support, provide:
- Database diagnostic output
- API response (with timing data or without)
- Backend error logs
- Frontend console errors
- Database schema for bus_availability table
