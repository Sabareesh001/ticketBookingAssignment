# Authentication Frontend Implementation

## Overview
Complete authentication system for the Bus Booking application with support for three user roles: User, Admin, and Bus Operator.

## Features Implemented

### 1. Authentication Pages
- **Login** (`/login`) - User login with email and password
- **Sign Up** (`/signup`) - New user registration with role selection
- **Forgot Password** (`/forgot-password`) - Request password reset link
- **Reset Password** (`/reset-password`) - Reset password with token
- **Unauthorized** (`/unauthorized`) - Access denied page

### 2. Authentication Service
Located in `src/app/services/auth.service.ts`

**Key Methods:**
- `signup(request: SignupRequest)` - Register new user
- `login(request: LoginRequest)` - Authenticate user
- `logout()` - Clear authentication state
- `forgotPassword(request: ForgotPasswordRequest)` - Request password reset
- `resetPassword(request: ResetPasswordRequest)` - Reset password
- `changePassword(userId, request)` - Change password for authenticated user
- `getCurrentUser()` - Get current logged-in user
- `getToken()` - Get JWT token
- `isAuthenticated()` - Check if user is authenticated
- `getUserRole()` - Get current user's role
- `hasRole(role)` - Check if user has specific role
- `hasAnyRole(roles)` - Check if user has any of the specified roles

**Observables:**
- `currentUser$` - Observable of current user
- `isAuthenticated$` - Observable of authentication state

### 3. Auth Guard
Located in `src/app/guards/auth.guard.ts`

Protects routes and enforces role-based access control:
```typescript
// Usage in routes
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [AuthGuardService],
  data: { roles: [UserRole.ADMIN] }
}
```

### 4. HTTP Interceptor
Located in `src/app/interceptors/auth.interceptor.ts`

- Automatically adds JWT token to all HTTP requests
- Handles 401 unauthorized responses by logging out user
- Redirects to login on authentication failure

### 5. Models & DTOs
Located in `src/app/models/auth.model.ts`

**Key Interfaces:**
- `LoginRequest` - Email and password
- `SignupRequest` - Full registration details
- `AuthResponse` - API response with token and user
- `UserDto` - User data transfer object
- `ChangePasswordRequest` - Current and new password
- `DecodedToken` - JWT token payload

**Enums:**
- `UserRole` - USER, ADMIN, BUS_OPERATOR

## Setup Instructions

### 1. Update API Base URL
In `src/app/services/auth.service.ts`, update the API URL if needed:
```typescript
private apiUrl = 'http://localhost:5001/api';
```

### 2. Configure Routes
Update `src/app/app.routes.ts` to add protected routes:
```typescript
{
  path: 'dashboard',
  component: DashboardComponent,
  canActivate: [AuthGuardService],
  data: { roles: [UserRole.USER, UserRole.ADMIN, UserRole.BUS_OPERATOR] }
}
```

### 3. Add HTTP Client Provider
Already configured in `app.config.ts` with:
- `provideHttpClient()`
- `AuthInterceptor` as HTTP_INTERCEPTORS

## API Endpoints Required

The backend should provide these endpoints:

### Authentication
- `POST /api/auth/login` - Login user
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password

### User Management
- `POST /api/user` - Create new user (signup)
- `GET /api/user/{id}` - Get user by ID
- `GET /api/user/email/{email}` - Get user by email
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user
- `POST /api/user/{id}/change-password` - Change password
- `POST /api/user/{id}/validate-password` - Validate password

## User Roles

### 1. User (Regular Customer)
- Can search and book buses
- Can view their bookings
- Can manage their profile

### 2. Admin
- Can manage all users
- Can manage bus operators
- Can view all bookings
- Can manage routes and buses

### 3. Bus Operator
- Can manage their buses
- Can view bookings for their buses
- Can manage routes
- Can update bus schedules

## JWT Token Handling

The service automatically:
1. Stores JWT token in localStorage with key `auth_token`
2. Stores user data in localStorage with key `current_user`
3. Adds token to Authorization header for all requests
4. Decodes token to extract user role and expiration
5. Handles token expiration (401 responses)

## Form Validation

All forms include:
- Required field validation
- Email format validation
- Password strength validation (minimum 6 characters)
- Password confirmation matching
- Phone number format validation (10 digits)
- Custom error messages

## Styling

All components use:
- Gradient background (purple theme)
- Responsive design (mobile-friendly)
- Consistent form styling
- Error and success alerts
- Smooth transitions and hover effects

## Usage Examples

### Login
```typescript
this.authService.login({
  email: 'user@example.com',
  password: 'password123'
}).subscribe({
  next: (response) => {
    // User logged in successfully
    this.router.navigate(['/dashboard']);
  },
  error: (error) => {
    // Handle login error
  }
});
```

### Check Authentication
```typescript
if (this.authService.isAuthenticated()) {
  // User is logged in
}

// Or use observable
this.authService.isAuthenticated$.subscribe(isAuth => {
  // React to auth state changes
});
```

### Check User Role
```typescript
if (this.authService.hasRole(UserRole.ADMIN)) {
  // Show admin features
}

if (this.authService.hasAnyRole([UserRole.ADMIN, UserRole.BUS_OPERATOR])) {
  // Show admin or operator features
}
```

### Get Current User
```typescript
const user = this.authService.getCurrentUser();
console.log(user.fullName, user.email);

// Or use observable
this.authService.currentUser$.subscribe(user => {
  if (user) {
    console.log('User:', user);
  }
});
```

### Logout
```typescript
this.authService.logout();
this.router.navigate(['/login']);
```

## File Structure

```
src/app/
├── models/
│   └── auth.model.ts
├── services/
│   └── auth.service.ts
├── guards/
│   └── auth.guard.ts
├── interceptors/
│   └── auth.interceptor.ts
├── pages/
│   ├── login/
│   │   ├── login.component.ts
│   │   ├── login.component.html
│   │   └── login.component.css
│   ├── signup/
│   │   ├── signup.component.ts
│   │   ├── signup.component.html
│   │   └── signup.component.css
│   ├── forgot-password/
│   │   ├── forgot-password.component.ts
│   │   ├── forgot-password.component.html
│   │   └── forgot-password.component.css
│   ├── reset-password/
│   │   ├── reset-password.component.ts
│   │   ├── reset-password.component.html
│   │   └── reset-password.component.css
│   └── unauthorized/
│       ├── unauthorized.component.ts
│       ├── unauthorized.component.html
│       └── unauthorized.component.css
├── app.routes.ts
└── app.config.ts
```

## Next Steps

1. **Create Backend Auth Endpoints** - Implement login, signup, forgot-password endpoints
2. **Add Dashboard Component** - Create main dashboard after login
3. **Add Role-Based Components** - Create admin, operator, and user dashboards
4. **Add Route Creation** - If no route exists between source and destination, create it
5. **Implement Email Service** - For password reset emails
6. **Add Token Refresh** - Implement refresh token mechanism
7. **Add 2FA** - Two-factor authentication for enhanced security

## Security Considerations

- JWT tokens are stored in localStorage (consider using httpOnly cookies for production)
- Passwords are hashed on the backend
- All API calls include CORS headers
- 401 responses trigger automatic logout
- Role-based access control on frontend (backend should also enforce)
- Password validation on both client and server

## Troubleshooting

### Login not working
- Check API URL in auth.service.ts
- Verify backend endpoints are running
- Check browser console for CORS errors
- Ensure credentials are correct

### Token not persisting
- Check localStorage is enabled
- Verify token is being returned from login endpoint
- Check browser's Application tab in DevTools

### Routes not protected
- Ensure AuthGuardService is added to route
- Verify user role matches required roles in route data
- Check that token is valid and not expired

### Interceptor not adding token
- Verify HTTP_INTERCEPTORS is configured in app.config.ts
- Check that requests are using HttpClient
- Ensure token exists in localStorage
