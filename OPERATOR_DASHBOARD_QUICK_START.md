# Bus Operator Dashboard - Quick Start Guide

## How to Use

### 1. Operator Login
1. Navigate to `/login`
2. Toggle to "Operator Login" tab
3. Enter operator email and password
4. Click "Login"
5. Automatically redirected to `/operator-dashboard`

### 2. View Locations
- Dashboard displays all locations created by the logged-in operator
- Locations shown in paginated table (10 per page)
- Table shows: Street Address, City, District, State, Country, Postal Code, Created Date
- Use Previous/Next buttons to navigate pages

### 3. Create New Location
1. Click "+ Add Location" button
2. Fill in required fields:
   - Street Address (min 5 characters)
   - City (min 2 characters)
   - Country (dropdown)
   - State (dropdown - loads after country selected)
   - District (dropdown - loads after state selected)
   - Postal Code (5-10 digits)
3. Optional fields:
   - Latitude (decimal format, e.g., 40.7128)
   - Longitude (decimal format, e.g., -74.0060)
4. Click "Create" button
5. Success toast appears, table refreshes with new location

### 4. Edit Location
1. Click "Edit" button on any location row
2. Modal opens with pre-filled data
3. Modify any fields
4. Click "Update" button
5. Success toast appears, table refreshes

### 5. Delete Location
1. Click "Delete" button on any location row
2. Confirmation dialog appears
3. Click "OK" to confirm deletion
4. Success toast appears, location removed from table

### 6. Logout
1. Click "Logout" button in top-right corner
2. Redirected to login page

## Form Validation

### Required Fields
- Street Address: 5+ characters
- City: 2+ characters
- Country: Must select
- State: Must select (after country)
- District: Must select (after state)
- Postal Code: 5-10 digits

### Optional Fields
- Latitude: Valid decimal number (e.g., 40.7128)
- Longitude: Valid decimal number (e.g., -74.0060)

### Error Messages
- Form shows inline error messages for invalid fields
- Toast notifications show API errors
- Modal displays error banner for submission failures

## Pagination

- **Default**: 10 locations per page
- **Navigation**: Previous/Next buttons
- **Info**: Shows current page and total pages
- **Disabled**: Buttons disabled at boundaries

## Notifications

### Success (Green Toast)
- "Location created successfully"
- "Location updated successfully"
- "Location deleted successfully"
- Auto-dismisses after 3 seconds

### Error (Red Toast)
- Shows specific error message from API
- Remains visible until user dismisses or navigates
- Examples:
  - "Failed to load locations"
  - "Failed to create location"
  - "Invalid location data"

## Responsive Design

- **Desktop**: Full table with all columns visible
- **Tablet**: Table adapts with smaller padding
- **Mobile**: 
  - Stacked layout
  - Modal takes 95% width
  - Touch-friendly buttons
  - Horizontal scroll for table if needed

## Data Hierarchy

When creating/editing locations, follow this order:

1. **Select Country** → Loads available states
2. **Select State** → Loads available districts
3. **Select District** → Ready to submit

If you change country, state and district selections reset.

## Security Notes

- Your locations are private to your operator account
- You cannot see or modify other operators' locations
- Token expires after 60 minutes (auto-logout)
- All data encrypted in transit (HTTPS)

## Troubleshooting

### "Unauthorized" Error
- Token expired, log in again
- Check browser console for details

### "Location not found"
- Location may have been deleted by another session
- Refresh page to see current state

### Dropdowns not loading
- Check internet connection
- Verify country/state selection
- Try refreshing page

### Form won't submit
- Check all required fields are filled
- Verify postal code is 5-10 digits
- Check latitude/longitude format if provided

## API Endpoints (For Reference)

```
GET    /api/operator-dashboard/locations
POST   /api/operator-dashboard/locations
GET    /api/operator-dashboard/locations/{id}
PUT    /api/operator-dashboard/locations/{id}
DELETE /api/operator-dashboard/locations/{id}
```

All endpoints require Bearer token in Authorization header.
