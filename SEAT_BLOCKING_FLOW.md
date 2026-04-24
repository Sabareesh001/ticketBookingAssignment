# Seat Blocking Flow Diagram

## User Journey Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                     USER SELECTS SEATS                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Frontend: toggleSeat() called                                  │
│  - Add/remove seat from selectedSeats[]                         │
│  - If seats selected and no reservation → reserveSelectedSeats()│
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  API Call: POST /api/booking/reserve                            │
│  Request: { userId, busId, travelDate, seatNumbers }            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Backend: ReserveSeatAsync()                                    │
│  1. Validate user and bus exist                                 │
│  2. Clean up expired reservations                               │
│  3. Check if seats already booked/reserved                      │
│  4. Create reservation entry:                                   │
│     - is_reserved = true                                        │
│     - reserved_until = now + 5 minutes                          │
│     - booking_status = 'reserved'                               │
│  5. Save to database                                            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Response: { reservationId, reservedUntil, remainingSeconds }   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Frontend: startReservationTimer()                              │
│  - Set remainingTime = 300 seconds                              │
│  - Start setInterval (1 second)                                 │
│  - Display countdown timer in UI                                │
│  - Update every second                                          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
                    ┌─────────┴─────────┐
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │  USER CONFIRMS    │  │  USER CLOSES OR   │
        │  BOOKING          │  │  TIMER EXPIRES    │
        └───────────────────┘  └───────────────────┘
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │ confirmBooking()  │  │ releaseReservation│
        │                   │  │ OR timer hits 0   │
        └───────────────────┘  └───────────────────┘
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │ POST /api/booking │  │ DELETE /api/      │
        │                   │  │ booking/reserve/  │
        │                   │  │ {reservationId}   │
        └───────────────────┘  └───────────────────┘
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │ CreateBookingAsync│  │ ReleaseReservation│
        │ - Find user's     │  │ Async()           │
        │   reservation     │  │ - Delete          │
        │ - Convert to      │  │   reservation     │
        │   confirmed       │  │   entry           │
        │ - Update fare,    │  │ - Seats become    │
        │   locations       │  │   available       │
        └───────────────────┘  └───────────────────┘
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │ BOOKING CONFIRMED │  │ SEATS RELEASED    │
        │ ✓ Seats booked    │  │ ✓ Available again │
        │ ✓ Reservation     │  │ ✓ Other users can │
        │   removed         │  │   select them     │
        └───────────────────┘  └───────────────────┘
```

## Background Cleanup Process

```
┌─────────────────────────────────────────────────────────────────┐
│  ReservationCleanupService (Background Service)                 │
│  Runs every 1 minute                                            │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  CleanupExpiredReservationsAsync()                              │
│  1. Query database for expired reservations:                    │
│     WHERE is_reserved = true                                    │
│     AND booking_status = 'reserved'                             │
│     AND reserved_until <= NOW()                                 │
│  2. Delete all expired reservations                             │
│  3. Log count of cleaned reservations                           │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  Database: Expired reservations removed                         │
│  Seats become available for other users                         │
└─────────────────────────────────────────────────────────────────┘
```

## Database State Transitions

```
┌──────────────┐
│   NO ENTRY   │  (Seat available)
└──────────────┘
       ↓ User selects seat
┌──────────────────────────────────────┐
│  RESERVATION ENTRY                   │
│  - is_reserved = true                │
│  - booking_status = 'reserved'       │
│  - reserved_until = now + 5 min      │
└──────────────────────────────────────┘
       ↓                    ↓
       ↓ User confirms      ↓ Timer expires OR user closes
       ↓                    ↓
┌──────────────────┐  ┌──────────────┐
│  BOOKING ENTRY   │  │  ENTRY       │
│  - is_reserved   │  │  DELETED     │
│    = false       │  │              │
│  - booking_status│  │  (Seat       │
│    = 'confirmed' │  │  available   │
│  - reserved_until│  │  again)      │
│    = null        │  │              │
└──────────────────┘  └──────────────┘
```

## Concurrent User Scenario

```
TIME: 10:00:00
┌─────────────┐                    ┌─────────────┐
│   USER A    │                    │   USER B    │
└─────────────┘                    └─────────────┘
       │                                  │
       │ Selects seats 1,2,3              │
       │ ──────────────────────→          │
       │                                  │
       │ Reservation created              │
       │ Timer: 5:00                      │
       │                                  │
       │                                  │ Tries to select seat 2
       │                                  │ ──────────────────────→
       │                                  │
       │                                  │ ✗ Seat 2 shown as booked
       │                                  │ (Cannot select)
       │                                  │
TIME: 10:02:30                            │
       │ Confirms booking                 │
       │ ──────────────────────→          │
       │                                  │
       │ Booking confirmed                │
       │ Reservation → Booking            │
       │                                  │
       │                                  │ Seat 2 still unavailable
       │                                  │ (Now confirmed booking)
       │                                  │

ALTERNATIVE SCENARIO:
TIME: 10:05:00
       │ Timer expires                    │
       │ Reservation deleted              │
       │ ──────────────────────→          │
       │                                  │
       │                                  │ Seat 2 now available!
       │                                  │ Can select ✓
       │                                  │ ──────────────────────→
```

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Dashboard Component                                     │   │
│  │  - Seat selection UI                                     │   │
│  │  - Countdown timer display                               │   │
│  │  - Reservation state management                          │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              ↕                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Booking Service                                         │   │
│  │  - reserveSeats()                                        │   │
│  │  - releaseReservation()                                  │   │
│  │  - createBooking()                                       │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              ↕ HTTP/REST
┌─────────────────────────────────────────────────────────────────┐
│                         BACKEND                                 │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  BookingController                                       │   │
│  │  - POST /api/booking/reserve                             │   │
│  │  - DELETE /api/booking/reserve/{id}                      │   │
│  │  - POST /api/booking                                     │   │
│  │  - GET /api/booking/bus/{id}/seats                       │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              ↕                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  BookingService                                          │   │
│  │  - ReserveSeatAsync()                                    │   │
│  │  - ReleaseReservationAsync()                             │   │
│  │  - CreateBookingAsync()                                  │   │
│  │  - CleanupExpiredReservationsAsync()                     │   │
│  │  - GetReservedSeatsAsync()                               │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              ↕                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  ReservationCleanupService (Background)                  │   │
│  │  - Runs every 1 minute                                   │   │
│  │  - Calls CleanupExpiredReservationsAsync()               │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              ↕ Entity Framework
┌─────────────────────────────────────────────────────────────────┐
│                       DATABASE                                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  bookings table                                          │   │
│  │  - id (PK)                                               │   │
│  │  - user_id                                               │   │
│  │  - bus_id                                                │   │
│  │  - seat_numbers                                          │   │
│  │  - booking_status ('confirmed', 'reserved', ...)         │   │
│  │  - is_reserved (NEW)                                     │   │
│  │  - reserved_until (NEW)                                  │   │
│  │  - ...                                                   │   │
│  └──────────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  Indexes                                                 │   │
│  │  - idx_bookings_reserved (is_reserved, reserved_until)   │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Timer States

```
┌─────────────────────────────────────────────────────────────────┐
│  TIMER STATE: ACTIVE (5:00 - 1:01)                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  ⏱️ Seats reserved for: 3:45                             │   │
│  │  Background: Purple gradient                             │   │
│  │  Animation: Gentle pulse                                 │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  TIMER STATE: WARNING (1:00 - 0:01)                            │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  ⏱️ Seats reserved for: 0:45                             │   │
│  │  Background: Red (#ff5252)                               │   │
│  │  Animation: Blinking                                     │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│  TIMER STATE: EXPIRED (0:00)                                   │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  ⚠️ Reservation expired. Please select seats again.      │   │
│  │  - Timer cleared                                         │   │
│  │  - Seats deselected                                      │   │
│  │  - Reservation released                                  │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Error Handling Flow

```
┌─────────────────────────────────────────────────────────────────┐
│  User Action: Select Seat                                       │
└─────────────────────────────────────────────────────────────────┘
                              ↓
                    ┌─────────┴─────────┐
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │  SUCCESS          │  │  ERROR            │
        └───────────────────┘  └───────────────────┘
                    ↓                   ↓
        ┌───────────────────┐  ┌───────────────────┐
        │ - Reservation     │  │ Possible Errors:  │
        │   created         │  │                   │
        │ - Timer starts    │  │ 1. Seats already  │
        │ - UI updates      │  │    reserved       │
        │                   │  │ 2. User not found │
        │                   │  │ 3. Bus not found  │
        │                   │  │ 4. Network error  │
        └───────────────────┘  └───────────────────┘
                                        ↓
                              ┌───────────────────┐
                              │ Error Handling:   │
                              │ - Show error msg  │
                              │ - Clear selection │
                              │ - Log to console  │
                              │ - Allow retry     │
                              └───────────────────┘
```

## Key Timing Diagram

```
T=0:00    User selects seats
          ↓
          Reservation created in DB
          ↓
          Timer starts: 5:00
          ↓
T=1:00    Timer: 4:00 (Normal state)
          ↓
T=2:00    Timer: 3:00 (Normal state)
          ↓
T=3:00    Timer: 2:00 (Normal state)
          ↓
T=4:00    Timer: 1:00 (Warning state - red background)
          ↓
T=4:30    Timer: 0:30 (Warning state - blinking)
          ↓
T=5:00    Timer: 0:00 (EXPIRED)
          ↓
          Reservation deleted from DB
          ↓
          Seats available again
          ↓
          User sees error message
```
