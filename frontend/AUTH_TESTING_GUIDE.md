# Authentication Testing Guide

## Manual Testing Checklist

### 1. Login Page Tests

#### Test 1.1: Valid Login
- [ ] Navigate to `/login`
- [ ] Enter valid email and password
- [ ] Click "Login" button
- [ ] Verify user is redirected to dashboard
- [ ] Verify token is stored in localStorage
- [ ] Verify user data is stored in localStorage

#### Test 1.2: Invalid Email
- [ ] Enter invalid email format
- [ ] Verify error message appears
- [ ] Verify submit button is disabled

#### Test 1.3: Empty Fields
- [ ] Leave email empty
- [ ] Click submit
- [ ] Verify error message "Email is required"
- [ ] Leave password empty
- [ ] Click submit
- [ ] Verify error message "Password is required"

#### Test 1.4: Short Password
- [ ] Enter password less than 6 characters
- [ ] Verify error message "Password must be at least 6 characters"

#### Test 1.5: Wrong Credentials
- [ ] Enter valid email but wrong password
- [ ] Click submit
- [ ] Verify error message appears
- [ ] Verify user is NOT logged in

#### Test 1.6: Non-existent User
- [ ] Enter email that doesn't exist
- [ ] Click submit
- [ ] Verify error message appears

#### Test 1.7: Loading State
- [ ] Click login button
- [ ] Verify button shows "Logging in..." text
- [ ] Verify button is disabled during request

#### Test 1.8: Return URL
- [ ] Try to access protected route without login
- [ ] Should redirect to `/login?returnUrl=/protected-route`
- [ ] After login, should redirect to the protected route

### 2. Sign Up Page Tests

#### Test 2.1: Valid Sign Up
- [ ] Navigate to `/signup`
- [ ] Fill all fields with valid data
- [ ] Click "Sign Up" button
- [ ] Verify user is created
- [ ] Verify user is logged in
- [ ] Verify redirected to dashboard

#### Test 2.2: Email Validation
- [ ] Enter invalid email format
- [ ] Verify error message "Email must be valid"

#### Test 2.3: Full Name Validation
- [ ] Enter name with less than 3 characters
- [ ] Verify error message "Full name must be at least 3 characters"

#### Test 2.4: Phone Number Validation
- [ ] Enter phone number with less than 10 digits
- [ ] Verify error message "Phone number must be 10 digits"
- [ ] Enter phone number with non-numeric characters
- [ ] Verify error message appears

#### Test 2.5: Password Validation
- [ ] Enter password less than 6 characters
- [ ] Verify error message "Password must be at least 6 characters"

#### Test 2.6: Password Confirmation
- [ ] Enter different passwords in password and confirm password
- [ ] Verify error message "Passwords do not match"

#### Test 2.7: Address Validation
- [ ] Enter address with less than 5 characters
- [ ] Verify error message "Address must be at least 5 characters"

#### Test 2.8: Date of Birth
- [ ] Leave date of birth empty
- [ ] Verify error message "Date of birth is required"

#### Test 2.9: Duplicate Email
- [ ] Try to sign up with existing email
- [ ] Verify error message "Email already exists"

#### Test 2.10: Form Reset
- [ ] Fill form with data
- [ ] Click submit
- [ ] After successful signup, verify form is cleared

### 3. Forgot Password Page Tests

#### Test 3.1: Valid Email
- [ ] Navigate to `/forgot-password`
- [ ] Enter valid email
- [ ] Click "Send Reset Link"
- [ ] Verify success message appears
- [ ] Verify redirected to login after 3 seconds

#### Test 3.2: Invalid Email Format
- [ ] Enter invalid email format
- [ ] Verify error message "Email must be valid"

#### Test 3.3: Empty Email
- [ ] Leave email empty
- [ ] Click submit
- [ ] Verify error message "Email is required"

#### Test 3.4: Non-existent Email
- [ ] Enter email that doesn't exist
- [ ] Click submit
- [ ] Verify appropriate message (success or error)

#### Test 3.5: Loading State
- [ ] Click "Send Reset Link" button
- [ ] Verify button shows "Sending..." text
- [ ] Verify button is disabled during request

### 4. Reset Password Page Tests

#### Test 4.1: Valid Reset
- [ ] Navigate to `/reset-password?email=user@example.com`
- [ ] Enter new password
- [ ] Confirm password
- [ ] Click "Reset Password"
- [ ] Verify success message
- [ ] Verify redirected to login

#### Test 4.2: Password Mismatch
- [ ] Enter different passwords
- [ ] Verify error message "Passwords do not match"

#### Test 4.3: Short Password
- [ ] Enter password less than 6 characters
- [ ] Verify error message "Password must be at least 6 characters"

#### Test 4.4: Missing Email
- [ ] Navigate to `/reset-password` without email parameter
- [ ] Verify error message "Invalid reset link"

### 5. Unauthorized Page Tests

#### Test 5.1: Access Denied
- [ ] Try to access admin page without admin role
- [ ] Verify redirected to `/unauthorized`
- [ ] Verify error message "Access Denied"
- [ ] Verify "Go to Dashboard" button works
- [ ] Verify "Login" button works

### 6. Authentication State Tests

#### Test 6.1: Token Persistence
- [ ] Login successfully
- [ ] Refresh the page
- [ ] Verify user is still logged in
- [ ] Verify token is still in localStorage

#### Test 6.2: Logout
- [ ] Login successfully
- [ ] Click logout button
- [ ] Verify redirected to login
- [ ] Verify token is removed from localStorage
- [ ] Verify user data is removed from localStorage

#### Test 6.3: Token Expiration
- [ ] Login successfully
- [ ] Wait for token to expire (or manually expire)
- [ ] Try to access protected route
- [ ] Verify redirected to login
- [ ] Verify error message appears

#### Test 6.4: 401 Response
- [ ] Login successfully
- [ ] Manually delete token from localStorage
- [ ] Try to access protected route
- [ ] Verify redirected to login

### 7. Role-Based Access Tests

#### Test 7.1: User Role
- [ ] Login as regular user
- [ ] Verify can access user dashboard
- [ ] Verify cannot access admin pages
- [ ] Verify cannot access operator pages

#### Test 7.2: Admin Role
- [ ] Login as admin
- [ ] Verify can access admin dashboard
- [ ] Verify can access user pages
- [ ] Verify cannot access operator pages

#### Test 7.3: Operator Role
- [ ] Login as operator
- [ ] Verify can access operator dashboard
- [ ] Verify can access user pages
- [ ] Verify cannot access admin pages

### 8. HTTP Interceptor Tests

#### Test 8.1: Token in Headers
- [ ] Login successfully
- [ ] Open DevTools Network tab
- [ ] Make any API request
- [ ] Verify Authorization header contains "Bearer {token}"

#### Test 8.2: No Token When Logged Out
- [ ] Logout
- [ ] Open DevTools Network tab
- [ ] Make any API request
- [ ] Verify Authorization header is NOT present

#### Test 8.3: 401 Handling
- [ ] Login successfully
- [ ] Manually delete token from localStorage
- [ ] Make API request
- [ ] Verify 401 response triggers logout
- [ ] Verify redirected to login

### 9. Form Validation Tests

#### Test 9.1: Real-time Validation
- [ ] Start typing in email field
- [ ] Verify error appears as you type
- [ ] Verify error disappears when valid

#### Test 9.2: Submit Validation
- [ ] Leave all fields empty
- [ ] Click submit
- [ ] Verify all error messages appear

#### Test 9.3: Error Clearing
- [ ] Enter invalid data
- [ ] Verify error message
- [ ] Correct the data
- [ ] Verify error message disappears

### 10. UI/UX Tests

#### Test 10.1: Responsive Design
- [ ] Test on desktop (1920x1080)
- [ ] Test on tablet (768x1024)
- [ ] Test on mobile (375x667)
- [ ] Verify layout is responsive
- [ ] Verify buttons are clickable on mobile

#### Test 10.2: Loading States
- [ ] Click submit button
- [ ] Verify button shows loading text
- [ ] Verify button is disabled
- [ ] Verify spinner or loading indicator appears

#### Test 10.3: Error Messages
- [ ] Trigger various errors
- [ ] Verify error messages are clear
- [ ] Verify error messages are visible
- [ ] Verify error messages are in red

#### Test 10.4: Success Messages
- [ ] Complete successful actions
- [ ] Verify success messages appear
- [ ] Verify success messages are in green
- [ ] Verify success messages disappear after timeout

#### Test 10.5: Navigation Links
- [ ] Test all navigation links
- [ ] Verify links work correctly
- [ ] Verify links navigate to correct pages

### 11. Browser Compatibility Tests

#### Test 11.1: Chrome
- [ ] Test all features in Chrome
- [ ] Verify no console errors

#### Test 11.2: Firefox
- [ ] Test all features in Firefox
- [ ] Verify no console errors

#### Test 11.3: Safari
- [ ] Test all features in Safari
- [ ] Verify no console errors

#### Test 11.4: Edge
- [ ] Test all features in Edge
- [ ] Verify no console errors

### 12. Security Tests

#### Test 12.1: XSS Prevention
- [ ] Try to inject script in form fields
- [ ] Verify script is not executed
- [ ] Verify data is properly escaped

#### Test 12.2: CSRF Protection
- [ ] Verify CSRF tokens are used (if applicable)
- [ ] Verify requests are validated

#### Test 12.3: Password Security
- [ ] Verify passwords are not logged
- [ ] Verify passwords are not visible in network requests
- [ ] Verify passwords are hashed on backend

#### Test 12.4: Token Security
- [ ] Verify token is not exposed in URL
- [ ] Verify token is stored securely
- [ ] Verify token is sent only over HTTPS (in production)

## Automated Testing

### Unit Tests Example

```typescript
describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthService],
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should login successfully', () => {
    const mockResponse = {
      success: true,
      token: 'test-token',
      user: { id: 1, email: 'test@example.com' }
    };

    service.login({ email: 'test@example.com', password: 'password' }).subscribe(response => {
      expect(response.success).toBe(true);
      expect(service.getToken()).toBe('test-token');
    });

    const req = httpMock.expectOne('http://localhost:5001/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should logout successfully', () => {
    service.logout();
    expect(service.getToken()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
  });
});
```

## Test Data

### Valid Test Credentials
```
Email: test@example.com
Password: password123
Full Name: Test User
Phone: 9876543210
DOB: 1990-01-15
Address: 123 Test Street
```

### Invalid Test Data
```
Invalid Email: notanemail
Short Password: 12345
Invalid Phone: 123
Short Address: 123
```

## Performance Testing

### Load Testing
- [ ] Test login with 100 concurrent users
- [ ] Verify response time < 2 seconds
- [ ] Verify no errors occur

### Memory Testing
- [ ] Monitor memory usage during login
- [ ] Verify no memory leaks
- [ ] Verify localStorage is not bloated

## Accessibility Testing

### Keyboard Navigation
- [ ] Tab through all form fields
- [ ] Verify focus is visible
- [ ] Verify Enter key submits form

### Screen Reader Testing
- [ ] Test with screen reader
- [ ] Verify all labels are read
- [ ] Verify error messages are announced

### Color Contrast
- [ ] Verify text contrast ratio > 4.5:1
- [ ] Verify buttons are distinguishable

## Test Report Template

```
Test Date: ___________
Tester: ___________
Browser: ___________
OS: ___________

Test Results:
- Login: PASS / FAIL
- Sign Up: PASS / FAIL
- Forgot Password: PASS / FAIL
- Reset Password: PASS / FAIL
- Unauthorized: PASS / FAIL
- Token Persistence: PASS / FAIL
- Logout: PASS / FAIL
- Role-Based Access: PASS / FAIL
- HTTP Interceptor: PASS / FAIL
- Form Validation: PASS / FAIL
- Responsive Design: PASS / FAIL
- Error Handling: PASS / FAIL

Issues Found:
1. ___________
2. ___________

Notes:
___________
```

## Continuous Integration

### GitHub Actions Example
```yaml
name: Auth Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-node@v2
        with:
          node-version: '18'
      - run: npm install
      - run: npm run test
      - run: npm run lint
```

## Debugging Tips

### Check localStorage
```javascript
// In browser console
localStorage.getItem('auth_token')
localStorage.getItem('current_user')
```

### Check Network Requests
1. Open DevTools (F12)
2. Go to Network tab
3. Look for API requests
4. Check request/response headers and body

### Check Console Errors
1. Open DevTools (F12)
2. Go to Console tab
3. Look for any error messages
4. Check stack traces

### Enable Debug Logging
```typescript
// In auth.service.ts
console.log('Login attempt:', request);
console.log('Login response:', response);
```

## Common Issues and Solutions

### Issue: Login not working
**Solution**: 
- Check API URL is correct
- Verify backend is running
- Check credentials are correct
- Look for CORS errors in console

### Issue: Token not persisting
**Solution**:
- Check localStorage is enabled
- Verify token is returned from login
- Check DevTools Application tab

### Issue: Routes not protected
**Solution**:
- Verify AuthGuardService is added to route
- Check user role matches required roles
- Verify token is valid

### Issue: Interceptor not adding token
**Solution**:
- Verify HTTP_INTERCEPTORS is configured
- Check token exists in localStorage
- Verify requests use HttpClient

## Sign-Off

- [ ] All manual tests passed
- [ ] All automated tests passed
- [ ] No critical issues found
- [ ] Performance acceptable
- [ ] Accessibility verified
- [ ] Security verified
- [ ] Ready for production

**Tested By**: ___________
**Date**: ___________
**Status**: ✅ APPROVED / ❌ REJECTED
