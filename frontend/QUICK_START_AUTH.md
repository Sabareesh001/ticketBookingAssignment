# Quick Start - Authentication Frontend

## What's Been Created

A complete, production-ready authentication system with:
- ✅ Login page
- ✅ Sign up page (with role selection)
- ✅ Forgot password page
- ✅ Reset password page
- ✅ Unauthorized access page
- ✅ JWT token management
- ✅ Role-based access control
- ✅ HTTP interceptor for token injection
- ✅ Auth guards for route protection
- ✅ Responsive design with modern UI

## Getting Started

### 1. Start the Frontend
```bash
cd frontend/bus-booking
npm start
```

### 2. Access Auth Pages
- Login: `http://localhost:4200/login`
- Sign Up: `http://localhost:4200/signup`
- Forgot Password: `http://localhost:4200/forgot-password`

### 3. Backend Requirements

Your backend needs these endpoints:

#### Login
```
POST /api/auth/login
Body: { email: string, password: string }
Response: { success: boolean, token: string, user: UserDto }
```

#### Sign Up
```
POST /api/user
Body: { fullName, email, phoneNumber, password, dateOfBirth, address }
Response: { success: boolean, token: string, user: UserDto }
```

#### Forgot Password
```
POST /api/auth/forgot-password
Body: { email: string }
Response: { success: boolean, message: string }
```

#### Reset Password
```
POST /api/auth/reset-password
Body: { email: string, newPassword: string }
Response: { success: boolean, message: string }
```

#### Change Password
```
POST /api/user/{id}/change-password
Body: { currentPassword: string, newPassword: string }
Response: { success: boolean, message: string }
```

## Key Files

| File | Purpose |
|------|---------|
| `src/app/services/auth.service.ts` | Core auth logic |
| `src/app/guards/auth.guard.ts` | Route protection |
| `src/app/interceptors/auth.interceptor.ts` | Token injection |
| `src/app/models/auth.model.ts` | TypeScript interfaces |
| `src/app/pages/login/` | Login component |
| `src/app/pages/signup/` | Sign up component |
| `src/app/pages/forgot-password/` | Forgot password component |
| `src/app/pages/reset-password/` | Reset password component |

## Using Auth in Your Components

### Check if User is Logged In
```typescript
import { AuthService } from './services/auth.service';

constructor(private authService: AuthService) {}

if (this.authService.isAuthenticated()) {
  // User is logged in
}
```

### Get Current User
```typescript
const user = this.authService.getCurrentUser();
console.log(user.fullName, user.email);
```

### Check User Role
```typescript
import { UserRole } from './models/auth.model';

if (this.authService.hasRole(UserRole.ADMIN)) {
  // Show admin features
}
```

### Protect Routes
```typescript
// In app.routes.ts
{
  path: 'admin',
  component: AdminComponent,
  canActivate: [AuthGuardService],
  data: { roles: [UserRole.ADMIN] }
}
```

### Logout
```typescript
this.authService.logout();
this.router.navigate(['/login']);
```

## User Roles

Three roles are supported:
- **USER** - Regular customer
- **ADMIN** - System administrator
- **BUS_OPERATOR** - Bus company operator

## API Configuration

Update the API URL in `src/app/services/auth.service.ts`:
```typescript
private apiUrl = 'http://localhost:5001/api';
```

## What Happens on Login

1. User enters email and password
2. Frontend sends POST to `/api/auth/login`
3. Backend validates and returns JWT token
4. Token is stored in localStorage
5. User data is stored in localStorage
6. User is redirected to dashboard
7. All subsequent requests include the token in Authorization header

## What Happens on Logout

1. Token is removed from localStorage
2. User data is cleared
3. User is redirected to login page
4. All subsequent requests won't have the token

## Token Expiration

When a request returns 401 (Unauthorized):
1. User is automatically logged out
2. User is redirected to login page
3. Error message is shown

## Form Validation

All forms validate:
- ✅ Required fields
- ✅ Email format
- ✅ Password strength (min 6 chars)
- ✅ Password confirmation
- ✅ Phone number format (10 digits)
- ✅ Address length (min 5 chars)

## Styling

- Modern gradient background (purple theme)
- Responsive design (works on mobile, tablet, desktop)
- Smooth animations and transitions
- Clear error messages
- Success notifications

## Next Steps

1. **Create Dashboard** - Add a dashboard component for authenticated users
2. **Add Role-Based Pages** - Create admin, operator, and user dashboards
3. **Implement Email Service** - Send password reset emails
4. **Add 2FA** - Two-factor authentication
5. **Add Profile Management** - Allow users to update their profile
6. **Add Booking Pages** - Search and book buses

## Troubleshooting

### "Cannot find module" errors
```bash
npm install
```

### CORS errors
- Check backend is running on correct port
- Verify CORS is enabled in backend
- Check API URL in auth.service.ts

### Login not working
- Check backend endpoints are implemented
- Verify credentials are correct
- Check browser console for errors

### Token not persisting
- Check localStorage is enabled
- Verify token is returned from login
- Check DevTools Application tab

## Support

For issues or questions:
1. Check the browser console for errors
2. Check the backend logs
3. Verify API endpoints are correct
4. Check that backend is running

## Production Checklist

- [ ] Update API URL to production backend
- [ ] Enable HTTPS
- [ ] Implement token refresh mechanism
- [ ] Add email verification
- [ ] Add password reset email
- [ ] Implement rate limiting
- [ ] Add security headers
- [ ] Test on multiple browsers
- [ ] Test on mobile devices
- [ ] Set up error logging
- [ ] Configure CORS properly
