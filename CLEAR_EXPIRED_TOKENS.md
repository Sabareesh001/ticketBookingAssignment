# How to Fix the 401 Error

## Problem
The 401 errors are caused by **EXPIRED OPERATOR TOKENS** stored in your browser's localStorage.

The endpoints failing are:
- `/api/operator-dashboard/locations`
- `/api/operator-dashboard/routes`
- `/api/operator-dashboard/available-locations`

These are **OPERATOR DASHBOARD** endpoints that require valid authentication.

## Root Cause
Your operator token expired at: `4/24/2026 7:24:17 AM`
Current time: `4/24/2026 9:38:13 AM`

The token has been expired for over 2 hours.

## Solution

### Option 1: Clear Browser Storage (Recommended)
1. Open your browser (where the app is running at http://localhost:4200)
2. Press `F12` to open Developer Tools
3. Go to the **Application** tab (Chrome) or **Storage** tab (Firefox)
4. In the left sidebar, expand **Local Storage**
5. Click on `http://localhost:4200`
6. Find and delete these keys:
   - `operator_token`
   - `operator_user`
   - `auth_token` (if present)
   - `current_user` (if present)
7. Refresh the page (`F5`)

### Option 2: Clear All Site Data
1. Open Developer Tools (`F12`)
2. Go to **Application** tab
3. Click **Clear site data** button
4. Refresh the page

### Option 3: Use Console
1. Open Developer Tools (`F12`)
2. Go to **Console** tab
3. Run these commands:
```javascript
localStorage.removeItem('operator_token');
localStorage.removeItem('operator_user');
localStorage.removeItem('auth_token');
localStorage.removeItem('current_user');
location.reload();
```

## After Clearing Tokens

1. **Restart the Angular development server** to pick up the interceptor changes:
   ```bash
   cd frontend/bus-booking
   # Stop the server (Ctrl+C)
   npm start
   ```

2. **Log in again** as an operator to get a fresh token

## What Was Fixed

I've updated the `OperatorAuthInterceptor` to:
1. Check if the token is expired before sending it
2. Automatically logout and redirect if the token is expired
3. Prevent sending expired tokens to the backend

This will prevent 401 errors from expired tokens in the future.
