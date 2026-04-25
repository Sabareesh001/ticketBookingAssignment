# Testing the Operator Dashboard Integration

## Prerequisites
- Backend API running on `http://localhost:5266`
- Frontend running on `http://localhost:4200`
- Operator logged in
- Database populated with locations and routes

## Test Scenarios

### Scenario 1: Add a New Bus

**Steps:**
1. Navigate to operator dashboard
2. Click "Add New Bus" button
3. Enter registration number: "KA-01-AB-1234"
4. Select source location from dropdown (e.g., "Bangalore, Main Street")
5. Select destination location from dropdown (e.g., "Mysore, Highway Road")
6. Verify routes dropdown populates with matching routes
7. Select a route (should show "Bangalore, Main Street → Mysore, Highway Road")
8. Enter seating capacity: 40
9. Enter price: 500
10. Check "Active" checkbox
11. Click "Save Bus"

**Expected Results:**
- ✅ Routes dropdown shows location names, not IDs
- ✅ Routes are filtered based on selected locations
- ✅ First matching route is auto-selected
- ✅ Bus is created successfully
- ✅ Success message appears
- ✅ Bus appears in the list with location names

### Scenario 2: View Bus Details

**Steps:**
1. Look at the bus card in the list
2. Verify all information is displayed

**Expected Results:**
- ✅ Registration number displayed
- ✅ Route shows as "Source City → Destination City"
- ✅ Distance in km displayed
- ✅ Duration in hours displayed
- ✅ Seating capacity displayed
- ✅ Price displayed
- ✅ Pickup and drop times displayed
- ✅ Active/Inactive status shown

### Scenario 3: Edit Bus

**Steps:**
1. Click "Edit" button on a bus card
2. Modify seating capacity to 50
3. Modify price to 600
4. Click "Save Bus"

**Expected Results:**
- ✅ Form populates with current bus data
- ✅ Changes are saved
- ✅ Bus card updates with new values
- ✅ Success message appears

### Scenario 4: Delete Bus

**Steps:**
1. Click "Delete" button on a bus card
2. Confirm deletion in dialog
3. Verify bus is removed from list

**Expected Results:**
- ✅ Confirmation dialog appears
- ✅ Bus is deleted from database
- ✅ Bus is removed from list
- ✅ Success message appears

### Scenario 5: Add Location

**Steps:**
1. Click "My Locations" tab
2. Click "Add New Location" button
3. Enter street address: "123 Main Street"
4. Enter city: "Bangalore"
5. Enter district ID: 1
6. Enter state ID: 1
7. Enter country ID: 1
8. Enter postal code: "560001"
9. Enter latitude: 12.9716
10. Enter longitude: 77.5946
11. Click "Save Location"

**Expected Results:**
- ✅ Location is created
- ✅ Location appears in the list
- ✅ Location is available in bus form dropdowns
- ✅ Success message appears

### Scenario 6: Dynamic Route Filtering

**Steps:**
1. Click "Add New Bus"
2. Select source location: "Bangalore, Main Street"
3. Select destination location: "Mysore, Highway Road"
4. Observe routes dropdown

**Expected Results:**
- ✅ Routes dropdown shows only routes from Bangalore to Mysore
- ✅ Routes display as "Bangalore, Main Street → Mysore, Highway Road"
- ✅ First route is auto-selected
- ✅ Changing either location updates the routes list

### Scenario 7: Validation

**Steps:**
1. Click "Add New Bus"
2. Try to submit form without filling required fields
3. Try to select same location as source and destination

**Expected Results:**
- ✅ Error messages appear for required fields
- ✅ Form doesn't submit with invalid data
- ✅ Cannot select same location as both source and destination

### Scenario 8: Error Handling

**Steps:**
1. Stop the backend API
2. Try to add a bus
3. Observe error message

**Expected Results:**
- ✅ Error message appears
- ✅ User is informed of the failure
- ✅ Form remains open for retry

## API Response Verification

### Routes Response Format
```json
[
  {
    "id": 1,
    "sourceLocationId": 1,
    "destinationLocationId": 2,
    "sourceLocationName": "Bangalore, Main Street",
    "destinationLocationName": "Mysore, Highway Road",
    "distanceKm": 150,
    "estimatedDurationHours": 3,
    "createdAt": "2024-04-24T10:00:00Z",
    "updatedAt": "2024-04-24T10:00:00Z"
  }
]
```

### Locations Response Format
```json
[
  {
    "id": 1,
    "streetAddress": "Main Street",
    "districtId": 1,
    "city": "Bangalore",
    "stateId": 1,
    "countryId": 1,
    "postalCode": "560001",
    "latitude": 12.9716,
    "longitude": 77.5946,
    "operatorId": null,
    "displayName": "Bangalore, Main Street",
    "createdAt": "2024-04-24T10:00:00Z",
    "updatedAt": "2024-04-24T10:00:00Z"
  }
]
```

### Bus Response Format
```json
{
  "id": 1,
  "registrationNumber": "KA-01-AB-1234",
  "operatorId": 1,
  "operatorName": "Operator Name",
  "routeId": 1,
  "sourceLocationId": 1,
  "destinationLocationId": 2,
  "seatingCapacity": 40,
  "price": 500,
  "isActive": true,
  "sourceCity": "Bangalore",
  "destinationCity": "Mysore",
  "distanceKm": 150,
  "estimatedDurationHours": 3,
  "operatingDays": "1,2,3,4,5,6,7",
  "pickupTime": "08:00:00",
  "dropTime": "18:00:00",
  "journeyDurationHours": 10,
  "advanceBookingDays": 90,
  "createdAt": "2024-04-24T10:00:00Z",
  "updatedAt": "2024-04-24T10:00:00Z",
  "schedules": []
}
```

## Browser Console Checks

Open browser DevTools (F12) and check:
- No JavaScript errors
- API calls are successful (200 status)
- Network tab shows proper requests to `/api/operator-dashboard/` endpoints
- Console shows no warnings about missing data

## Performance Checks

- Routes load within 1 second
- Locations load within 1 second
- Route filtering is instant when locations change
- Bus list loads within 2 seconds
- No lag when switching between tabs

## Accessibility Checks

- All form fields have proper labels
- Error messages are clearly visible
- Buttons are properly labeled
- Dropdowns are keyboard accessible
- Tab order is logical

## Cross-Browser Testing

Test on:
- Chrome/Chromium
- Firefox
- Safari
- Edge

Verify:
- Layout is responsive
- All features work correctly
- No console errors
- Styling is consistent
