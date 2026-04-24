# Visual Diagram: Automatic Cleanup Process

## Timeline: What Happens When Client Loses Power

```
TIME: 10:00:00 AM
┌─────────────────────────────────────────────────────────────┐
│  USER'S COMPUTER                                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Browser: Selects seats 1, 2, 3                      │   │
│  │  Sends: POST /api/booking/reserve                    │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                        ↓ HTTP Request
┌─────────────────────────────────────────────────────────────┐
│  SERVER                                                     │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  API: Creates reservation                            │   │
│  │  Database INSERT:                                    │   │
│  │    id: 123                                           │   │
│  │    seat_numbers: "1, 2, 3"                           │   │
│  │    is_reserved: true                                 │   │
│  │    reserved_until: 10:05:00 AM  ← 5 min from now    │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:01:00 AM
┌─────────────────────────────────────────────────────────────┐
│  SERVER (Background Service - Runs Every Minute)            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Cleanup Check #1:                                   │   │
│  │  Query: WHERE reserved_until <= 10:01:00             │   │
│  │  Found: 0 expired (10:05:00 > 10:01:00)             │   │
│  │  Action: None                                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:02:00 AM
┌─────────────────────────────────────────────────────────────┐
│  USER'S COMPUTER                                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  ⚡ POWER CUT! Computer shuts down                   │   │
│  │  ❌ Browser closed                                    │   │
│  │  ❌ No internet connection                            │   │
│  │  ❌ Cannot send release request                       │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                        ↓ No connection
┌─────────────────────────────────────────────────────────────┐
│  SERVER (Still Running)                                     │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Database still has reservation:                     │   │
│  │    id: 123                                           │   │
│  │    seat_numbers: "1, 2, 3"                           │   │
│  │    is_reserved: true                                 │   │
│  │    reserved_until: 10:05:00 AM                       │   │
│  │                                                      │   │
│  │  ✓ Reservation still exists (client offline)        │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Cleanup Check #2:                                   │   │
│  │  Query: WHERE reserved_until <= 10:02:00             │   │
│  │  Found: 0 expired (10:05:00 > 10:02:00)             │   │
│  │  Action: None                                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:03:00 AM
┌─────────────────────────────────────────────────────────────┐
│  USER'S COMPUTER                                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  ❌ Still offline                                     │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  SERVER (Background Service Continues)                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Cleanup Check #3:                                   │   │
│  │  Query: WHERE reserved_until <= 10:03:00             │   │
│  │  Found: 0 expired (10:05:00 > 10:03:00)             │   │
│  │  Action: None                                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:04:00 AM
┌─────────────────────────────────────────────────────────────┐
│  USER'S COMPUTER                                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  ❌ Still offline                                     │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  SERVER (Background Service Continues)                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Cleanup Check #4:                                   │   │
│  │  Query: WHERE reserved_until <= 10:04:00             │   │
│  │  Found: 0 expired (10:05:00 > 10:04:00)             │   │
│  │  Action: None                                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:05:00 AM
┌─────────────────────────────────────────────────────────────┐
│  USER'S COMPUTER                                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  ❌ Still offline                                     │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  SERVER (Background Service Continues)                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Cleanup Check #5:                                   │   │
│  │  Query: WHERE reserved_until <= 10:05:00             │   │
│  │  Found: 1 expired! (10:05:00 <= 10:05:00)           │   │
│  │                                                      │   │
│  │  🗑️  DELETE FROM bookings WHERE id = 123            │   │
│  │                                                      │   │
│  │  ✅ Reservation DELETED!                             │   │
│  │  ✅ Seats 1, 2, 3 now AVAILABLE                      │   │
│  │                                                      │   │
│  │  Log: "Cleaned up 1 expired reservations"           │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘

═══════════════════════════════════════════════════════════════

TIME: 10:06:00 AM
┌─────────────────────────────────────────────────────────────┐
│  ANOTHER USER'S COMPUTER                                    │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Opens seat selection for same bus                   │   │
│  │  Sends: GET /api/booking/bus/1/seats                 │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                        ↓ HTTP Request
┌─────────────────────────────────────────────────────────────┐
│  SERVER                                                     │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  API: Returns available seats                        │   │
│  │  Response:                                           │   │
│  │    confirmedBookings: []                             │   │
│  │    reservedSeats: []                                 │   │
│  │                                                      │   │
│  │  ✅ Seats 1, 2, 3 shown as AVAILABLE                 │   │
│  │  ✅ New user can select them!                        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Key Points

### 1. Client Independence
```
┌──────────────┐         ┌──────────────┐
│   CLIENT     │         │   SERVER     │
│              │         │              │
│  Can crash   │    ✗    │  Always runs │
│  Can lose    │    ✗    │  Independent │
│  power       │    ✗    │  Reliable    │
│  Can go      │    ✗    │  Automatic   │
│  offline     │    ✗    │  cleanup     │
└──────────────┘         └──────────────┘
```

### 2. Database-Driven Expiry
```
┌─────────────────────────────────────────────────────────┐
│  Database Record                                        │
├─────────────────────────────────────────────────────────┤
│  reserved_until: 2026-04-24 10:05:00 UTC               │
│                                                         │
│  Server checks: NOW() >= reserved_until ?               │
│                                                         │
│  If YES → DELETE                                        │
│  If NO  → Keep                                          │
└─────────────────────────────────────────────────────────┘
```

### 3. Multiple Safety Nets
```
Cleanup Trigger #1: Background Service (every 1 min)
        ↓
Cleanup Trigger #2: Before seat availability check
        ↓
Cleanup Trigger #3: Before creating new reservation
        ↓
Cleanup Trigger #4: Before creating booking
        ↓
Cleanup Trigger #5: Manual API endpoint
```

## Comparison: With vs Without Background Service

### ❌ Without Background Service (Bad)
```
User reserves → Power cut → Reservation stuck forever
                           → Seats permanently unavailable
                           → System broken
```

### ✅ With Background Service (Good)
```
User reserves → Power cut → Background service deletes after 5 min
                           → Seats available again
                           → System works perfectly
```

## Real-World Scenarios

### Scenario 1: Normal User
```
10:00 - Reserve seats
10:02 - Select payment method
10:04 - Complete payment
10:04 - Reservation → Booking ✓
Result: Seats booked successfully
```

### Scenario 2: Indecisive User
```
10:00 - Reserve seats
10:01 - Browse other buses
10:02 - Check reviews
10:03 - Compare prices
10:04 - Still deciding...
10:05 - Timer expires
10:05 - Reservation deleted automatically ✓
Result: Seats available for others
```

### Scenario 3: Power Cut
```
10:00 - Reserve seats
10:02 - ⚡ Power cut
10:02 - Computer off
10:03 - (Background service running)
10:04 - (Background service running)
10:05 - Background service deletes ✓
Result: Seats available for others
```

### Scenario 4: Network Issue
```
10:00 - Reserve seats
10:01 - Internet disconnects
10:02 - Cannot complete booking
10:03 - Internet still down
10:04 - Internet still down
10:05 - Background service deletes ✓
Result: Seats available for others
```

### Scenario 5: Browser Crash
```
10:00 - Reserve seats
10:01 - Browser crashes
10:02 - User restarts computer
10:03 - User opens different app
10:04 - User forgets about booking
10:05 - Background service deletes ✓
Result: Seats available for others
```

## Summary

**The system is bulletproof because:**

1. ✅ Cleanup runs on **server** (not client)
2. ✅ Runs **automatically** every minute
3. ✅ Uses **database timestamp** (accurate)
4. ✅ Has **5 different triggers** (redundancy)
5. ✅ Handles **all failure scenarios**
6. ✅ Requires **zero user action**
7. ✅ Is **production-ready**

**No matter what happens to the client, the server will clean up expired reservations!**
