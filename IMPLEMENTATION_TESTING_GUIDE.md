# Bus Timing Implementation Testing Guide

## Overview
This guide provides step-by-step instructions to test the date-specific pickup and drop time functionality that has been implemented in the bus booking system.

## Prerequisites

### Database Setup
1. **Run the ALTER queries** (from PICKUP_DROP_TIME_IMPLEMENTATION_SUMMARY.md):
```sql
-- Add timing columns to bus_availability table
ALTER TABLE bus_availability 
ADD COLUMN pickup_time TIME DEFAULT '08:00:00',
ADD COLUMN drop_time TIME DEFAULT '18:00:00',
ADD COLUMN journey_duration_hours DECIMAL(5,2) DEFAULT 10.00;

-- Update existing records with default timings
UPDATE bus_availability ba 
SET pickup_time = b.pickup_time,
    drop_time = b.drop_time,
    journey_duration_hours = b.journey_duration_hours
FROM buses b 
WHERE ba.bus_id = b.id;
```

### Backend Setup
1. **Ensure the backend service methods are implemented** in `BusAvailabilityService.cs`
2. **Verify the new API endpoints** are working:
   - `GET /api/busavailability/available-dates-with-timing/{busId}`
   - `PUT /api/busavailability/update-timing`
   - `PUT /api/busavailability/bulk-update-timing`

### Frontend Setup
1. **Ensure all components are properly imported** in the Angular application
2. **Verify routing** is configured for `/bus-timing-management`

## Testing Scenarios

### 1. Basic Functionality Test

#### Step 1: Access Bus Timing Management
1. Navigate to the dashboard
2. Click on "⏰ Bus Timing" in the navigation menu
3. **Expected**: You should see the Bus Timing Management page

#### Step 2: Select a Bus
1. Use the dropdown to select a bus
2. **Expected**: Available dates should load automatically
3. **Expected**: Each date should show current pickup/drop times

#### Step 3: Generate Availability (if needed)
1. If no dates are shown, click "Generate Availability"
2. **Expected**: Availability records should be created
3. **Expected**: Dates should appear in the table

### 2. Single Date Timing Update

#### Step 1: Update Single Date
1. Select a specific date from the dropdown
2. Set new pickup time (e.g., 09:00)
3. Set new drop time (e.g., 19:00)
4. Set journey duration (e.g., 10 hours)
5. Click "Update Single Date"

#### Step 2: Verify Update
1. **Expected**: Success message should appear
2. **Expected**: The table should refresh showing new timing
3. **Expected**: Only the selected date should have updated timing

### 3. Bulk Timing Update

#### Step 1: Select Multiple Dates
1. Use checkboxes to select multiple dates
2. Or click "Select All" to select all dates
3. **Expected**: Selected count should update

#### Step 2: Set New Timing
1. Set new pickup time (e.g., 07:30)
2. Set new drop time (e.g., 17:30)
3. Set journey duration (e.g., 10 hours)
4. Click "Update Selected Dates"

#### Step 3: Verify Bulk Update
1. **Expected**: Success message with count of updated dates
2. **Expected**: All selected dates should show new timing
3. **Expected**: Table should refresh automatically

### 4. Bus Search with Date-Specific Timing

#### Step 1: Search for Buses
1. Go back to the dashboard
2. Select source and destination districts
3. Select a travel date
4. Click "Search Buses"

#### Step 2: Verify Date-Specific Timing Display
1. **Expected**: Bus results should show timing specific to the selected date
2. **Expected**: Timing should match what was set in the timing management
3. **Expected**: Small note should show "*For [selected date]"

### 5. Booking with Correct Timing

#### Step 1: Select a Bus
1. Click "Select Seats" on a bus
2. **Expected**: Seat selection modal should open
3. **Expected**: Bus info should show correct timing for the selected date

#### Step 2: Complete Booking
1. Select seats and confirm booking
2. **Expected**: Booking confirmation should show the actual pickup/drop times for that specific date

### 6. API Testing (Optional - for developers)

#### Test New Endpoints
```bash
# Get available dates with timing
GET /api/busavailability/available-dates-with-timing/1

# Update single date timing
PUT /api/busavailability/update-timing
{
  "busId": 1,
  "availableDate": "2026-04-25",
  "pickupTime": "09:00:00",
  "dropTime": "19:00:00",
  "journeyDurationHours": 10
}

# Bulk update timing
PUT /api/busavailability/bulk-update-timing
{
  "busId": 1,
  "dates": ["2026-04-25", "2026-04-26"],
  "pickupTime": "08:30:00",
  "dropTime": "18:30:00",
  "journeyDurationHours": 10
}
```

## Expected Results

### Database Changes
- `bus_availability` table should have new timing columns
- Each availability record should have date-specific timing
- Default timing should be inherited from bus configuration

### Frontend Changes
- Bus timing management page should be accessible
- Bus search results should show date-specific timing
- Booking confirmation should use actual timing for the date

### Backend Changes
- New API endpoints should return timing information
- Service methods should handle timing updates
- Availability generation should include timing data

## Troubleshooting

### Common Issues

#### 1. No Available Dates Shown
- **Solution**: Click "Generate Availability" button
- **Cause**: Bus availability records haven't been created yet

#### 2. Timing Updates Not Reflected
- **Solution**: Refresh the page or reload available dates
- **Cause**: Frontend cache or component state issue

#### 3. API Errors
- **Solution**: Check backend logs and ensure database schema is updated
- **Cause**: Missing database columns or service implementation

#### 4. Bus Search Shows Wrong Timing
- **Solution**: Verify that availability records have correct timing data
- **Cause**: Availability records may not have been updated with new timing

### Verification Queries

```sql
-- Check if timing columns exist
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
AND column_name IN ('pickup_time', 'drop_time', 'journey_duration_hours');

-- Check timing data for a specific bus
SELECT available_date, pickup_time, drop_time, journey_duration_hours 
FROM bus_availability 
WHERE bus_id = 1 
ORDER BY available_date;

-- Verify timing updates
SELECT ba.available_date, ba.pickup_time, ba.drop_time, b.pickup_time as bus_pickup_time
FROM bus_availability ba
JOIN buses b ON ba.bus_id = b.id
WHERE ba.bus_id = 1
ORDER BY ba.available_date;
```

## Success Criteria

✅ **Database**: Timing columns added and populated  
✅ **Backend**: New API endpoints working correctly  
✅ **Frontend**: Timing management interface functional  
✅ **Integration**: Bus search shows date-specific timing  
✅ **Booking**: Confirmation uses correct timing  
✅ **User Experience**: Intuitive and responsive interface  

## Next Steps

After successful testing:
1. **Performance Testing**: Test with large datasets
2. **User Acceptance Testing**: Get feedback from actual users
3. **Documentation**: Update user manuals and API documentation
4. **Monitoring**: Set up logging and monitoring for the new features
5. **Backup**: Ensure database backup includes new timing data

This implementation provides a solid foundation for date-specific bus timing management while maintaining backward compatibility and providing an excellent user experience.