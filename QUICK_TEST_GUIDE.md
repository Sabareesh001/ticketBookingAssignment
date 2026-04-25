# Quick Test Guide - Bus Operator Dashboard

## Status: ✅ READY TO TEST
All compilation errors resolved. Backend and frontend compile without errors.

---

## Quick Start

### 1. Start Backend
```bash
cd backend/BusBookingAPI
dotnet run
```
Backend will run on: `http://localhost:5266`

### 2. Start Frontend
```bash
cd frontend/bus-booking
npm start
```
Frontend will run on: `http://localhost:4200`

---

## Test Scenario: Complete Workflow

### Step 1: Operator Login
1. Navigate to `http://localhost:4200/operator-login`
2. Enter operator credentials
3. Click "Login"
4. Should redirect to operator dashboard

### Step 2: Create a Location
1. Click "My Locations" tab
2. Click "Add New Location" button
3. Fill in location details:
   - Street Address: "123 Main Street"
   - City: "Mumbai"
   - District ID: 1
   - State ID: 1
   - Country ID: 1
   - Postal Code: "400001"
4. Click "Save Location"
5. Verify success message appears
6. Verify location appears in list

### Step 3: Create Another Location
1. Repeat Step 2 with different city (e.g., "Delhi")
2. Verify both locations appear in list

### Step 4: Create a Bus
1. Click "My Buses" tab
2. Click "Add New Bus" button
3. Fill in bus details:
   - Registration Number: "MH-01-AB-1234"
   - Source Location: Select "Mumbai" (first location)
   - **Verify routes filter dynamically** ← KEY TEST
   - Destination Location: Select "Delhi" (second location)
   - **Verify routes filter again** ← KEY TEST
   - Route: Should auto-populate with filtered route
   - Seating Capacity: 50
   - Price: 1500
4. Click "Save Bus"
5. Verify success message appears

### Step 5: Verify Bus Display
1. Bus should appear in list with:
   - Registration Number: "MH-01-AB-1234"
   - **Route: "Mumbai → Delhi"** (location names, not IDs) ← KEY TEST
   - Seating Capacity: 50
   - Price: ₹1500

### Step 6: Edit Bus
1. Click "Edit" button on bus card
2. Modify seating capacity to 60
3. Click "Save Bus"
4. Verify success message
5. Verify bus updated in list

### Step 7: Delete Bus
1. Click "Delete" button on bus card
2. Confirm deletion
3. Verify success message
4. Verify bus removed from list

### Step 8: Logout
1. Click "Logout" button
2. Should redirect to login page

---

## Key Features to Verify

### ✅ Dynamic Route Filtering
- [ ] When source location changes, routes filter
- [ ] When destination location changes, routes filter
- [ ] First matching route auto-selects
- [ ] Routes show location names (e.g., "Mumbai → Delhi")

### ✅ Location Display
- [ ] Bus cards show location names, not IDs
- [ ] Location dropdowns show city names
- [ ] Location list displays all created locations

### ✅ Form Validation
- [ ] Required fields show error messages when empty
- [ ] Success messages appear after save
- [ ] Error messages appear on failure
- [ ] Loading state shows during operations

### ✅ CRUD Operations
- [ ] Create bus works
- [ ] Create location works
- [ ] Edit bus works
- [ ] Edit location works
- [ ] Delete bus works (with confirmation)
- [ ] Delete location works (with confirmation)

### ✅ UI/UX
- [ ] Tab switching works (Buses ↔ Locations)
- [ ] Forms toggle open/close
- [ ] Messages clear when switching tabs
- [ ] Logout redirects to login

---

## Expected API Calls

### When Dashboard Loads
```
GET /api/operator-dashboard/buses
GET /api/operator-dashboard/locations
GET /api/operator-dashboard/routes
GET /api/operator-dashboard/all-locations
```

### When Source Location Changes
```
GET /api/operator-dashboard/routes/{sourceLocationId}/{destinationLocationId}
```

### When Creating Bus
```
POST /api/operator-dashboard/buses
```

### When Updating Bus
```
PUT /api/operator-dashboard/buses/{id}
```

### When Deleting Bus
```
DELETE /api/operator-dashboard/buses/{id}
```

---

## Troubleshooting

### Issue: "Failed to load buses"
- Verify backend is running on `http://localhost:5266`
- Check browser console for error details
- Verify operator is logged in (token exists)

### Issue: Routes not filtering
- Verify both source and destination locations are selected
- Check browser console for API errors
- Verify locations have different IDs

### Issue: Bus card shows "N/A" for location names
- Verify backend is returning `sourceCity` and `destinationCity`
- Check API response in browser Network tab
- Verify BusDto includes location properties

### Issue: Form validation errors
- Verify all required fields are filled
- Check field values match expected types
- Verify no special characters in text fields

---

## Files Modified

### Backend
- `Controllers/OperatorDashboardController.cs`
- `Services/OperatorService.cs`
- `Services/RouteService.cs`
- `Services/LocationService.cs`
- `DTOs/BusDto.cs`
- `DTOs/RouteDto.cs`
- `DTOs/LocationDto.cs`

### Frontend
- `pages/operator-dashboard/operator-dashboard.component.ts`
- `pages/operator-dashboard/operator-dashboard.component.html`
- `services/operator-dashboard.service.ts` (NEW)
- `models/operator-auth.model.ts`

---

## Compilation Status

✅ **All files compile without errors**
- No TypeScript errors
- No Angular template errors
- No backend compilation errors

---

## Next Steps After Testing

1. If all tests pass → Ready for production deployment
2. If issues found → Check error messages and logs
3. Run full test suite: `npm test` (frontend)
4. Run backend tests if available

---

**Last Updated**: 2026-04-24
**Status**: ✅ READY FOR TESTING
