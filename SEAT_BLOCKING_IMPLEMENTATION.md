# Seat Blocking Mechanism Implementation

## Overview
This document describes the implementation of a seat blocking/reservation mechanism that prevents multiple users from booking the same seats simultaneously. When a user selects seats, they are reserved for 5 minutes, giving the user time to complete the booking.

## Features
- **Automatic Seat Reservation**: Seats are automatically reserved when a user selects them
- **5-Minute Grace Period**: Reserved seats are held for exactly 5 minutes
- **Visual Countdown Timer**: Users see a real-time countdown showing remaining reservation time
- **Automatic Cleanup**: Expired reservations are automatically removed every minute
- **Conflict Prevention**: Other users cannot select seats that are already reserved or booked

## Database Changes

### Modified Table: `bookings`
Added two new columns to support seat reservation:

```sql
is_reserved BOOLEAN DEFAULT FALSE
reserved_until TIMESTAMP NULL
```

Also updated the booking_status constraint to include 'reserved':
```sql
CHECK (booking_status IN ('confirmed', 'cancelled', 'pending', 'reserved'))
```

### Updated File
- `database/setup/5_bookings.sql`

## Backend Changes

### 1. Model Updates
**File**: `backend/BusBookingAPI/Models/Booking.cs`

Added properties:
```csharp
public bool IsReserved { get; set; } = false;
public DateTime? ReservedUntil { get; set; }
```

### 2. New DTOs
**File**: `backend/BusBookingAPI/DTOs/ReserveSeatDto.cs`

Created DTOs for seat reservation:
- `ReserveSeatDto`: Request to reserve seats
- `ReserveSeatResponse`: Response with reservation details and expiry time
- `ReleaseReservationDto`: Request to release a reservation

### 3. Service Layer Updates
**File**: `backend/BusBookingAPI/Services/BookingService.cs`

Added new methods to `IBookingService`:
- `ReserveSeatAsync()`: Creates a temporary seat reservation
- `ReleaseReservationAsync()`: Manually releases a reservation
- `CleanupExpiredReservationsAsync()`: Removes expired reservations
- `GetReservedSeatsAsync()`: Gets list of currently reserved seats

Updated `CreateBookingAsync()`:
- Now checks for existing user reservations
- Converts reservations to confirmed bookings automatically
- Validates against both booked AND reserved seats

### 4. Background Service
**File**: `backend/BusBookingAPI/Services/ReservationCleanupService.cs`

Created a background service that:
- Runs every 1 minute
- Automatically cleans up expired reservations
- Ensures database consistency

Registered in `Program.cs`:
```csharp
builder.Services.AddHostedService<ReservationCleanupService>();
```

### 5. Controller Endpoints
**File**: `backend/BusBookingAPI/Controllers/BookingController.cs`

Added new endpoints:

#### POST /api/booking/reserve
Reserve seats temporarily for 5 minutes.

**Request**:
```json
{
  "userId": 1,
  "busId": 5,
  "travelDate": "2026-05-01T00:00:00Z",
  "seatNumbers": "1, 2, 3"
}
```

**Response**:
```json
{
  "reservationId": 123,
  "seatNumbers": "1, 2, 3",
  "reservedUntil": "2026-04-24T10:35:00Z",
  "remainingSeconds": 300
}
```

#### DELETE /api/booking/reserve/{reservationId}
Release a seat reservation manually.

#### POST /api/booking/cleanup-expired
Manually trigger cleanup of expired reservations (also runs automatically).

#### Updated GET /api/booking/bus/{busId}/seats
Now automatically cleans up expired reservations before returning booked seats.

## Frontend Changes

### 1. Service Updates
**File**: `frontend/bus-booking/src/app/services/booking.service.ts`

Added new interfaces and methods:
- `ReserveSeatRequest`: Interface for reservation request
- `ReserveSeatResponse`: Interface for reservation response
- `reserveSeats()`: Method to reserve seats
- `releaseReservation()`: Method to release reservation

### 2. Dashboard Component
**File**: `frontend/bus-booking/src/app/pages/dashboard/dashboard.ts`

Added properties:
```typescript
reservationId: number | null = null;
reservationTimer: any = null;
remainingTime: number = 0;
isReserving: boolean = false;
```

Added methods:
- `reserveSelectedSeats()`: Reserves seats when user selects them
- `releaseReservation()`: Releases reservation when modal closes
- `startReservationTimer()`: Starts countdown timer
- `clearReservationTimer()`: Clears the timer
- `getFormattedRemainingTime()`: Formats time as MM:SS

Updated lifecycle:
- `ngOnDestroy()`: Ensures reservation is released when component is destroyed
- `toggleSeat()`: Automatically reserves seats on selection
- `closeSeatModal()`: Releases reservation when modal closes

### 3. UI Updates
**File**: `frontend/bus-booking/src/app/pages/dashboard/dashboard.html`

Added reservation timer display:
```html
<div *ngIf="reservationId && remainingTime > 0" class="reservation-timer">
  <span class="timer-icon">⏱️</span>
  <span class="timer-text">Seats reserved for:</span>
  <span class="timer-value">{{ getFormattedRemainingTime() }}</span>
</div>
```

### 4. Styling
**File**: `frontend/bus-booking/src/app/pages/dashboard/dashboard.css`

Added styles for:
- `.reservation-timer`: Container with gradient background
- `.timer-value`: Countdown display
- `.timer-warning`: Red background when < 60 seconds remain
- Animations: `pulse` and `blink` for visual feedback

## How It Works

### User Flow
1. **User selects seats**: 
   - Frontend calls `reserveSeats()` API
   - Backend creates a reservation entry with `is_reserved=true` and `reserved_until=now+5min`
   - Frontend starts countdown timer

2. **During reservation period**:
   - Timer counts down from 5:00 to 0:00
   - Other users see these seats as unavailable
   - User can modify selection (releases old reservation, creates new one)

3. **User confirms booking**:
   - Frontend calls `createBooking()` API
   - Backend finds user's reservation and converts it to confirmed booking
   - Reservation is removed, booking is created

4. **User closes modal or time expires**:
   - Frontend calls `releaseReservation()` API
   - Backend deletes the reservation entry
   - Seats become available again

5. **Automatic cleanup**:
   - Background service runs every minute
   - Removes any reservations where `reserved_until < now`

### Conflict Prevention
- When checking seat availability, the system checks for:
  - Confirmed bookings (`booking_status = 'confirmed'`)
  - Active reservations (`booking_status = 'reserved' AND reserved_until > now`)
- Users cannot select seats that are booked or reserved by others
- Users can only have one active reservation at a time per bus/date

## Testing the Implementation

### 1. Database Setup
Run the updated SQL script:
```bash
psql -U postgres -d busBooking -f database/setup/5_bookings.sql
```

### 2. Backend Testing
Start the backend:
```bash
cd backend/BusBookingAPI
dotnet run
```

Test endpoints using Swagger at `http://localhost:5266`

### 3. Frontend Testing
Start the frontend:
```bash
cd frontend/bus-booking
npm start
```

Test scenarios:
1. Select seats and watch the countdown timer
2. Close modal and verify seats are released
3. Let timer expire and verify seats are released
4. Complete booking and verify reservation converts to booking
5. Open two browser windows and verify one user can't select another's reserved seats

## Configuration

### Reservation Duration
To change the 5-minute duration, update:

**Backend** (`BookingService.cs`):
```csharp
var reservedUntil = DateTime.UtcNow.AddMinutes(5); // Change 5 to desired minutes
```

**Frontend** (`booking.service.ts`):
```typescript
remainingSeconds: 300 // Change 300 to desired seconds
```

### Cleanup Interval
To change how often expired reservations are cleaned up:

**Backend** (`ReservationCleanupService.cs`):
```csharp
private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1); // Change interval
```

## API Documentation

### Reserve Seats
```
POST /api/booking/reserve
Content-Type: application/json

{
  "userId": 1,
  "busId": 5,
  "travelDate": "2026-05-01",
  "seatNumbers": "1, 2, 3"
}

Response: 200 OK
{
  "reservationId": 123,
  "seatNumbers": "1, 2, 3",
  "reservedUntil": "2026-04-24T10:35:00Z",
  "remainingSeconds": 300
}
```

### Release Reservation
```
DELETE /api/booking/reserve/123

Response: 200 OK
{
  "message": "Reservation released successfully"
}
```

### Create Booking (with reservation)
```
POST /api/booking
Content-Type: application/json

{
  "userId": 1,
  "busId": 5,
  "travelDate": "2026-05-01",
  "seatNumbers": "1, 2, 3",
  "totalFare": 450,
  "pickupLocationId": 10,
  "dropLocationId": 20,
  "scheduleId": 5
}

Response: 201 Created
// If user has active reservation, it's converted to booking
// Otherwise, creates new booking after validation
```

## Error Handling

### Common Errors
1. **Seats already reserved**: Returns 400 with message "Some of the requested seats are already booked or reserved"
2. **Reservation expired**: Frontend shows error and clears selection
3. **User not logged in**: Returns 400 with message "User not logged in"
4. **Invalid date**: Returns 400 with message "Travel date must be in the future"

## Performance Considerations

1. **Database Indexes**: Added index on `(is_reserved, reserved_until)` for fast cleanup queries
2. **Background Service**: Runs every minute to avoid database bloat
3. **Automatic Cleanup**: Called before seat availability checks to ensure data freshness
4. **Efficient Queries**: Uses indexed columns and date comparisons

## Security Considerations

1. **User Validation**: All endpoints validate user existence
2. **Ownership Check**: Users can only release their own reservations
3. **Expiry Enforcement**: Backend always validates `reserved_until` timestamp
4. **Conflict Prevention**: Database-level checks prevent double-booking

## Future Enhancements

Possible improvements:
1. Add WebSocket support for real-time seat availability updates
2. Implement reservation extension (add 2 more minutes)
3. Add analytics for reservation conversion rates
4. Implement priority queuing for high-demand routes
5. Add email/SMS notifications when reservation is about to expire
