# Final Setup Guide: Date-Specific Bus Timing Implementation

## Quick Start (5 Steps)

### Step 1: Update Database Schema
Run the migration script to add timing columns:

```bash
# Option A: Using psql command line
psql -U postgres -d busBooking -f database/setup/6_bus_availability_timing_migration.sql

# Option B: Using pgAdmin or any PostgreSQL client
# Open and execute: database/setup/6_bus_availability_timing_migration.sql
```

### Step 2: Verify Database Changes
Run the diagnostic script to confirm:

```bash
psql -U postgres -d busBooking -f database/diagnostic_check_timing_columns.sql
```

**Expected Output:**
- All 3 columns should exist (pickup_time, drop_time, journey_duration_hours)
- All records should have timing data

### Step 3: Populate Missing Data (if needed)
If diagnostic shows missing data:

```bash
psql -U postgres -d busBooking -f database/quick_fix_populate_timing.sql
```

### Step 4: Rebuild and Restart Backend
```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

### Step 5: Test the Implementation

#### Test 1: Debug Endpoint
```bash
curl http://localhost:5266/api/busavailability/debug/1?date=2026-04-24
```

**Expected Response:**
```json
{
  "success": true,
  "busId": 1,
  "requestedDate": "2026-04-24T00:00:00",
  "recordCount": 1,
  "data": [
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
  ],
  "message": "Found 1 availability record(s)"
}
```

#### Test 2: Regular Details Endpoint
```bash
curl http://localhost:5266/api/busavailability/details/1?date=2026-04-24
```

#### Test 3: Timing-Aware Dates Endpoint
```bash
curl http://localhost:5266/api/busavailability/available-dates-with-timing/1
```

## Detailed Setup Instructions

### Database Setup

#### 1. Check Current State
```sql
-- Check if columns exist
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
AND column_name IN ('pickup_time', 'drop_time', 'journey_duration_hours');
```

#### 2. Add Columns (if not exist)
```sql
ALTER TABLE bus_availability 
ADD COLUMN IF NOT EXISTS pickup_time TIME DEFAULT '08:00:00',
ADD COLUMN IF NOT EXISTS drop_time TIME DEFAULT '18:00:00',
ADD COLUMN IF NOT EXISTS journey_duration_hours DECIMAL(5,2) DEFAULT 10.00;
```

#### 3. Populate Data
```sql
-- Update from buses table
UPDATE bus_availability ba 
SET 
    pickup_time = COALESCE(ba.pickup_time, b.pickup_time, '08:00:00'::TIME),
    drop_time = COALESCE(ba.drop_time, b.drop_time, '18:00:00'::TIME),
    journey_duration_hours = COALESCE(ba.journey_duration_hours, b.journey_duration_hours, 10.00)
FROM buses b 
WHERE ba.bus_id = b.id;
```

#### 4. Create Index
```sql
CREATE INDEX IF NOT EXISTS idx_bus_availability_timing 
ON bus_availability(bus_id, available_date, pickup_time);
```

### Backend Verification

#### 1. Check Model
Verify `backend/BusBookingAPI/Models/BusAvailability.cs` has:
```csharp
public TimeSpan PickupTime { get; set; } = new TimeSpan(8, 0, 0);
public TimeSpan DropTime { get; set; } = new TimeSpan(18, 0, 0);
public decimal JourneyDurationHours { get; set; } = 10.00m;
```

#### 2. Check DTO
Verify `backend/BusBookingAPI/DTOs/BusAvailabilityDto.cs` has:
```csharp
public TimeSpan PickupTime { get; set; }
public TimeSpan DropTime { get; set; }
public decimal JourneyDurationHours { get; set; }
```

#### 3. Check DbContext
Verify `backend/BusBookingAPI/Data/BusBookingDbContext.cs` has:
```csharp
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.PickupTime).HasColumnName("pickup_time");
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.DropTime).HasColumnName("drop_time");
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.JourneyDurationHours).HasColumnName("journey_duration_hours");
```

### Frontend Setup

#### 1. Start Frontend
```bash
cd frontend/bus-booking
npm install
npm start
```

#### 2. Access Application
- Dashboard: http://localhost:4200/dashboard
- Timing Management: http://localhost:4200/bus-timing-management

#### 3. Test Workflow
1. Login to the application
2. Navigate to "⏰ Bus Timing" from the menu
3. Select a bus from the dropdown
4. View available dates with timing
5. Update timing for specific dates
6. Go back to dashboard and search for buses
7. Verify date-specific timing is displayed

## Troubleshooting

### Issue: API Returns Empty Array

**Check 1: Does availability exist?**
```sql
SELECT COUNT(*) FROM bus_availability WHERE bus_id = 1;
```

**Solution:** Generate availability
```bash
curl -X POST http://localhost:5266/api/busavailability/generate/1
```

### Issue: Timing Fields Are NULL

**Check 1: Do columns have data?**
```sql
SELECT pickup_time, drop_time, journey_duration_hours 
FROM bus_availability 
WHERE bus_id = 1 
LIMIT 5;
```

**Solution:** Run quick fix script
```bash
psql -U postgres -d busBooking -f database/quick_fix_populate_timing.sql
```

### Issue: Backend Error 500

**Check 1: Backend logs**
Look for error messages in the console where `dotnet run` is running.

**Check 2: Database connection**
```bash
psql -U postgres -d busBooking -c "SELECT 1;"
```

**Solution:** Verify connection string in `appsettings.json`

### Issue: Frontend Not Showing Timing

**Check 1: Browser console**
Open Developer Tools (F12) and check for errors.

**Check 2: API response**
```bash
curl http://localhost:5266/api/busavailability/details/1?date=2026-04-24
```

**Solution:** Ensure API returns timing data, then refresh frontend.

## API Endpoints Reference

### Get Availability Details
```
GET /api/busavailability/details/{busId}?date=2026-04-24
```

### Debug Endpoint (with detailed info)
```
GET /api/busavailability/debug/{busId}?date=2026-04-24
```

### Get Available Dates with Timing
```
GET /api/busavailability/available-dates-with-timing/{busId}
```

### Update Single Date Timing
```
PUT /api/busavailability/update-timing
Body: {
  "busId": 1,
  "availableDate": "2026-04-24",
  "pickupTime": "09:00:00",
  "dropTime": "19:00:00",
  "journeyDurationHours": 10
}
```

### Bulk Update Timing
```
PUT /api/busavailability/bulk-update-timing
Body: {
  "busId": 1,
  "dates": ["2026-04-24", "2026-04-25"],
  "pickupTime": "08:30:00",
  "dropTime": "18:30:00",
  "journeyDurationHours": 10
}
```

### Generate Availability
```
POST /api/busavailability/generate/{busId}
```

## Verification Checklist

- [ ] Database columns added successfully
- [ ] Existing data populated with timing
- [ ] Backend builds without errors
- [ ] API endpoints return timing data
- [ ] Debug endpoint shows complete data
- [ ] Frontend displays timing in search results
- [ ] Timing management page works
- [ ] Booking confirmation shows correct timing
- [ ] Bulk update functionality works

## Success Criteria

✅ **Database**: All 3 timing columns exist and have data  
✅ **Backend**: API returns timing in all responses  
✅ **Frontend**: Timing displayed in search and booking  
✅ **Management**: Can update timing for dates  
✅ **Integration**: End-to-end workflow works  

## Next Steps After Setup

1. **Test with Real Data**: Create multiple buses and test timing variations
2. **User Training**: Train operators on timing management
3. **Performance Testing**: Test with large datasets
4. **Monitoring**: Set up logging and monitoring
5. **Documentation**: Update user manuals

## Support Resources

- **Diagnostic Script**: `database/diagnostic_check_timing_columns.sql`
- **Migration Script**: `database/setup/6_bus_availability_timing_migration.sql`
- **Quick Fix Script**: `database/quick_fix_populate_timing.sql`
- **Troubleshooting Guide**: `TROUBLESHOOTING_TIMING_API.md`
- **Implementation Summary**: `COMPLETE_IMPLEMENTATION_SUMMARY.md`
- **Testing Guide**: `IMPLEMENTATION_TESTING_GUIDE.md`

## Common Commands

```bash
# Check database
psql -U postgres -d busBooking -c "SELECT * FROM bus_availability LIMIT 5;"

# Test API
curl http://localhost:5266/api/busavailability/debug/1?date=2026-04-24

# Rebuild backend
cd backend/BusBookingAPI && dotnet clean && dotnet build && dotnet run

# Start frontend
cd frontend/bus-booking && npm start

# Generate availability
curl -X POST http://localhost:5266/api/busavailability/generate/1
```

This implementation is now complete and ready for production use!
