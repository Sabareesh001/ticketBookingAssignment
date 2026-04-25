# Immediate Debug Steps - 401 Unauthorized

## What I Added

### Backend
- ✅ Explicit `AuthenticationSchemes = "Bearer"` on `[Authorize]`
- ✅ Detailed logging in token validation
- ✅ Logs all JWT claims
- ✅ Logs operator ID extraction

### Frontend
- ✅ Console logging in interceptor
- ✅ Logs token presence
- ✅ Logs Authorization header addition
- ✅ Logs all errors

## Do This Now

### Step 1: Rebuild Backend
```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

### Step 2: Rebuild Frontend
```bash
cd frontend/bus-booking
npm run build
npm start
```

### Step 3: Clear Browser Cache
- Press: `Ctrl+Shift+Delete`
- Select: "All time"
- Check: "Cookies" and "Cached images"
- Click: "Clear data"

### Step 4: Open DevTools
- Press: `F12`
- Go to: **Console** tab

### Step 5: Test
1. Login as operator
2. Go to "My Buses" tab
3. Click "Add New Bus"
4. **Look at Console** - you should see logs like:
   ```
   [OperatorAuthInterceptor] URL: http://localhost:5266/api/operator-dashboard/buses
   [OperatorAuthInterceptor] Token exists: true
   [OperatorAuthInterceptor] Adding Authorization header
   ```

### Step 6: Check Network Tab
1. Go to **Network** tab
2. Look for requests to `/api/operator-dashboard/`
3. Click on a request
4. Go to **Headers** tab
5. Look for:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
   ```

### Step 7: Check Backend Logs
1. Look in: `backend/BusBookingAPI/logs/app-*.txt`
2. Search for: `User claims:`
3. Should show:
   ```
   [INFO] User claims: NameIdentifier=1, Email=operator@test.com, ...
   [INFO] Extracted operator ID: 1
   ```

## What to Look For

### ✅ Success Indicators
- Console shows: `Token exists: true`
- Network shows: `Authorization: Bearer ...`
- Backend logs show: `User claims: NameIdentifier=...`
- Response status: **200 OK** (not 401)

### ❌ Problem Indicators
- Console shows: `Token exists: false` → Token not stored
- Network shows: No Authorization header → Interceptor not working
- Backend logs show: `NameIdentifier claim not found` → JWT invalid
- Response status: **401 Unauthorized** → Token rejected

## If Token Exists: false

**Problem**: Token not in localStorage

**Check**:
1. DevTools → Application → Local Storage
2. Look for key: `operator_auth_token`
3. Should have a long JWT string

**Fix**:
- Logout and login again
- Check login response in Network tab
- Verify backend returns token

## If No Authorization Header

**Problem**: Interceptor not adding header

**Check**:
1. Is interceptor registered in `app.config.ts`?
2. Is URL correct? (should include `/operator-dashboard`)
3. Did you rebuild frontend?

**Fix**:
- Rebuild: `npm run build`
- Clear cache: `Ctrl+Shift+Delete`
- Restart: `npm start`

## If Backend Shows Claim Error

**Problem**: JWT token invalid or missing claims

**Check**:
1. Backend logs for error message
2. Token in localStorage (copy it)
3. Go to https://jwt.io
4. Paste token
5. Check if claims are present

**Fix**:
- Logout and login again
- Check JWT configuration in `appsettings.json`
- Verify backend is generating token correctly

## If Still 401

**Do This**:
1. Restart backend: `dotnet run`
2. Restart frontend: `npm start`
3. Clear cache: `Ctrl+Shift+Delete`
4. Login again
5. Check all logs again

**If still broken**:
1. Share console logs
2. Share network tab screenshot
3. Share backend logs
4. Share error message

---

**The logging will tell us exactly where the problem is!**
