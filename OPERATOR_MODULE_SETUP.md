# Bus Operator Module - Quick Setup Guide

## What Was Implemented

### Backend
✅ Operator Authentication Service with JWT
✅ Operator Auth Controller (login/signup)
✅ Operator Dashboard Controller (bus & location management)
✅ Extended Operator Service with operator-specific queries
✅ Password hashing with SHA256
✅ Authorization checks for resource ownership

### Frontend
✅ Operator Auth Service
✅ Operator Login Component
✅ Operator Signup Component
✅ Operator Dashboard Component (tabbed interface)
✅ Home/Landing Page
✅ Operator Auth Guard
✅ Updated Routes

## Files Created

### Backend Files
```
backend/BusBookingAPI/DTOs/OperatorAuthRequest.cs
backend/BusBookingAPI/Services/OperatorAuthService.cs
backend/BusBookingAPI/Controllers/OperatorAuthController.cs
backend/BusBookingAPI/Controllers/OperatorDashboardController.cs
```

### Frontend Files
```
frontend/bus-booking/src/app/models/operator-auth.model.ts
frontend/bus-booking/src/app/services/operator-auth.service.ts
frontend/bus-booking/src/app/pages/operator-login/operator-login.component.ts
frontend/bus-booking/src/app/pages/operator-login/operator-login.component.html
frontend/bus-booking/src/app/pages/operator-login/operator-login.component.css
frontend/bus-booking/src/app/pages/operator-signup/operator-signup.component.ts
frontend/bus-booking/src/app/pages/operator-signup/operator-signup.component.html
frontend/bus-booking/src/app/pages/operator-signup/operator-signup.component.css
frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.ts
frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.html
frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.css
frontend/bus-booking/src/app/pages/home/home.component.ts
frontend/bus-booking/src/app/pages/home/home.component.html
frontend/bus-booking/src/app/pages/home/home.component.css
frontend/bus-booking/src/app/guards/operator-auth.guard.ts
```

### Modified Files
```
backend/BusBookingAPI/Program.cs (added OperatorAuthService registration)
backend/BusBookingAPI/Services/OperatorService.cs (added operator-specific methods)
frontend/bus-booking/src/app/app.routes.ts (added operator routes)
```

## How to Use

### 1. Backend Setup

The backend is already configured. Just ensure:
- Database is running
- `Program.cs` has the new service registered (already done)
- API is running on `http://localhost:5266`

### 2. Frontend Setup

The frontend is ready to use. Just ensure:
- Angular app is running
- API URL is correct in services

### 3. Testing the Module

#### Operator Registration
1. Navigate to `http://localhost:4200/home`
2. Click "Bus Operator" → "Register"
3. Fill in the form:
   - Operator Name: "ABC Travels"
   - Email: "operator@example.com"
   - Phone: "9876543210"
   - License Number: "LIC123456"
   - Address: "123 Main Street, City"
   - Password: "password123"
4. Click "Register"
5. Should redirect to operator dashboard

#### Operator Login
1. Navigate to `http://localhost:4200/operator-login`
2. Enter email and password
3. Click "Login"
4. Should redirect to operator dashboard

#### Bus Management
1. In operator dashboard, go to "My Buses" tab
2. Click "Add New Bus"
3. Fill in bus details:
   - Registration Number: "KA-01-AB-1234"
   - Route ID: 1 (must exist in database)
   - Source Location ID: 1 (must exist)
   - Destination Location ID: 2 (must exist)
   - Seating Capacity: 50
   - Price: 500
4. Click "Save Bus"
5. Bus appears in the list

#### Location Management
1. In operator dashboard, go to "My Locations" tab
2. Click "Add New Location"
3. Fill in location details:
   - Street Address: "456 Bus Station Road"
   - City: "Bangalore"
   - District ID: 1 (must exist)
   - State ID: 1 (must exist)
   - Country ID: 1 (must exist)
   - Postal Code: "560001"
4. Click "Save Location"
5. Location appears in the list

## Key Features

### Registration
- Email and license number uniqueness validation
- Password confirmation
- Secure password hashing

### Authentication
- JWT token-based authentication
- Separate token storage from user auth
- Automatic token inclusion in API requests

### Bus Management
- Create, read, update, delete buses
- Only operators can manage their own buses
- Validation of route and location IDs

### Location Management
- Create, read, update, delete locations
- Locations automatically linked to operator
- Hierarchical location selection (Country → State → District)
- Optional GPS coordinates

### Dashboard
- Tabbed interface for buses and locations
- Real-time list updates
- Edit and delete functionality
- Form validation with error messages
- Success/error notifications

## API Endpoints

### Authentication
```
POST /api/operator-auth/login
{
  "email": "operator@example.com",
  "password": "password123"
}

POST /api/operator-auth/signup
{
  "operatorName": "ABC Travels",
  "email": "operator@example.com",
  "phoneNumber": "9876543210",
  "licenseNumber": "LIC123456",
  "address": "123 Main Street",
  "password": "password123"
}
```

### Bus Management
```
GET /api/operator-dashboard/buses
POST /api/operator-dashboard/buses
PUT /api/operator-dashboard/buses/{id}
DELETE /api/operator-dashboard/buses/{id}
```

### Location Management
```
GET /api/operator-dashboard/locations
POST /api/operator-dashboard/locations
PUT /api/operator-dashboard/locations/{id}
DELETE /api/operator-dashboard/locations/{id}
```

## Security Notes

1. **Token Storage**: Tokens are stored in localStorage with separate keys for operators
2. **Authorization**: All dashboard endpoints check operator ID from token
3. **Password**: Hashed with SHA256 before storage
4. **CORS**: Ensure backend CORS policy allows frontend origin

## Troubleshooting

### Issue: "Cannot find module" errors
**Solution**: Ensure all files are created in correct paths

### Issue: 401 Unauthorized errors
**Solution**: 
- Check token is stored in localStorage
- Verify token hasn't expired
- Check Authorization header is being sent

### Issue: 403 Forbidden errors
**Solution**: 
- Verify you're trying to access your own resources
- Check operator ID in token matches resource owner

### Issue: Bus/Location creation fails
**Solution**:
- Verify route/location IDs exist in database
- Check all required fields are provided
- Verify you're authenticated

## Next Steps

1. **Test the complete flow** - Register, login, create buses/locations
2. **Verify database** - Check BusOperator, Bus, and Location tables
3. **Check logs** - Review backend logs for any errors
4. **Test authorization** - Try accessing other operator's resources (should fail)

## Database Verification

To verify the implementation is working:

```sql
-- Check operator was created
SELECT * FROM bus_operators WHERE email = 'operator@example.com';

-- Check operator's buses
SELECT * FROM buses WHERE operator_id = 1;

-- Check operator's locations
SELECT * FROM locations WHERE operator_id = 1;
```

## Support

Refer to `BUS_OPERATOR_MODULE_IMPLEMENTATION.md` for detailed documentation.
