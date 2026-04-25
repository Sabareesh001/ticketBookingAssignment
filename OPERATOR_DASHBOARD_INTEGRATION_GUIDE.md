# Operator Dashboard Integration Guide

## Overview
The operator dashboard has been fully integrated with the new route and location selection features. Operators can now add buses by selecting location names and routes with meaningful display names instead of IDs.

## Frontend Integration Complete ✅

### 1. New Service: `operator-dashboard.service.ts`
Located at: `frontend/bus-booking/src/app/services/operator-dashboard.service.ts`

**Features:**
- `getAllRoutes()` - Fetches all routes with location names
- `getRoutesByLocations(sourceLocationId, destinationLocationId)` - Fetches routes between two specific locations
- `getAllLocations()` - Fetches all locations with display names
- `getLocationsByDistrict(districtId)` - Fetches locations by district

### 2. Updated Component: `operator-dashboard.component.ts`
Located at: `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.ts`

**New Properties:**
- `routes: RouteWithNames[]` - Stores routes with location names
- `availableLocations: LocationWithName[]` - Stores all available locations

**New Methods:**
- `loadRoutes()` - Loads all routes on component initialization
- `loadAvailableLocations()` - Loads all locations on component initialization
- `onSourceLocationChange(event)` - Dynamically loads routes when source location changes
- `onDestinationLocationChange(event)` - Dynamically loads routes when destination location changes

**Workflow:**
1. Component loads all routes and locations on init
2. Operator selects source location from dropdown
3. Operator selects destination location from dropdown
4. Routes are automatically filtered based on selected locations
5. Operator selects a route from the filtered list
6. Bus details are filled in and saved

### 3. Updated Template: `operator-dashboard.component.html`
Located at: `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.html`

**Form Changes:**
- **Source Location**: Dropdown showing "City, Street Address" format
- **Destination Location**: Dropdown showing "City, Street Address" format
- **Route**: Dropdown showing "Source City → Destination City" format

**Bus Card Display:**
- Shows route as "Source City → Destination City"
- Displays distance in km
- Displays estimated duration in hours
- Shows pickup and drop times
- Displays seating capacity and price

## User Experience Flow

### Adding a New Bus

1. **Click "Add New Bus"** button
2. **Enter Registration Number** (e.g., "KA-01-AB-1234")
3. **Select Source Location** from dropdown
   - Shows all available locations with city and address
4. **Select Destination Location** from dropdown
   - Shows all available locations with city and address
5. **Routes Auto-Load** based on selected locations
   - Dropdown shows "Source → Destination" format
6. **Select Route** from the filtered list
7. **Enter Seating Capacity** (e.g., 40)
8. **Enter Price** (e.g., 500)
9. **Check Active** checkbox if bus is active
10. **Click "Save Bus"**

### Viewing Buses

Each bus card displays:
- Registration number and active status
- Route information (Source → Destination)
- Distance and duration
- Seating capacity and price
- Pickup and drop times
- Edit and Delete buttons

### Managing Locations

Operators can create, edit, and delete their own locations:
- Street Address
- City
- District, State, Country
- Postal Code
- Latitude and Longitude (optional)

## API Endpoints Used

### Routes
- `GET /api/operator-dashboard/routes` - Get all routes
- `GET /api/operator-dashboard/routes/{sourceLocationId}/{destinationLocationId}` - Get routes between two locations

### Locations
- `GET /api/operator-dashboard/all-locations` - Get all locations
- `GET /api/operator-dashboard/locations-by-district/{districtId}` - Get locations by district

### Buses
- `GET /api/operator-dashboard/buses` - Get operator's buses
- `POST /api/operator-dashboard/buses` - Create new bus
- `PUT /api/operator-dashboard/buses/{id}` - Update bus
- `DELETE /api/operator-dashboard/buses/{id}` - Delete bus

## Key Features

✅ **Dynamic Route Loading** - Routes are filtered based on selected locations
✅ **Meaningful Names** - All dropdowns show location and route names, not IDs
✅ **Auto-Selection** - First matching route is auto-selected when locations are chosen
✅ **Validation** - Prevents selecting same location as both source and destination
✅ **Rich Bus Display** - Bus cards show all relevant information
✅ **Location Management** - Operators can manage their own locations
✅ **Error Handling** - Proper error messages for failed operations
✅ **Loading States** - Visual feedback during data loading

## Testing Checklist

- [ ] Load operator dashboard
- [ ] Verify all routes and locations load on init
- [ ] Add new bus with location selection
- [ ] Verify routes filter when locations are selected
- [ ] Verify route auto-selection works
- [ ] Edit existing bus
- [ ] Delete bus
- [ ] Add new location
- [ ] Edit location
- [ ] Delete location
- [ ] Verify bus cards display location names instead of IDs
- [ ] Test with different location combinations
- [ ] Verify error messages appear on failures

## Troubleshooting

### Routes not loading
- Check that locations exist in the database
- Verify routes are created between those locations
- Check browser console for API errors

### Locations not appearing in dropdown
- Ensure locations are created in the database
- Check that locations have valid city and street address
- Verify API endpoint returns data

### Route not auto-selecting
- Ensure at least one route exists between selected locations
- Check that route IDs are valid
- Verify form control is properly bound

## Performance Notes

- Routes are loaded once on component init
- Routes are re-fetched only when location selection changes
- Locations are loaded once on component init
- All API calls use proper error handling
- Change detection is triggered only when needed

## Future Enhancements

- Add route creation UI for operators
- Add bulk bus import functionality
- Add bus schedule management
- Add availability calendar view
- Add analytics dashboard
