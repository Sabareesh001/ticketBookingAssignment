# Seat Blocking Feature - UI Guide

## Visual Guide to the Seat Reservation Feature

This document describes what users will see when using the seat blocking feature.

## 1. Initial State - Seat Selection Modal

```
┌─────────────────────────────────────────────────────────────┐
│  Select Your Seats                                      ✕   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Operator: Express Travels                                  │
│  Route: Mumbai → Pune                                       │
│  Date: 2026-05-01                                          │
│  Pickup Time: 08:00 AM                                     │
│  Drop Time: 12:00 PM                                       │
│  Price per seat: ₹450                                      │
│                                                             │
│  Legend:                                                    │
│  [  ] Available    [✓] Selected    [X] Booked             │
│                                                             │
│  🚌 Front                                                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  [1] [2]     AISLE     [3] [4]                      │   │
│  │  [5] [6]     AISLE     [7] [8]                      │   │
│  │  [9] [10]    AISLE     [11][12]                     │   │
│  │  [13][14]    AISLE     [15][16]                     │   │
│  │  [X] [X]     AISLE     [17][18]  ← Seats 19,20 booked│  │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Available Seats: 38                                        │
│                                                             │
│  [Cancel]                          [Confirm Booking]        │
└─────────────────────────────────────────────────────────────┘
```

## 2. After Selecting Seats - Timer Appears

```
┌─────────────────────────────────────────────────────────────┐
│  Select Your Seats                                      ✕   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  ⏱️ Seats reserved for: 5:00                        │   │
│  │  (Purple gradient background, gentle pulse)          │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Operator: Express Travels                                  │
│  Route: Mumbai → Pune                                       │
│  Date: 2026-05-01                                          │
│                                                             │
│  🚌 Front                                                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  [✓] [✓]     AISLE     [✓] [4]  ← Seats 1,2,3 selected│ │
│  │  [5] [6]     AISLE     [7] [8]                      │   │
│  │  [9] [10]    AISLE     [11][12]                     │   │
│  │  [13][14]    AISLE     [15][16]                     │   │
│  │  [X] [X]     AISLE     [17][18]                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  Selected Seats: 1, 2, 3                                    │
│  Total Seats: 3                                             │
│  Total Fare: ₹1,350                                        │
│                                                             │
│  [Cancel]                          [Confirm Booking]        │
└─────────────────────────────────────────────────────────────┘
```

## 3. Timer Counting Down (Normal State)

```
┌─────────────────────────────────────────────────────────────┐
│  ⏱️ Seats reserved for: 3:45                               │
│  Background: Purple gradient (#667eea → #764ba2)            │
│  Animation: Gentle pulse (scale 1.0 → 1.02)                │
└─────────────────────────────────────────────────────────────┘

Time remaining: 3 minutes 45 seconds
User has plenty of time to complete booking
```

## 4. Timer Warning State (< 1 minute remaining)

```
┌─────────────────────────────────────────────────────────────┐
│  ⏱️ Seats reserved for: 0:45                               │
│  Background: Red (#ff5252)                                  │
│  Animation: Blinking (opacity 1.0 → 0.7)                   │
└─────────────────────────────────────────────────────────────┘

Time remaining: 45 seconds
Visual warning to user - complete booking soon!
```

## 5. Timer Expired State

```
┌─────────────────────────────────────────────────────────────┐
│  Select Your Seats                                      ✕   │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ⚠️ Reservation expired. Please select seats again.        │
│  (Red error message)                                        │
│                                                             │
│  Operator: Express Travels                                  │
│  Route: Mumbai → Pune                                       │
│                                                             │
│  🚌 Front                                                   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  [1] [2]     AISLE     [3] [4]  ← Seats available again│ │
│  │  [5] [6]     AISLE     [7] [8]                      │   │
│  │  [9] [10]    AISLE     [11][12]                     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  [Cancel]                          [Confirm Booking]        │
└─────────────────────────────────────────────────────────────┘

Seats automatically deselected
User must select seats again to create new reservation
```

## 6. Concurrent User View

### User A's View (Has Reservation)
```
┌─────────────────────────────────────────────────────────────┐
│  ⏱️ Seats reserved for: 4:30                               │
│                                                             │
│  🚌 Front                                                   │
│  │  [✓] [✓]     AISLE     [3] [4]  ← User A selected 1,2  │
│  │  [5] [6]     AISLE     [7] [8]                      │   │
└─────────────────────────────────────────────────────────────┘
```

### User B's View (Sees Reserved Seats as Booked)
```
┌─────────────────────────────────────────────────────────────┐
│  Select Your Seats                                          │
│                                                             │
│  🚌 Front                                                   │
│  │  [X] [X]     AISLE     [3] [4]  ← Seats 1,2 unavailable│ │
│  │  [5] [6]     AISLE     [7] [8]  ← Can select these     │ │
└─────────────────────────────────────────────────────────────┘

User B cannot click on seats 1 and 2
They appear as booked (grayed out)
```

## 7. Successful Booking Confirmation

```
┌─────────────────────────────────────────────────────────────┐
│                    ✅ Booking Confirmed!                    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Booking ID: 12345                                          │
│  Seats: 1, 2, 3                                            │
│  Pickup: 08:00 AM                                          │
│  Drop: 12:00 PM                                            │
│  Total Fare: ₹1,350                                        │
│                                                             │
│  Your seats have been confirmed!                            │
│                                                             │
│                        [OK]                                 │
└─────────────────────────────────────────────────────────────┘

Reservation converted to confirmed booking
Timer removed
Modal closes
```

## Color Scheme

### Seat States
```
Available Seat:
  Background: #f0f0f0 (light gray)
  Border: #ccc
  Cursor: pointer
  Hover: #e0e0e0

Selected Seat:
  Background: #4CAF50 (green)
  Color: white
  Border: #45a049
  Cursor: pointer

Booked Seat:
  Background: #e0e0e0 (gray)
  Color: #999
  Border: #ccc
  Cursor: not-allowed
  Opacity: 0.6
```

### Timer States
```
Normal Timer (5:00 - 1:01):
  Background: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
  Color: white
  Animation: pulse (2s)

Warning Timer (1:00 - 0:01):
  Background: #ff5252 (red)
  Color: white
  Animation: blink (1s)

Expired:
  Display: Error message
  Background: #ffebee (light red)
  Color: #c62828 (dark red)
```

## Responsive Design

### Desktop (> 768px)
```
┌─────────────────────────────────────────────────────────────┐
│  Modal: 800px width, centered                               │
│  Seats: 4 per row (2 + aisle + 2)                          │
│  Timer: Full width, prominent                               │
└─────────────────────────────────────────────────────────────┘
```

### Mobile (< 768px)
```
┌───────────────────────────────┐
│  Modal: 95% width             │
│  Seats: 2 per row (1+aisle+1) │
│  Timer: Full width, stacked   │
│  Buttons: Full width, stacked │
└───────────────────────────────┘
```

## Animations

### Timer Pulse (Normal State)
```css
@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.02); }
}
Duration: 2s
Timing: ease-in-out
Infinite: yes
```

### Timer Blink (Warning State)
```css
@keyframes blink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}
Duration: 1s
Timing: ease-in-out
Infinite: yes
```

### Seat Hover
```css
transition: all 0.3s ease
transform: scale(1.05) on hover
```

## User Interactions

### Click on Available Seat
1. Seat background changes to green
2. Seat number added to "Selected Seats" list
3. Total fare updates
4. If first seat selected → Timer appears (5:00)
5. If not first seat → Timer continues

### Click on Selected Seat (Deselect)
1. Seat background changes back to gray
2. Seat number removed from list
3. Total fare updates
4. If last seat deselected → Reservation released

### Click on Booked Seat
1. No action (cursor: not-allowed)
2. Seat remains grayed out
3. Optional: Show tooltip "This seat is already booked"

### Close Modal
1. Confirmation dialog: "Release your seat reservation?"
2. If Yes → Release reservation, close modal
3. If No → Keep modal open

### Confirm Booking Button
1. Disable button (show "Processing...")
2. Call booking API
3. Show success message
4. Close modal
5. Refresh bus list

## Error States

### Seats Already Reserved
```
┌─────────────────────────────────────────────────────────────┐
│  ⚠️ Some seats are already reserved by another user.       │
│  Please select different seats.                             │
└─────────────────────────────────────────────────────────────┘
```

### Network Error
```
┌─────────────────────────────────────────────────────────────┐
│  ⚠️ Failed to reserve seats. Please check your connection  │
│  and try again.                                             │
└─────────────────────────────────────────────────────────────┘
```

### Booking Failed
```
┌─────────────────────────────────────────────────────────────┐
│  ⚠️ Booking failed. Your reservation has been released.    │
│  Please try again.                                          │
└─────────────────────────────────────────────────────────────┘
```

## Accessibility

### Screen Reader Announcements
- "Seat 1 selected. Timer started. 5 minutes remaining."
- "Seat 2 selected. 3 seats selected. Total fare 1350 rupees."
- "Warning: Only 1 minute remaining to complete booking."
- "Reservation expired. Please select seats again."

### Keyboard Navigation
- Tab: Navigate between seats
- Space/Enter: Select/deselect seat
- Escape: Close modal (with confirmation)
- Arrow keys: Navigate seat grid

### ARIA Labels
```html
<button aria-label="Seat 1, Available" role="button">1</button>
<button aria-label="Seat 2, Selected" role="button">2</button>
<button aria-label="Seat 19, Booked" role="button" disabled>19</button>
<div role="timer" aria-live="polite">5:00</div>
```

## Loading States

### Initial Load
```
┌─────────────────────────────────────────────────────────────┐
│  Loading seat availability...                               │
│  [Spinner animation]                                        │
└─────────────────────────────────────────────────────────────┘
```

### Reserving Seats
```
┌─────────────────────────────────────────────────────────────┐
│  Reserving your seats...                                    │
│  [Spinner animation]                                        │
└─────────────────────────────────────────────────────────────┘
```

### Processing Booking
```
┌─────────────────────────────────────────────────────────────┐
│  [Cancel]                    [Processing... ⏳]             │
│  (Confirm button disabled with spinner)                     │
└─────────────────────────────────────────────────────────────┘
```

---

## Summary

The UI provides:
- ✅ Clear visual feedback for all states
- ✅ Prominent countdown timer
- ✅ Color-coded seat states
- ✅ Smooth animations
- ✅ Responsive design
- ✅ Accessibility support
- ✅ Error handling
- ✅ Loading states

Users will have a smooth, intuitive experience with clear indication of:
- Which seats are available/selected/booked
- How much time they have left
- What actions they can take
- Any errors that occur
