# Seat Blocking Feature - Deployment Checklist

## Pre-Deployment Checks

### Database
- [ ] Backup current database
  ```bash
  pg_dump -U postgres busBooking > backup_before_seat_blocking.sql
  ```
- [ ] Review migration script: `database/MIGRATION_SEAT_BLOCKING.sql`
- [ ] Test migration on development database first
- [ ] Verify no active bookings will be affected

### Backend
- [ ] All files compile without errors
- [ ] No breaking changes to existing APIs
- [ ] Background service registered in Program.cs
- [ ] Environment variables configured (if any)
- [ ] Logging configured properly

### Frontend
- [ ] All TypeScript files compile without errors
- [ ] No console errors in development
- [ ] Timer displays correctly
- [ ] Responsive design works on mobile

## Deployment Steps

### Step 1: Database Migration
```bash
# Connect to database
psql -U postgres -d busBooking

# Run migration
\i database/MIGRATION_SEAT_BLOCKING.sql

# Verify changes
\d bookings

# Check new columns exist
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'bookings' 
AND column_name IN ('is_reserved', 'reserved_until');

# Check index created
\di idx_bookings_reserved
```

**Expected Output:**
```
column_name    | data_type
---------------+-----------
is_reserved    | boolean
reserved_until | timestamp

Index "public.idx_bookings_reserved"
```

- [ ] Migration completed successfully
- [ ] Columns added
- [ ] Index created
- [ ] Constraint updated

### Step 2: Backend Deployment
```bash
cd backend/BusBookingAPI

# Clean and build
dotnet clean
dotnet build

# Run tests (if any)
dotnet test

# Publish for production
dotnet publish -c Release -o ./publish

# Start application
dotnet run
```

**Verify:**
- [ ] Application starts without errors
- [ ] Check logs for "Reservation Cleanup Service started"
- [ ] Swagger UI accessible at http://localhost:5266
- [ ] New endpoints visible in Swagger:
  - POST /api/booking/reserve
  - DELETE /api/booking/reserve/{id}
  - POST /api/booking/cleanup-expired

### Step 3: Frontend Deployment
```bash
cd frontend/bus-booking

# Install dependencies (if needed)
npm install

# Build for production
npm run build

# Or start development server
npm start
```

**Verify:**
- [ ] Application builds without errors
- [ ] No TypeScript compilation errors
- [ ] No console errors on page load

### Step 4: Integration Testing

#### Test 1: Basic Reservation Flow
1. [ ] Search for buses
2. [ ] Select a bus
3. [ ] Select seats (1, 2, 3)
4. [ ] Verify timer appears showing 5:00
5. [ ] Wait 10 seconds
6. [ ] Verify timer shows 4:50
7. [ ] Complete booking
8. [ ] Verify booking confirmed

#### Test 2: Reservation Expiry
1. [ ] Search for buses
2. [ ] Select a bus
3. [ ] Select seats (4, 5)
4. [ ] Wait for timer to reach 0:00
5. [ ] Verify seats are deselected
6. [ ] Verify error message appears
7. [ ] Verify seats can be selected again

#### Test 3: Modal Close
1. [ ] Search for buses
2. [ ] Select a bus
3. [ ] Select seats (6, 7)
4. [ ] Close modal
5. [ ] Reopen modal
6. [ ] Verify seats are available again

#### Test 4: Concurrent Users
1. [ ] Open two browser windows
2. [ ] Window A: Select seats (8, 9)
3. [ ] Window B: Try to select seat 8
4. [ ] Verify Window B shows seat 8 as booked
5. [ ] Window A: Close modal
6. [ ] Window B: Refresh seat view
7. [ ] Verify Window B can now select seat 8

#### Test 5: Background Cleanup
1. [ ] Create a reservation via API
2. [ ] Wait 6 minutes
3. [ ] Check database for expired reservations
   ```sql
   SELECT * FROM bookings 
   WHERE is_reserved = true 
   AND reserved_until <= NOW();
   ```
4. [ ] Verify no expired reservations exist

### Step 5: API Testing

#### Test Reserve Endpoint
```bash
curl -X POST http://localhost:5266/api/booking/reserve \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "busId": 5,
    "travelDate": "2026-05-01T00:00:00Z",
    "seatNumbers": "10, 11"
  }'
```

**Expected Response:**
```json
{
  "reservationId": 123,
  "seatNumbers": "10, 11",
  "reservedUntil": "2026-04-24T10:35:00Z",
  "remainingSeconds": 300
}
```

- [ ] Status: 200 OK
- [ ] Response contains reservationId
- [ ] reservedUntil is ~5 minutes in future

#### Test Release Endpoint
```bash
curl -X DELETE http://localhost:5266/api/booking/reserve/123
```

**Expected Response:**
```json
{
  "message": "Reservation released successfully"
}
```

- [ ] Status: 200 OK
- [ ] Reservation deleted from database

#### Test Cleanup Endpoint
```bash
curl -X POST http://localhost:5266/api/booking/cleanup-expired
```

**Expected Response:**
```json
{
  "message": "Expired reservations cleaned up successfully"
}
```

- [ ] Status: 200 OK
- [ ] Expired reservations removed

## Post-Deployment Verification

### Database Checks
```sql
-- Check for any orphaned reservations
SELECT COUNT(*) FROM bookings 
WHERE is_reserved = true 
AND reserved_until <= NOW();
-- Should be 0

-- Check active reservations
SELECT COUNT(*) FROM bookings 
WHERE is_reserved = true 
AND reserved_until > NOW();
-- Should match current user activity

-- Check confirmed bookings not affected
SELECT COUNT(*) FROM bookings 
WHERE booking_status = 'confirmed' 
AND is_reserved = false;
-- Should match pre-deployment count
```

- [ ] No orphaned reservations
- [ ] Active reservations match user activity
- [ ] Existing bookings unaffected

### Log Checks
```bash
# Check backend logs
tail -f backend/BusBookingAPI/logs/app-*.txt

# Look for:
# - "Reservation Cleanup Service started"
# - "Reserving seats"
# - "Cleaned up X expired reservations"
# - No error messages
```

- [ ] Cleanup service started
- [ ] Reservation operations logged
- [ ] No unexpected errors

### Performance Checks
```sql
-- Check query performance
EXPLAIN ANALYZE 
SELECT * FROM bookings 
WHERE is_reserved = true 
AND reserved_until > NOW();

-- Should use idx_bookings_reserved index
```

- [ ] Index being used
- [ ] Query time < 10ms

### Browser Checks
- [ ] No console errors
- [ ] Timer updates smoothly
- [ ] No memory leaks (check DevTools)
- [ ] Works in Chrome
- [ ] Works in Firefox
- [ ] Works in Safari
- [ ] Works on mobile

## Monitoring Setup

### Database Monitoring
Create a monitoring query:
```sql
-- Save as monitoring_reservations.sql
SELECT 
  COUNT(*) FILTER (WHERE is_reserved = true AND reserved_until > NOW()) as active_reservations,
  COUNT(*) FILTER (WHERE is_reserved = true AND reserved_until <= NOW()) as expired_reservations,
  COUNT(*) FILTER (WHERE booking_status = 'confirmed') as confirmed_bookings,
  AVG(EXTRACT(EPOCH FROM (reserved_until - created_at))) FILTER (WHERE is_reserved = true) as avg_reservation_duration
FROM bookings
WHERE created_at > NOW() - INTERVAL '1 hour';
```

- [ ] Monitoring query created
- [ ] Schedule to run every 5 minutes

### Application Monitoring
- [ ] Set up alerts for:
  - High number of expired reservations
  - Background service failures
  - API endpoint errors
  - Database connection issues

### Log Monitoring
- [ ] Configure log aggregation
- [ ] Set up alerts for ERROR level logs
- [ ] Monitor reservation conversion rate

## Rollback Plan

If issues occur, follow these steps:

### 1. Stop Applications
```bash
# Stop backend
pkill -f "dotnet.*BusBookingAPI"

# Stop frontend
pkill -f "ng serve"
```

### 2. Rollback Database
```sql
-- Remove new columns
ALTER TABLE bookings DROP COLUMN IF EXISTS is_reserved;
ALTER TABLE bookings DROP COLUMN IF EXISTS reserved_until;

-- Restore original constraint
ALTER TABLE bookings DROP CONSTRAINT IF EXISTS bookings_booking_status_check;
ALTER TABLE bookings ADD CONSTRAINT bookings_booking_status_check 
CHECK (booking_status IN ('confirmed', 'cancelled', 'pending'));

-- Remove index
DROP INDEX IF EXISTS idx_bookings_reserved;
```

### 3. Rollback Code
```bash
# Backend
cd backend/BusBookingAPI
git checkout HEAD~1 -- .

# Frontend
cd frontend/bus-booking
git checkout HEAD~1 -- .
```

### 4. Restart Applications
```bash
# Backend
cd backend/BusBookingAPI
dotnet run

# Frontend
cd frontend/bus-booking
npm start
```

- [ ] Rollback procedure documented
- [ ] Rollback tested in development

## Sign-Off

### Development Team
- [ ] Code reviewed
- [ ] Tests passed
- [ ] Documentation complete

### QA Team
- [ ] All test scenarios passed
- [ ] No critical bugs found
- [ ] Performance acceptable

### DevOps Team
- [ ] Deployment scripts ready
- [ ] Monitoring configured
- [ ] Rollback plan tested

### Product Owner
- [ ] Feature meets requirements
- [ ] User experience approved
- [ ] Ready for production

## Final Checklist

- [ ] All pre-deployment checks completed
- [ ] Database migration successful
- [ ] Backend deployed and running
- [ ] Frontend deployed and running
- [ ] All integration tests passed
- [ ] API tests passed
- [ ] Post-deployment verification completed
- [ ] Monitoring configured
- [ ] Rollback plan ready
- [ ] Team sign-off obtained

## Deployment Date & Time
- **Date**: _______________
- **Time**: _______________
- **Deployed By**: _______________
- **Verified By**: _______________

## Notes
_Add any deployment notes, issues encountered, or special considerations here:_

---

## Success Criteria Met ✓
- [ ] Zero downtime deployment
- [ ] No data loss
- [ ] All existing features working
- [ ] New feature working as expected
- [ ] Performance within acceptable limits
- [ ] No critical errors in logs

**Deployment Status**: ⬜ Not Started | ⬜ In Progress | ⬜ Completed | ⬜ Rolled Back
