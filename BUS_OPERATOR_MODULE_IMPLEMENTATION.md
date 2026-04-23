# Bus Operator Module Implementation Guide

## Overview
This document outlines the complete implementation of the Bus Operator Module for the Bus Booking System. The module includes operator registration, authentication, bus management, location management, and a dedicated operator dashboard.

## Architecture

### Backend Components

#### 1. **Operator Authentication Service** (`OperatorAuthService.cs`)
- Handles operator login and signup
- Implements JWT token generation with role `bus_operator`
- Uses SHA256 password hashing for security
- Validates email and license number uniqueness

**Key Methods:**
- `LoginAsync()` - Authenticates operator with email/password
- `SignupAsync()` - Registers new operator
- `GenerateToken()` - Creates JWT token with operator claims

#### 2. **Operator Auth Controller** (`OperatorAuthController.cs`)
- REST endpoints for operator authentication
- Routes:
  - `POST /api/operator-auth/login` - Operator login
  - `POST /api/operator-auth/signup` - Operator registration

#### 3. **Operator Dashboard Controller** (`OperatorDashboardController.cs`)
- Manages operator-specific bus and location operations
- Enforces authorization - operators can only manage their own resources
- Routes:
  - `GET /api/operator-dashboard/buses` - Get operator's buses
  - `POST /api/operator-dashboard/buses` - Create bus
  - `PUT /api/operator-dashboard/buses/{id}` - Update bus
  - `DELETE /api/operator-dashboard/buses/{id}` - Delete bus
  - `GET /api/operator-dashboard/locations` - Get operator's locations
  - `POST /api/operator-dashboard/locations` - Create location
  - `PUT /api/operator-dashboard/locations/{id}` - Update location
  - `DELETE /api/operator-dashboard/locations/{id}` - Delete location

#### 4. **Extended Operator Service** (`OperatorService.cs`)
- Added methods to retrieve operator-specific buses and locations
- `GetOperatorBusesAsync()` - Fetches all buses for an operator
- `GetOperatorLocationsAsync()` - Fetches all locations for an operator

### Frontend Components

#### 1. **Operator Auth Service** (`operator-auth.service.ts`)
- Manages operator authentication state
- Stores token and operator info in localStorage with separate keys
- Provides observables for reactive updates
- Methods:
  - `signup()` - Register new operator
  - `login()` - Authenticate operator
  - `logout()` - Clear authentication
  - `getToken()` - Retrieve stored token
  - `getCurrentOperator()` - Get current operator info

#### 2. **Operator Login Component** (`operator-login.component.ts`)
- Standalone Angular component for operator login
- Form validation with reactive forms
- Error handling and loading states
- Redirects to operator dashboard on success

#### 3. **Operator Signup Component** (`operator-signup.component.ts`)
- Standalone Angular component for operator registration
- Collects: operator name, email, phone, license number, address, password
- Password confirmation validation
- Redirects to operator dashboard on success

#### 4. **Operator Dashboard Component** (`operator-dashboard.component.ts`)
- Main operator interface with tabbed layout
- Two tabs: "My Buses" and "My Locations"
- Features:
  - **Bus Management:**
    - View all operator buses
    - Create new bus with form validation
    - Edit existing buses
    - Delete buses with confirmation
    - Display bus details (registration number, capacity, price, locations)
  
  - **Location Management:**
    - View all operator locations
    - Create new location with hierarchical selection (Country → State → District)
    - Edit existing locations
    - Delete locations with confirmation
    - Display location details (address, city, postal code, coordinates)

#### 5. **Home Component** (`home.component.ts`)
- Landing page with role selection
- Two options: Passenger and Bus Operator
- Links to respective login/signup pages

#### 6. **Operator Auth Guard** (`operator-auth.guard.ts`)
- Route protection for operator dashboard
- Redirects unauthenticated users to operator login

### Data Models

#### Backend DTOs
- `OperatorLoginRequest` - Email and password
- `OperatorSignupRequest` - Registration details
- `OperatorAuthResponse` - Token and operator info
- `CreateBusDto` - Bus creation payload
- `UpdateBusDto` - Bus update payload
- `CreateLocationDto` - Location creation payload
- `UpdateLocationDto` - Location update payload

#### Frontend Models
- `OperatorLoginRequest` - Login credentials
- `OperatorSignupRequest` - Registration details
- `OperatorAuthResponse` - Auth response
- `OperatorDto` - Operator information
- `BusDto` - Bus details
- `LocationDto` - Location details

## Security Features

1. **JWT Authentication**
   - Tokens include operator ID, email, name, and role claim
   - Configurable expiration (default 60 minutes)
   - HS256 signing algorithm

2. **Authorization**
   - Operators can only access their own resources
   - Dashboard controller validates operator ID from token
   - Returns 403 Forbidden for unauthorized access

3. **Password Security**
   - SHA256 hashing for password storage
   - Password confirmation on signup
   - Minimum 6 character requirement

4. **Data Validation**
   - Email uniqueness validation
   - License number uniqueness validation
   - Required field validation
   - Format validation (phone number, postal code, etc.)

## API Endpoints Summary

### Authentication
```
POST /api/operator-auth/login
POST /api/operator-auth/signup
```

### Bus Management
```
GET /api/operator-dashboard/buses
POST /api/operator-dashboard/buses
GET /api/operator-dashboard/buses/{id}
PUT /api/operator-dashboard/buses/{id}
DELETE /api/operator-dashboard/buses/{id}
```

### Location Management
```
GET /api/operator-dashboard/locations
POST /api/operator-dashboard/locations
GET /api/operator-dashboard/locations/{id}
PUT /api/operator-dashboard/locations/{id}
DELETE /api/operator-dashboard/locations/{id}
```

## Database Schema

### BusOperator Table
- `id` (PK)
- `operator_name`
- `email` (UNIQUE)
- `phone_number`
- `license_number` (UNIQUE)
- `address`
- `password_hash`
- `is_active`
- `created_at`
- `updated_at`

### Location Table (Modified)
- Added `operator_id` (FK) - nullable for public locations
- Operators can only CRUD locations they created

### Bus Table (Existing)
- `operator_id` (FK) - links to BusOperator
- Operators can only CRUD their own buses

## Workflow

### Operator Registration Flow
1. Operator visits `/operator-signup`
2. Fills registration form with business details
3. System validates email and license uniqueness
4. Password is hashed and stored
5. Operator account created
6. JWT token generated
7. Redirects to operator dashboard

### Operator Login Flow
1. Operator visits `/operator-login`
2. Enters email and password
3. System validates credentials
4. JWT token generated on success
5. Token stored in localStorage
6. Redirects to operator dashboard

### Bus Management Flow
1. Operator views "My Buses" tab
2. Can create new bus with form
3. System validates route and location IDs
4. Bus linked to operator
5. Operator can edit or delete own buses
6. List updates in real-time

### Location Management Flow
1. Operator views "My Locations" tab
2. Can create new location with hierarchical selection
3. Location automatically linked to operator
4. Operator can edit or delete own locations
5. Locations can be used as source/destination for buses

## Frontend Routes

```
/home                    - Landing page
/login                   - User login
/signup                  - User registration
/operator-login          - Operator login
/operator-signup         - Operator registration
/operator-dashboard      - Operator dashboard (protected)
/dashboard               - User dashboard (protected)
```

## Configuration

### Backend (Program.cs)
```csharp
builder.Services.AddScoped<IOperatorAuthService, OperatorAuthService>();
```

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Secret": "your-secret-key-change-this-in-production",
    "Issuer": "BusBookingAPI",
    "Audience": "BusBookingClient",
    "ExpirationMinutes": 60
  }
}
```

## Testing Checklist

### Backend
- [ ] Operator signup with valid data
- [ ] Operator signup with duplicate email
- [ ] Operator signup with duplicate license
- [ ] Operator login with correct credentials
- [ ] Operator login with incorrect password
- [ ] Create bus as operator
- [ ] Create location as operator
- [ ] Operator cannot access other operator's buses
- [ ] Operator cannot access other operator's locations
- [ ] Delete bus with active bookings (should fail)
- [ ] Delete location with active buses (should fail)

### Frontend
- [ ] Operator signup form validation
- [ ] Operator login form validation
- [ ] Dashboard loads operator's buses
- [ ] Dashboard loads operator's locations
- [ ] Create bus form works
- [ ] Edit bus form works
- [ ] Delete bus with confirmation
- [ ] Create location form works
- [ ] Edit location form works
- [ ] Delete location with confirmation
- [ ] Logout clears token and redirects
- [ ] Protected routes redirect unauthenticated users

## Future Enhancements

1. **Analytics Dashboard**
   - Revenue tracking
   - Booking statistics
   - Bus utilization rates

2. **Advanced Features**
   - Bulk bus import/export
   - Schedule management
   - Dynamic pricing
   - Promotional codes

3. **Integration**
   - Payment gateway integration
   - SMS notifications
   - Email notifications
   - Real-time tracking

4. **Admin Features**
   - Operator verification workflow
   - Commission management
   - Dispute resolution

## Troubleshooting

### Common Issues

1. **401 Unauthorized on Dashboard**
   - Check token is stored in localStorage
   - Verify token hasn't expired
   - Check Authorization header format

2. **403 Forbidden on Resource Access**
   - Verify operator ID in token matches resource owner
   - Check operator has permission for operation

3. **Bus/Location Creation Fails**
   - Verify route/location IDs exist
   - Check all required fields are provided
   - Verify operator is authenticated

4. **CORS Errors**
   - Ensure backend CORS policy allows frontend origin
   - Check API URL is correct

## Support

For issues or questions, refer to:
- Backend logs: `backend/BusBookingAPI/logs/`
- Frontend console: Browser DevTools
- API documentation: Swagger UI at `http://localhost:5266`
