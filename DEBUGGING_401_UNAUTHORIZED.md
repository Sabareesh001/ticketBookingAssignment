# Debugging 401 Unauthorized - Complete Guide

## Changes Made for Debugging

### Backend Changes
1. **OperatorDashboardController.cs**
   - Added `AuthenticationSchemes = "Bearer"` to `[Authorize]` attribute
   - Added detailed logging in `GetOperatorIdFromToken()` method
   - Logs all claims from the JWT token
   - Logs operator ID extraction

### Frontend Changes
1. **operator-auth.interceptor.ts**
   - Added console logging for token presence
   - Added logging for Authorization header addition
   - Added logging for responses and errors
   - Logs all interceptor actions

## How to Debug

### Step 1: Rebuild Everything
```bash
# Backend
cd backend/BusBookingAPI
dotnet clean
dotnet build

# Frontend
cd frontend/bus-booking
npm run build
```

### Step 2: Clear Browser Cache
- Press: `Ctrl+Shift+Delete`
- Clear all time, cookies, and cached files

### Step 3: Open Browser DevTools
- Press: `F12`
- Go to: **Console** tab
- Go to: **Network** tab

### Step 4: Test the Flow

1. **Login as Operator**
   - Go to operator login page
   - Enter credentials
   - Click Login
   - Check Console for logs

2. **Navigate to Dashboard**
   - Go to "My Buses" tab
   - Check Console for logs

3. **Click "Add New Bus"**
   - Check Console for logs
   - Check Network tab for API requests

### Step 5: Check Console Logs

Look for these logs in the Console tab:

```
[OperatorAuthInterceptor] URL: http://localhost:5266/api/operator-dashboard/buses
[OperatorAuthInterceptor] Token exists: true
[OperatorAuthInterceptor] Adding Authorization header
[OperatorAuthInterceptor] Response received for http://localhost:5266/api/operator-dashboard/buses
```

**If you see:**
```
[OperatorAuthInterceptor] Token exists: false
```
→ Token is not being stored in localStorage

**If you see:**
```
[OperatorAuthInterceptor] No token found for operator request
```
→ Token retrieval failed

### Step 6: Check Network Tab

1. Look for requests to `/api/operator-dashboard/`
2. Click on each request
3. Go to **Headers** tab
4. Look for **Request Headers** section
5. Check for:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
   ```

**If Authorization header is missing:**
→ Interceptor not adding it

**If Authorization header is present:**
→ Check backend logs

### Step 7: Check Backend Logs

Look in `backend/BusBookingAPI/logs/app-*.txt` for:

```
[INFO] User claims: NameIdentifier=1, Email=operator@test.com, Name=Operator Name, role=bus_operator
[INFO] Extracted operator ID: 1
```

**If you see:**
```
[ERROR] NameIdentifier claim not found in token
```
→ JWT token doesn't have NameIdentifier claim

**If you see:**
```
[ERROR] Failed to parse operator ID from claim: abc
```
→ Operator ID is not a valid integer

## Common Issues & Solutions

### Issue 1: Token Not Stored
**Symptom**: Console shows `Token exists: false`

**Solution**:
1. Check if login was successful
2. Check if token is in localStorage:
   - Open DevTools → Application → Local Storage
   - Look for key: `operator_auth_token`
   - Should have a long JWT string

**Fix**:
- Logout and login again
- Check login response in Network tab
- Verify token is returned from backend

### Issue 2: Token Not Sent
**Symptom**: Authorization header missing in Network tab

**Solution**:
1. Verify interceptor is registered in `app.config.ts`
2. Verify URL includes `/operator-dashboard`
3. Check console for interceptor logs

**Fix**:
- Rebuild frontend: `npm run build`
- Clear cache: `Ctrl+Shift+Delete`
- Restart frontend server

### Issue 3: Token Invalid
**Symptom**: Backend logs show claim errors

**Solution**:
1. Check JWT configuration in `appsettings.json`
2. Verify secret key matches
3. Check token expiration

**Fix**:
- Logout and login again
- Check if token is expired
- Verify JWT secret in backend config

### Issue 4: Still 401 After All Fixes
**Symptom**: All above checks pass but still 401

**Solution**:
1. Check backend authentication middleware order
2. Verify `[Authorize]` attribute is correct
3. Check if CORS is blocking requests

**Fix**:
- Restart backend: `dotnet run`
- Check backend logs for detailed errors
- Verify CORS policy allows requests

## Step-by-Step Debugging Checklist

- [ ] Rebuild backend: `dotnet build`
- [ ] Rebuild frontend: `npm run build`
- [ ] Clear browser cache: `Ctrl+Shift+Delete`
- [ ] Open DevTools: `F12`
- [ ] Go to Console tab
- [ ] Login as operator
- [ ] Check console logs for token
- [ ] Navigate to dashboard
- [ ] Check console logs for interceptor
- [ ] Click "Add New Bus"
- [ ] Check Network tab for Authorization header
- [ ] Check backend logs for claims
- [ ] Verify 200 OK response (not 401)

## Expected Console Output

### Successful Flow
```
[OperatorAuthInterceptor] URL: http://localhost:5266/api/operator-dashboard/buses
[OperatorAuthInterceptor] Token exists: true
[OperatorAuthInterceptor] Adding Authorization header
[OperatorAuthInterceptor] Response received for http://localhost:5266/api/operator-dashboard/buses
```

### Backend Logs
```
[INFO] User claims: NameIdentifier=1, Email=operator@test.com, Name=Operator Name, role=bus_operator
[INFO] Extracted operator ID: 1
[INFO] Fetching buses for operator 1
```

### Network Response
```
Status: 200 OK
Headers:
  Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## If Still Not Working

1. **Check backend is running**
   ```bash
   curl http://localhost:5266/api/health
   ```

2. **Check frontend is running**
   ```bash
   curl http://localhost:4200
   ```

3. **Check database connection**
   - Look in backend logs for database errors
   - Verify PostgreSQL is running

4. **Check JWT configuration**
   - Backend: `appsettings.json`
   - Verify Secret, Issuer, Audience match

5. **Check operator exists in database**
   ```sql
   SELECT * FROM bus_operators WHERE email = 'operator@test.com';
   ```

6. **Check token is valid**
   - Go to https://jwt.io
   - Paste token from localStorage
   - Verify claims are correct
   - Verify signature is valid

## Files with Logging

### Backend
- `backend/BusBookingAPI/Controllers/OperatorDashboardController.cs` - Token validation logs
- `backend/BusBookingAPI/logs/app-*.txt` - All backend logs

### Frontend
- `frontend/bus-booking/src/app/interceptors/operator-auth.interceptor.ts` - Interceptor logs
- Browser Console (F12) - All frontend logs

## Next Steps

1. **Rebuild and test**
2. **Check console logs**
3. **Check network tab**
4. **Check backend logs**
5. **Share logs if still not working**

---

**With these changes, you should be able to see exactly where the 401 is coming from and fix it accordingly.**
