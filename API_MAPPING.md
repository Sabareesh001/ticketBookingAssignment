# Frontend to Backend API Mapping

## Overview
This document maps all frontend API calls to their corresponding backend endpoint implementations.

---

## Authentication APIs

### 1. Signup (User Registration)
**Frontend Call:**
```typescript
// auth.service.ts
signup(request: SignupRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.apiUrl}/user`, request)
}
```

**Request Body:**
```typescript
{
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  dateOfBirth: string;
  address: string;
}
```

**Backend Endpoint:**
```
POST /api/user
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `CreateUser(CreateUserDto createUserDto)`
- **Service:** `AuthService.SignupAsync(CreateUserDto request)`
- **Response:** `AuthResponse` with JWT token and user details
- **Status Codes:** 201 (Created), 400 (Bad Request), 500 (Server Error)

---

### 2. Login
**Frontend Call:**
```typescript
// auth.service.ts
login(request: LoginRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
}
```

**Request Body:**
```typescript
{
  email: string;
  password: string;
}
```

**Backend Endpoint:**
```
POST /api/auth/login
```

**Backend Implementation:**
- **Controller:** `AuthController.cs`
- **Method:** `Login(LoginRequest request)`
- **Service:** `AuthService.LoginAsync(LoginRequest request)`
- **Response:** `AuthResponse` with JWT token and user details
- **Status Codes:** 200 (OK), 400 (Bad Request), 401 (Unauthorized), 500 (Server Error)

---

### 3. Forgot Password
**Frontend Call:**
```typescript
// auth.service.ts
forgotPassword(request: ForgotPasswordRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.apiUrl}/auth/forgot-password`, request)
}
```

**Request Body:**
```typescript
{
  email: string;
}
```

**Backend Endpoint:**
```
POST /api/auth/forgot-password
```

**Backend Implementation:**
- **Controller:** `AuthController.cs`
- **Method:** `ForgotPassword(ForgotPasswordRequest request)`
- **Service:** `AuthService.ForgotPasswordAsync(ForgotPasswordRequest request)`
- **Response:** `AuthResponse` with success message
- **Status Codes:** 200 (OK), 400 (Bad Request), 500 (Server Error)
- **Note:** For security, returns same message whether email exists or not

---

### 4. Reset Password
**Frontend Call:**
```typescript
// auth.service.ts
resetPassword(request: ResetPasswordRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.apiUrl}/auth/reset-password`, request)
}
```

**Request Body:**
```typescript
{
  email: string;
  newPassword: string;
}
```

**Backend Endpoint:**
```
POST /api/auth/reset-password
```

**Backend Implementation:**
- **Controller:** `AuthController.cs`
- **Method:** `ResetPassword(ResetPasswordRequest request)`
- **Service:** `AuthService.ResetPasswordAsync(ResetPasswordRequest request)`
- **Response:** `AuthResponse` with JWT token and user details
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 5. Change Password
**Frontend Call:**
```typescript
// auth.service.ts
changePassword(userId: number, request: ChangePasswordRequest): Observable<any> {
  return this.http.post(`${this.apiUrl}/user/${userId}/change-password`, request)
}
```

**Request Body:**
```typescript
{
  currentPassword: string;
  newPassword: string;
}
```

**Backend Endpoint:**
```
POST /api/user/{userId}/change-password
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `ChangePassword(int id, ChangePasswordDto changePasswordDto)`
- **Service:** `UserService.ChangePasswordAsync(int userId, string currentPassword, string newPassword)`
- **Response:** `{ message: "Password changed successfully" }`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

## User Management APIs

### 6. Get User by ID
**Backend Endpoint:**
```
GET /api/user/{id}
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `GetUser(int id)`
- **Service:** `UserService.GetUserByIdAsync(int id)`
- **Response:** `UserDto`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 7. Get All Users
**Backend Endpoint:**
```
GET /api/user
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `GetAllUsers()`
- **Service:** `UserService.GetAllUsersAsync()`
- **Response:** `List<UserDto>`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 8. Get User by Email
**Backend Endpoint:**
```
GET /api/user/email/{email}
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `GetUserByEmail(string email)`
- **Service:** `UserService.GetUserByEmailAsync(string email)`
- **Response:** `UserDto`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 9. Update User
**Backend Endpoint:**
```
PUT /api/user/{id}
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `UpdateUser(int id, UpdateUserDto updateUserDto)`
- **Service:** `UserService.UpdateUserAsync(int id, UpdateUserDto updateUserDto)`
- **Response:** `UserDto`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 10. Delete User
**Backend Endpoint:**
```
DELETE /api/user/{id}
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `DeleteUser(int id)`
- **Service:** `UserService.DeleteUserAsync(int id)`
- **Response:** `{ message: "User with ID {id} deleted successfully" }`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 11. Validate Password
**Backend Endpoint:**
```
POST /api/user/{id}/validate-password
```

**Backend Implementation:**
- **Controller:** `UserController.cs`
- **Method:** `ValidatePassword(int id, ValidatePasswordDto validatePasswordDto)`
- **Service:** `UserService.ValidatePasswordAsync(int userId, string password)`
- **Response:** `{ isValid: boolean }`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

## Booking APIs

### 12. Get Booking by ID
**Backend Endpoint:**
```
GET /api/booking/{id}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `GetBooking(int id)`
- **Service:** `BookingService.GetBookingByIdAsync(int id)`
- **Response:** `BookingDto`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 13. Get All Bookings
**Backend Endpoint:**
```
GET /api/booking
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `GetAllBookings()`
- **Service:** `BookingService.GetAllBookingsAsync()`
- **Response:** `List<BookingDto>`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 14. Get Bookings by User ID
**Backend Endpoint:**
```
GET /api/booking/user/{userId}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `GetBookingsByUserId(int userId)`
- **Service:** `BookingService.GetBookingsByUserIdAsync(int userId)`
- **Response:** `List<BookingDto>`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 15. Get Bookings by Bus ID
**Backend Endpoint:**
```
GET /api/booking/bus/{busId}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `GetBookingsByBusId(int busId)`
- **Service:** `BookingService.GetBookingsByBusIdAsync(int busId)`
- **Response:** `List<BookingDto>`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 16. Get Bookings by Date Range
**Backend Endpoint:**
```
GET /api/booking/date-range?startDate={startDate}&endDate={endDate}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `GetBookingsByDateRange(DateTime startDate, DateTime endDate)`
- **Service:** `BookingService.GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)`
- **Response:** `List<BookingDto>`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 17. Create Booking
**Backend Endpoint:**
```
POST /api/booking
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `CreateBooking(CreateBookingDto createBookingDto)`
- **Service:** `BookingService.CreateBookingAsync(CreateBookingDto createBookingDto)`
- **Response:** `BookingDto`
- **Status Codes:** 201 (Created), 400 (Bad Request), 500 (Server Error)

---

### 18. Update Booking
**Backend Endpoint:**
```
PUT /api/booking/{id}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `UpdateBooking(int id, UpdateBookingDto updateBookingDto)`
- **Service:** `BookingService.UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto)`
- **Response:** `BookingDto`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 19. Delete Booking
**Backend Endpoint:**
```
DELETE /api/booking/{id}
```

**Backend Implementation:**
- **Controller:** `BookingController.cs`
- **Method:** `DeleteBooking(int id)`
- **Service:** `BookingService.DeleteBookingAsync(int id)`
- **Response:** `{ message: "Booking with ID {id} deleted successfully" }`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

## Bus APIs

### 20. Get Bus by ID
**Backend Endpoint:**
```
GET /api/bus/{id}
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `GetBus(int id)`
- **Service:** `BusService.GetBusByIdAsync(int id)`
- **Response:** `BusDto`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 21. Get All Buses
**Backend Endpoint:**
```
GET /api/bus
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `GetAllBuses()`
- **Service:** `BusService.GetAllBusesAsync()`
- **Response:** `List<BusDto>`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 22. Get Available Buses
**Backend Endpoint:**
```
GET /api/bus/available?sourceDistrict={sourceDistrict}&destinationDistrict={destinationDistrict}
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `GetAvailableBuses(string sourceDistrict, string destinationDistrict)`
- **Service:** `BusService.GetAvailableBusesAsync(string sourceDistrict, string destinationDistrict)`
- **Response:** `AvailableBusesResponse`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 23. Create Bus
**Backend Endpoint:**
```
POST /api/bus
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `CreateBus(CreateBusDto createBusDto)`
- **Service:** `BusService.CreateBusAsync(CreateBusDto createBusDto)`
- **Response:** `BusDto`
- **Status Codes:** 201 (Created), 400 (Bad Request), 500 (Server Error)

---

### 24. Update Bus
**Backend Endpoint:**
```
PUT /api/bus/{id}
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `UpdateBus(int id, UpdateBusDto updateBusDto)`
- **Service:** `BusService.UpdateBusAsync(int id, UpdateBusDto updateBusDto)`
- **Response:** `BusDto`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 25. Delete Bus
**Backend Endpoint:**
```
DELETE /api/bus/{id}
```

**Backend Implementation:**
- **Controller:** `BusController.cs`
- **Method:** `DeleteBus(int id)`
- **Service:** `BusService.DeleteBusAsync(int id)`
- **Response:** `{ message: "Bus with ID {id} deleted successfully" }`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

## Route APIs

### 26. Get Route by ID
**Backend Endpoint:**
```
GET /api/route/{id}
```

**Backend Implementation:**
- **Controller:** `RouteController.cs`
- **Method:** `GetRoute(int id)`
- **Service:** `RouteService.GetRouteByIdAsync(int id)`
- **Response:** `RouteDto`
- **Status Codes:** 200 (OK), 404 (Not Found), 500 (Server Error)

---

### 27. Get All Routes
**Backend Endpoint:**
```
GET /api/route
```

**Backend Implementation:**
- **Controller:** `RouteController.cs`
- **Method:** `GetAllRoutes()`
- **Service:** `RouteService.GetAllRoutesAsync()`
- **Response:** `List<RouteDto>`
- **Status Codes:** 200 (OK), 500 (Server Error)

---

### 28. Create Route
**Backend Endpoint:**
```
POST /api/route
```

**Backend Implementation:**
- **Controller:** `RouteController.cs`
- **Method:** `CreateRoute(CreateRouteDto createRouteDto)`
- **Service:** `RouteService.CreateRouteAsync(CreateRouteDto createRouteDto)`
- **Response:** `RouteDto`
- **Status Codes:** 201 (Created), 400 (Bad Request), 500 (Server Error)

---

### 29. Update Route
**Backend Endpoint:**
```
PUT /api/route/{id}
```

**Backend Implementation:**
- **Controller:** `RouteController.cs`
- **Method:** `UpdateRoute(int id, UpdateRouteDto updateRouteDto)`
- **Service:** `RouteService.UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto)`
- **Response:** `RouteDto`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

### 30. Delete Route
**Backend Endpoint:**
```
DELETE /api/route/{id}
```

**Backend Implementation:**
- **Controller:** `RouteController.cs`
- **Method:** `DeleteRoute(int id)`
- **Service:** `RouteService.DeleteRouteAsync(int id)`
- **Response:** `{ message: "Route with ID {id} deleted successfully" }`
- **Status Codes:** 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Server Error)

---

## Additional Controllers Available

The following controllers are also available with similar CRUD operations:

- **LocationController** - `/api/location`
- **StateController** - `/api/state`
- **DistrictController** - `/api/district`
- **CountryController** - `/api/country`
- **OperatorController** - `/api/operator`

Each follows the same pattern:
- `GET /{id}` - Get by ID
- `GET` - Get all
- `POST` - Create
- `PUT /{id}` - Update
- `DELETE /{id}` - Delete

---

## Summary

| Category | Count | Endpoints |
|----------|-------|-----------|
| Authentication | 5 | Login, Signup, Forgot Password, Reset Password, Change Password |
| User Management | 6 | Get by ID, Get All, Get by Email, Update, Delete, Validate Password |
| Bookings | 8 | Get by ID, Get All, Get by User, Get by Bus, Get by Date Range, Create, Update, Delete |
| Buses | 5 | Get by ID, Get All, Get Available, Create, Update, Delete |
| Routes | 5 | Get by ID, Get All, Create, Update, Delete |
| Other Resources | 15+ | Locations, States, Districts, Countries, Operators (CRUD operations) |
| **Total** | **39+** | **Complete REST API** |

---

## API Base URL
```
http://localhost:5266/api
```

## Authentication
JWT Bearer token required for protected endpoints. Include in request header:
```
Authorization: Bearer {token}
```

## Response Format
All responses follow a consistent format:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {}
}
```

Or for errors:
```json
{
  "message": "Error description"
}
```
