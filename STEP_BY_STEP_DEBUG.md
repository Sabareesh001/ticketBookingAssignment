# Step-by-Step Debug for Operator Dashboard 401 Errors

## Current Status
- ✅ `/operator-dashboard/locations` - Working
- ❌ `/operator-dashboard/routes` - 401 Error
- ❌ `/operator-dashboard/available-locations` - 401 Error

## Debug Steps

### Step 1: Check Browser Console
1. Open the operator dashboard page
2. Open browser DevTools (F12)
3. Go to Console tab
4. Look for messages starting with `[OperatorAuthInterceptor]`
5. Check if you see:
   ```
   [OperatorAuthInterceptor] URL: http://localhost:5266/api/operator-dashboard/routes
   [OperatorAuthInterceptor] Token exists: true
   [OperatorAuthInterceptor] Adding Authorization header
   ```

### Step 2: Check Network Tab
1. Go to Network tab in DevTools
2. Refresh the operator dashboard
3. Look for requests to:
   - `/api/operator-dashboard/routes`
   - `/api/operator-dashboard/available-locations`
4. Click on each request and check:
   - **Request Headers**: Should have `Authorization: Bearer <token>`
   - **Response**: Check status code and response body

### Step 3: Test Token Manually
1. Open Console tab
2. Run this script:
```javascript
// Copy and paste this entire block
const token = localStorage.getItem('operator_auth_token');
console.log('Token exists:', !!token);

if (token) {
    // Test routes endpoint
    fetch('http://localhost:5266/api/operator-dashboard/routes', {
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(r => r.ok ? r.json() : r.text())
    .then(data => console.log('Routes result:', data))
    .catch(e => console.error('Routes error:', e));
    
    // Test available-locations endpoint
    fetch('http://localhost:5266/api/operator-dashboard/available-locations', {
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(r => r.ok ? r.json() : r.text())
    .then(data => console.log('Available-locations result:', data))
    .catch(e => console.error('Available-locations error:', e));
}
```

### Step 4: Check Backend Logs
1. Look at the backend console output
2. Check for log messages like:
   - `Fetching routes for operator {operatorId}`
   - `Fetching available locations for operator {operatorId}`
   - Any error messages

### Step 5: Verify Operator Exists
Run this in browser console:
```javascript
const token = localStorage.getItem('operator_auth_token');
if (token) {
    const payload = JSON.parse(atob(token.split('.')[1]));
    console.log('Operator ID from token:', payload.nameid || payload.sub);
    console.log('Full payload:', payload);
}
```

## Possible Issues

### Issue 1: Token Not Being Sent
- **Symptom**: No `Authorization` header in Network tab
- **Cause**: Interceptor not working
- **Fix**: Restart Angular dev server

### Issue 2: Wrong Token Format
- **Symptom**: 401 with "Invalid token" message
- **Cause**: Token corrupted or wrong format
- **Fix**: Clear token and log in again

### Issue 3: Operator Doesn't Exist
- **Symptom**: 404 or "Operator not found" error
- **Cause**: Operator ID in token doesn't match database
- **Fix**: Check database or create operator

### Issue 4: Service Method Error
- **Symptom**: 500 error in response
- **Cause**: Backend service throwing exception
- **Fix**: Check backend logs for stack trace

## Quick Fixes to Try

### Fix 1: Clear Everything and Start Fresh
```javascript
localStorage.clear();
location.reload();
// Then log in again
```

### Fix 2: Restart Angular Dev Server
```bash
# Stop the server (Ctrl+C)
# Then restart
npm start
```

### Fix 3: Check if Interceptor is Working
Add this to `operator-auth.interceptor.ts` temporarily:
```typescript
console.log('🔍 INTERCEPTOR CALLED:', request.url);
console.log('🔍 TOKEN EXISTS:', !!token);
```

## Report Back
After trying these steps, report:
1. What you see in the Console tab
2. What you see in the Network tab for the failing requests
3. Results of the manual token test
4. Any error messages from backend logs