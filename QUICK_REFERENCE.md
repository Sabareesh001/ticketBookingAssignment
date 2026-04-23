# Bus Operator Module - Quick Reference

## URLs

| Page | URL |
|------|-----|
| Home | http://localhost:4200/home |
| Operator Login | http://localhost:4200/operator-login |
| Operator Signup | http://localhost:4200/operator-signup |
| Operator Dashboard | http://localhost:4200/operator-dashboard |

## Test Credentials (After Registration)

```
Email: operator@example.com
Password: password123
License: LIC123456
```

## API Base URL

```
http://localhost:5266/api
```

## Key Files

### Backend
- `backend/BusBookingAPI/Services/OperatorAuthService.cs` - Auth logic
- `backend/BusBookingAPI/Controllers/OperatorAuthController.cs` - Auth endpoints
- `backend/BusBookingAPI/Controllers/OperatorDashboardController.cs` - Dashboard endpoints

### Frontend
- `frontend/bus-booking/src/app/services/operator-auth.service.ts` - Auth service
- `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.ts` - Dashboard

## Database Tables

| Table | Purpose |
|-------|---------|
| bus_operators | Operator accounts |
| buses | Bus details (linked to operator) |
| locations | Locations (linked to operator) |

## Features

### Registration
- Operator name, email, phone, license, address, password
- Email and license uniqueness validation
- Password confirmation

### Authentication
- JWT token (60 min expiration)
- Separate storage from user auth
- Auto-included in API requests

### Bus Management
- Create bus with route and location IDs
- Edit bus details
- Delete bus
- View all operator's buses

### Location Management
- Create location with hierarchical selection
- Edit location details
- Delete location
- View all operator's locations

## Common Tasks

### Register New Operator
1. Go to /operator-signup
2. Fill form with operator details
3. Click Register
4. Redirects to dashboard

### Login as Operator
1. Go to /operator-login
2. Enter email and password
3. Click Login
4. Redirects to dashboard

### Create Bus
1. Go to operator dashboard
2. Click "My Buses" tab
3. Click "Add New Bus"
4. Fill form (registration number, route ID, locations, capacity, price)
5. Click "Save Bus"

### Create Location
1. Go to operator dashboard
2. Click "My Locations" tab
3. Click "Add New Location"
4. Fill form (address, city, district, state, country, postal code)
5. Click "Save Location"

## Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Invalid credentials | Check email/password |
| 403 Forbidden | Accessing other operator's resource | Only access your own resources |
| 400 Bad Request | Invalid form data | Check all required fields |
| 409 Conflict | Email/license already exists | Use unique email/license |

## Token Storage

- **User Token**: `auth_token` in localStorage
- **Operator Token**: `operator_auth_token` in localStorage

## Authorization

All dashboard endpoints check:
1. Valid JWT token
2. Operator ID from token matches resource owner
3. Returns 403 if unauthorized

## Password Requirements

- Minimum 6 characters
- Must match confirmation
- Hashed with SHA256 before storage

## Form Validation

### Operator Signup
- Operator Name: 3+ characters
- Email: Valid email format
- Phone: 10 digits
- License: 5+ characters
- Address: 5+ characters
- Password: 6+ characters

### Bus Creation
- Registration Number: Required
- Route ID: Required (must exist)
- Source Location: Required (must exist)
- Destination Location: Required (must exist)
- Seating Capacity: Required, minimum 1
- Price: Required, minimum 0

### Location Creation
- Street Address: 5+ characters
- City: 2+ characters
- District ID: Required (must exist)
- State ID: Required (must exist)
- Country ID: Required (must exist)
- Postal Code: 3+ characters

## Troubleshooting

### Can't login
- Check email is correct
- Check password is correct
- Verify operator account exists in database

### Can't create bus
- Verify route ID exists
- Verify location IDs exist
- Check all required fields are filled

### Can't create location
- Verify district/state/country IDs exist
- Check all required fields are filled
- Verify postal code format

### 401 errors
- Token may have expired (60 min)
- Try logging in again
- Check localStorage for token

### 403 errors
- You're trying to access another operator's resource
- Only operators can access their own buses/locations

## Database Queries

### Check operator exists
```sql
SELECT * FROM bus_operators WHERE email = 'operator@example.com';
```

### Check operator's buses
```sql
SELECT * FROM buses WHERE operator_id = 1;
```

### Check operator's locations
```sql
SELECT * FROM locations WHERE operator_id = 1;
```

## Performance Tips

1. Buses and locations load on dashboard open
2. Lists update after create/edit/delete
3. Forms validate in real-time
4. Token auto-included in all requests

## Security Notes

1. Never share JWT token
2. Token expires after 60 minutes
3. Password is hashed before storage
4. Each operator can only access their resources
5. Use HTTPS in production

## Support

- See BUS_OPERATOR_MODULE_IMPLEMENTATION.md for detailed docs
- See OPERATOR_MODULE_SETUP.md for setup instructions
- Check backend logs: backend/BusBookingAPI/logs/
- Check browser console for frontend errors
