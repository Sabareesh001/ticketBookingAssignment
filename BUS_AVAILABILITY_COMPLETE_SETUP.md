# Bus Availability Manager - Complete Setup Guide

## ✅ What Has Been Implemented

### Frontend (Angular)
1. **New Tab in Operator Dashboard** - "Bus Availability" tab added
2. **Full CRUD UI** with structured table display
3. **Advanced Filtering System**:
   - Filter by Bus (dropdown with full details)
   - Filter by Status (Active/Inactive)
   - Date Range filters (From/To)
   - Real-time search by bus registration or route
   - Clear filters button
4. **Smart Create/Edit Forms** with validation and auto-population
5. **Pagination** (10 records per page)
6. **Service Integration** - `BusAvailabilityService` with all CRUD methods

### Backend (C# .NET)
1. **Controller** - `BusAvailabilityController.cs` with all endpoints
2. **Service** - `BusAvailabilityService.cs` implementing `IBusAvailabilityService`
3. **DTOs** - All data transfer objects defined
4. **Model** - `BusAvailability.cs` with timing fields
5. **DbContext** - Updated with timing column mappings

## 🔧 Setup Steps

### Step 1: Restart Backend API

The backend needs to be restarted to register all changes:

```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5266
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 2: Verify Swagger UI

Open browser to: `http://localhost:5266/swagger`

You should see these BusAvailability endpoints:
- ✅ GET `/api/BusAvailability` - Get all availabilities
- ✅ GET `/api/BusAvailability/{id}` - Get by ID
- ✅ GET `/api/BusAvailability/bus/{busId}` - Get by bus
- ✅ POST `/api/BusAvailability` - Create availability
- ✅ PUT `/api/BusAvailability/{id}` - Update availability
- ✅ DELETE `/api/BusAvailability/{id}` - Delete availability
- ✅ POST `/api/BusAvailability/generate/{busId}` - Generate availability
- ✅ And more...

### Step 3: Test API Endpoint

Test the GET endpoint:
```bash
curl http://localhost:5266/api/busavailability
```

**Expected Response:**
- `[]` (empty array if no data)
- Or array of availability records

### Step 4: Start Frontend

```bash
cd frontend/bus-booking
npm start
```

### Step 5: Test the UI

1. Navigate to: `http://localhost:4200`
2. Login as operator
3. Click on "Bus Availability" tab
4. Should load without 404 errors

## 📋 Database Schema

The `bus_availability` table should have these columns:

```sql
CREATE TABLE bus_availability (
    id SERIAL PRIMARY KEY,
    bus_id INTEGER NOT NULL REFERENCES buses(id) ON DELETE CASCADE,
    available_date DATE NOT NULL,
    total_seats INTEGER NOT NULL,
    available_seats INTEGER NOT NULL,
    is_active BOOLEAN DEFAULT true,
    schedule_id INTEGER REFERENCES bus_schedules(id) ON DELETE CASCADE,
    pickup_time TIME,
    drop_time TIME,
    journey_duration_hours NUMERIC(5,2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(bus_id, available_date)
);
```

### Check if Columns Exist

Run this SQL query to verify:

```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
ORDER BY ordinal_position;
```

### Add Missing Columns (if needed)

If the timing columns are missing, run:

```sql
ALTER TABLE bus_availability 
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);
```

## 🎯 Features Overview

### 1. Table Display
- Shows bus registration, route, date, seats, timing, status
- Color-coded seat availability (orange for low, red for none)
- Formatted times (12-hour with AM/PM)
- Action buttons (Edit/Delete)

### 2. Filters
- **Bus Filter**: Dropdown showing "REG123 (Mumbai → Pune) (45 seats)"
- **Status Filter**: Active/Inactive
- **Date Range**: From and To date pickers
- **Search**: Real-time search by registration or route
- **Clear**: Reset all filters

### 3. Create/Edit Form
- **Bus Selection**: Auto-populates seat capacity
- **Date Picker**: Minimum date = today
- **Seat Fields**: Validates available ≤ total
- **Time Pickers**: Optional pickup/drop times
- **Duration**: Optional journey duration in hours
- **Status**: Active/Inactive toggle
- **Helper Text**: Contextual hints for each field

### 4. Validation
- Required field validation
- Date validation (no past dates)
- Seat validation (available ≤ total)
- Real-time error messages
- Form-level error display

### 5. User Feedback
- Success toast notifications
- Error toast notifications
- Loading states
- Empty states
- No results states

## 🔍 Troubleshooting

### Issue: 404 Error on API Call

**Cause**: Backend not running or not restarted after changes

**Solution**:
1. Stop backend (Ctrl+C)
2. Run: `dotnet clean && dotnet build && dotnet run`
3. Verify Swagger UI loads
4. Test endpoint: `curl http://localhost:5266/api/busavailability`

### Issue: Empty Table

**Cause**: No availability records in database

**Solution**:
1. Create a bus first (in Bus Manager tab)
2. Click "+ Add Availability" button
3. Select bus, date, and seats
4. Click "Create"

### Issue: Timing Columns Not Saving

**Cause**: Database columns missing

**Solution**:
Run the ALTER TABLE command above to add missing columns

### Issue: CORS Error

**Cause**: Backend CORS not configured

**Solution**:
Verify `Program.cs` has:
```csharp
app.UseCors("AllowAll");
```

### Issue: Authentication Error

**Cause**: Operator not logged in or token expired

**Solution**:
1. Logout and login again
2. Check browser console for auth errors
3. Verify operator-auth.interceptor is working

## 📊 API Endpoints Reference

### Get All Availabilities
```http
GET /api/busavailability
Response: BusAvailabilityDto[]
```

### Get by ID
```http
GET /api/busavailability/{id}
Response: BusAvailabilityDto
```

### Get by Bus
```http
GET /api/busavailability/bus/{busId}
Response: BusAvailabilityDto[]
```

### Create Availability
```http
POST /api/busavailability
Content-Type: application/json

{
  "busId": 1,
  "availableDate": "2026-04-30",
  "totalSeats": 45,
  "availableSeats": 45,
  "isActive": true,
  "pickupTime": "08:00:00",
  "dropTime": "16:00:00",
  "journeyDurationHours": 8.0
}

Response: BusAvailabilityDto
```

### Update Availability
```http
PUT /api/busavailability/{id}
Content-Type: application/json

{
  "busId": 1,
  "availableDate": "2026-04-30",
  "totalSeats": 45,
  "availableSeats": 40,
  "isActive": true,
  "pickupTime": "08:00:00",
  "dropTime": "16:00:00",
  "journeyDurationHours": 8.0
}

Response: BusAvailabilityDto
```

### Delete Availability
```http
DELETE /api/busavailability/{id}
Response: { "message": "Availability deleted successfully" }
```

### Generate Availability (Bulk)
```http
POST /api/busavailability/generate/{busId}
Response: { "message": "Availability generated successfully for bus {busId}" }
```

## 🧪 Testing Checklist

### Backend Tests
- [ ] Swagger UI loads at http://localhost:5266/swagger
- [ ] GET /api/busavailability returns 200
- [ ] POST /api/busavailability creates record
- [ ] PUT /api/busavailability/{id} updates record
- [ ] DELETE /api/busavailability/{id} deletes record
- [ ] Database columns exist (pickup_time, drop_time, journey_duration_hours)

### Frontend Tests
- [ ] Bus Availability tab appears in dashboard
- [ ] Table loads without errors
- [ ] Filters work correctly
- [ ] Search works in real-time
- [ ] Create modal opens and validates
- [ ] Bus selection auto-populates seats
- [ ] Date picker prevents past dates
- [ ] Create saves successfully
- [ ] Edit modal pre-fills data
- [ ] Edit saves successfully
- [ ] Delete confirms and removes record
- [ ] Pagination works with filters
- [ ] Success/error messages display
- [ ] Loading states show during API calls

### Integration Tests
- [ ] Create availability for existing bus
- [ ] Edit availability and verify changes
- [ ] Delete availability and verify removal
- [ ] Filter by bus and verify results
- [ ] Filter by status and verify results
- [ ] Filter by date range and verify results
- [ ] Search by registration and verify results
- [ ] Clear filters and verify reset
- [ ] Navigate between pages and verify data

## 📝 Files Modified/Created

### Frontend Files
- ✅ `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.html`
- ✅ `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.ts`
- ✅ `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.css`
- ✅ `frontend/bus-booking/src/app/services/bus-availability.service.ts` (already existed)

### Backend Files
- ✅ `backend/BusBookingAPI/Controllers/BusAvailabilityController.cs` (already existed)
- ✅ `backend/BusBookingAPI/Services/BusAvailabilityService.cs` (already existed)
- ✅ `backend/BusBookingAPI/Services/IBusAvailabilityService.cs` (already existed)
- ✅ `backend/BusBookingAPI/DTOs/BusAvailabilityDto.cs` (already existed)
- ✅ `backend/BusBookingAPI/Models/BusAvailability.cs` (already existed)
- ✅ `backend/BusBookingAPI/Data/BusBookingDbContext.cs` (updated with timing columns)
- ✅ `backend/BusBookingAPI/Program.cs` (service already registered)

### Documentation Files
- ✅ `BUS_AVAILABILITY_MANAGER_IMPLEMENTATION.md`
- ✅ `BACKEND_RESTART_INSTRUCTIONS.md`
- ✅ `BUS_AVAILABILITY_COMPLETE_SETUP.md` (this file)

## 🚀 Next Steps

1. **Restart Backend** - Follow Step 1 above
2. **Verify API** - Check Swagger UI and test endpoints
3. **Test Frontend** - Login and test all CRUD operations
4. **Verify Database** - Check if timing columns exist
5. **Test Filters** - Verify all filtering and search functionality
6. **Test Pagination** - Create 10+ records and test pagination

## ✨ Success Criteria

You'll know everything is working when:
- ✅ No 404 errors in browser console
- ✅ Bus Availability tab loads data
- ✅ Can create new availability records
- ✅ Can edit existing records
- ✅ Can delete records with confirmation
- ✅ Filters work correctly
- ✅ Search returns relevant results
- ✅ Pagination works smoothly
- ✅ Success/error messages display properly
- ✅ All validations work as expected

## 🎉 You're Done!

The Bus Availability Manager is now fully implemented and ready to use. If you encounter any issues, refer to the Troubleshooting section above or check the logs in `backend/BusBookingAPI/logs/`.
