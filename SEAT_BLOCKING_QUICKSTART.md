# Seat Blocking Feature - Quick Start Guide

## What's New?
Users can now reserve seats for 5 minutes while they complete their booking. This prevents other users from booking the same seats simultaneously.

## Installation Steps

### 1. Database Migration
Run the migration script on your existing database:

```bash
psql -U postgres -d busBooking -f database/MIGRATION_SEAT_BLOCKING.sql
```

Or if you're setting up fresh, the changes are already in:
```bash
psql -U postgres -d busBooking -f database/setup/5_bookings.sql
```

### 2. Backend Setup
No additional packages needed. The changes are already integrated.

Just restart your backend:
```bash
cd backend/BusBookingAPI
dotnet run
```

The background cleanup service will start automatically.

### 3. Frontend Setup
No additional packages needed. The changes are already integrated.

Just restart your frontend:
```bash
cd frontend/bus-booking
npm start
```

## How to Use

### For End Users
1. Search for buses and select a bus
2. Click on seats to select them
3. **NEW**: A countdown timer appears showing 5:00 minutes
4. Complete your booking before the timer expires
5. If you close the modal or time expires, seats are released automatically

### For Developers

#### Reserve Seats (API)
```bash
curl -X POST http://localhost:5266/api/booking/reserve \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "busId": 5,
    "travelDate": "2026-05-01T00:00:00Z",
    "seatNumbers": "1, 2, 3"
  }'
```

#### Release Reservation (API)
```bash
curl -X DELETE http://localhost:5266/api/booking/reserve/123
```

#### Check Booked Seats (API)
```bash
curl http://localhost:5266/api/booking/bus/5/seats?travelDate=2026-05-01
```

## Testing Scenarios

### Scenario 1: Normal Booking Flow
1. User A selects seats 1, 2, 3
2. Timer starts at 5:00
3. User A completes booking
4. Seats are confirmed

### Scenario 2: Reservation Expiry
1. User A selects seats 1, 2, 3
2. Timer counts down to 0:00
3. Seats are automatically released
4. User B can now select those seats

### Scenario 3: Modal Close
1. User A selects seats 1, 2, 3
2. User A closes the modal
3. Seats are immediately released
4. User B can now select those seats

### Scenario 4: Concurrent Users
1. User A selects seats 1, 2, 3
2. User B tries to select seat 2
3. User B sees seat 2 as unavailable (booked)
4. After 5 minutes or if User A releases, User B can select seat 2

## Configuration

### Change Reservation Duration
Default is 5 minutes. To change:

**Backend** - `BookingService.cs` line ~380:
```csharp
var reservedUntil = DateTime.UtcNow.AddMinutes(5); // Change to desired minutes
```

**Frontend** - `booking.service.ts` response interface:
```typescript
remainingSeconds: 300 // Change to desired seconds (5 min = 300 sec)
```

### Change Cleanup Interval
Default is 1 minute. To change:

**Backend** - `ReservationCleanupService.cs` line ~9:
```csharp
private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);
```

## Monitoring

### Check Active Reservations
```sql
SELECT id, user_id, bus_id, seat_numbers, reserved_until, 
       EXTRACT(EPOCH FROM (reserved_until - NOW())) as seconds_remaining
FROM bookings
WHERE is_reserved = true 
  AND booking_status = 'reserved'
  AND reserved_until > NOW()
ORDER BY reserved_until;
```

### Check Expired Reservations (should be empty)
```sql
SELECT id, user_id, bus_id, seat_numbers, reserved_until
FROM bookings
WHERE is_reserved = true 
  AND booking_status = 'reserved'
  AND reserved_until <= NOW();
```

### View Cleanup Service Logs
```bash
# In backend logs
grep "Cleaning up expired reservations" backend/BusBookingAPI/logs/app-*.txt
grep "Cleaned up" backend/BusBookingAPI/logs/app-*.txt
```

## Troubleshooting

### Issue: Seats not releasing after 5 minutes
**Solution**: Check if background service is running:
```bash
# Look for this in logs
grep "Reservation Cleanup Service started" backend/BusBookingAPI/logs/app-*.txt
```

### Issue: Timer not showing in UI
**Solution**: Check browser console for errors. Ensure reservation API call succeeded.

### Issue: "Seats already reserved" error
**Solution**: Wait for reservation to expire or manually cleanup:
```bash
curl -X POST http://localhost:5266/api/booking/cleanup-expired
```

### Issue: Database constraint error
**Solution**: Ensure migration was run successfully:
```sql
\d bookings  -- Check table structure
```

## Performance Tips

1. **Database**: The new index on `(is_reserved, reserved_until)` speeds up cleanup queries
2. **Background Service**: Runs every minute - adjust if needed for high-traffic scenarios
3. **Frontend**: Timer uses `setInterval` - automatically cleared on component destroy
4. **API**: Cleanup is called before seat availability checks to ensure fresh data

## Security Notes

1. Users can only release their own reservations
2. Expired reservations are automatically removed
3. Backend validates all reservation requests
4. No user can reserve seats that are already reserved/booked

## Support

For issues or questions:
1. Check logs in `backend/BusBookingAPI/logs/`
2. Review `SEAT_BLOCKING_IMPLEMENTATION.md` for detailed documentation
3. Test API endpoints using Swagger at `http://localhost:5266`

## Rollback (if needed)

To remove the feature:

```sql
-- Remove columns
ALTER TABLE bookings DROP COLUMN IF EXISTS is_reserved;
ALTER TABLE bookings DROP COLUMN IF EXISTS reserved_until;

-- Restore original constraint
ALTER TABLE bookings DROP CONSTRAINT IF EXISTS bookings_booking_status_check;
ALTER TABLE bookings ADD CONSTRAINT bookings_booking_status_check 
CHECK (booking_status IN ('confirmed', 'cancelled', 'pending'));

-- Remove index
DROP INDEX IF EXISTS idx_bookings_reserved;
```

Then revert code changes using git.
