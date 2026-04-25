# Deployment Guide - Operator Dashboard Fixes

## Pre-Deployment Checklist

### Code Review
- [x] Backend service methods reviewed
- [x] Backend controller endpoints reviewed
- [x] Frontend service methods reviewed
- [x] Frontend component methods reviewed
- [x] No compilation errors
- [x] No TypeScript errors
- [x] No C# errors

### Testing
- [ ] Unit tests passed (if applicable)
- [ ] Integration tests passed
- [ ] Manual testing completed
- [ ] Cross-browser testing completed
- [ ] Performance testing completed

### Documentation
- [x] OPERATOR_DASHBOARD_FIXES_SUMMARY.md created
- [x] OPERATOR_DASHBOARD_ARCHITECTURE.md created
- [x] IMPLEMENTATION_DETAILS.md created
- [x] QUICK_REFERENCE.md created
- [x] DEPLOYMENT_GUIDE.md created

### Database
- [x] Schema verified (no changes needed)
- [x] Indexes verified (already in place)
- [x] Test data prepared
- [x] Backup plan documented

## Deployment Steps

### Step 1: Backup Current State
```bash
# Backup database
mysqldump -u root -p busBooking > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup backend code
git commit -m "Pre-deployment backup"
git tag -a v_pre_operator_dashboard_fix -m "Before operator dashboard fixes"

# Backup frontend code
# (Already in git)
```

### Step 2: Deploy Backend Changes

#### 2.1 Update OperatorService.cs
- File: `backend/BusBookingAPI/Services/OperatorService.cs`
- Changes:
  - Added `GetOperatorRoutesAsync()` method
  - Added `GetOperatorAvailableLocationsAsync()` method
  - Updated interface with new method signatures

#### 2.2 Update OperatorDashboardController.cs
- File: `backend/BusBookingAPI/Controllers/OperatorDashboardController.cs`
- Changes:
  - Updated `GetOperatorRoutes()` endpoint (was `GetAllRoutes()`)
  - Updated `GetAvailableLocations()` endpoint (was `GetAllLocations()`)
  - Both now require authentication and filter by operator

#### 2.3 Rebuild Backend
```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet publish -c Release
```

#### 2.4 Verify Backend Compilation
```bash
# Check for build errors
dotnet build --no-restore

# Expected output: Build succeeded
```

### Step 3: Deploy Frontend Changes

#### 3.1 Update operator-dashboard.service.ts
- File: `frontend/bus-booking/src/app/services/operator-dashboard.service.ts`
- Changes:
  - Added `getAvailableLocations()` method
  - Updated `getOperatorRoutes()` method

#### 3.2 Update operator-dashboard.component.ts
- File: `frontend/bus-booking/src/app/pages/operator-dashboard/operator-dashboard.component.ts`
- Changes:
  - Updated `loadRoutes()` to call `getOperatorRoutes()`
  - Updated `loadAvailableLocations()` to call `getAvailableLocations()`

#### 3.3 Build Frontend
```bash
cd frontend/bus-booking
npm install
npm run build
```

#### 3.4 Verify Frontend Build
```bash
# Check for build errors
npm run build

# Expected output: Build succeeded
```

### Step 4: Deploy to Production

#### 4.1 Backend Deployment
```bash
# Stop current backend service
systemctl stop bus-booking-api

# Deploy new version
cp -r backend/BusBookingAPI/bin/Release/net10.0/publish/* /var/www/bus-booking-api/

# Start backend service
systemctl start bus-booking-api

# Verify backend is running
curl http://localhost:5266/api/health
```

#### 4.2 Frontend Deployment
```bash
# Deploy new build
cp -r frontend/bus-booking/dist/* /var/www/bus-booking-frontend/

# Verify frontend is accessible
curl http://localhost:4200
```

### Step 5: Smoke Testing

#### 5.1 Backend Smoke Tests
```bash
# Test authentication
curl -X POST http://localhost:5266/api/operator-auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"operatora@test.com","password":"password"}'

# Test new endpoints
curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5266/api/operator-dashboard/available-locations

curl -H "Authorization: Bearer <TOKEN>" \
  http://localhost:5266/api/operator-dashboard/routes
```

#### 5.2 Frontend Smoke Tests
1. Open browser to http://localhost:4200
2. Login as Operator A
3. Navigate to "My Buses" tab
4. Click "Add New Bus"
5. Verify locations dropdown shows only Operator A's locations
6. Verify routes dropdown shows only Operator A's routes
7. Logout and login as Operator B
8. Verify Operator B sees only their own data

### Step 6: Monitor Deployment

#### 6.1 Check Logs
```bash
# Backend logs
tail -f /var/log/bus-booking-api/app.log

# Frontend logs (browser console)
# Open DevTools → Console tab
```

#### 6.2 Monitor Performance
```bash
# Check API response times
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5266/api/operator-dashboard/available-locations

# Monitor database queries
# Check slow query log if enabled
```

#### 6.3 Monitor Errors
```bash
# Check for 500 errors
grep "500" /var/log/bus-booking-api/app.log

# Check for authorization errors
grep "401\|403" /var/log/bus-booking-api/app.log
```

## Rollback Plan

If issues occur, follow these steps to rollback:

### Immediate Rollback (< 5 minutes)

#### 1. Stop Services
```bash
systemctl stop bus-booking-api
systemctl stop bus-booking-frontend
```

#### 2. Restore Previous Version
```bash
# Backend
cp -r /var/backups/bus-booking-api-previous/* /var/www/bus-booking-api/

# Frontend
cp -r /var/backups/bus-booking-frontend-previous/* /var/www/bus-booking-frontend/
```

#### 3. Start Services
```bash
systemctl start bus-booking-api
systemctl start bus-booking-frontend
```

#### 4. Verify Rollback
```bash
curl http://localhost:5266/api/health
curl http://localhost:4200
```

### Database Rollback (if needed)
```bash
# Restore from backup
mysql -u root -p busBooking < backup_YYYYMMDD_HHMMSS.sql

# Verify data integrity
SELECT COUNT(*) FROM locations;
SELECT COUNT(*) FROM routes;
```

### Code Rollback
```bash
# Backend
git revert <commit-hash>
git push origin main

# Frontend
git revert <commit-hash>
git push origin main
```

## Post-Deployment Verification

### 24-Hour Monitoring

#### Hour 1: Critical Checks
- [ ] Backend API responding normally
- [ ] Frontend loading without errors
- [ ] Authentication working
- [ ] Locations dropdown showing correct data
- [ ] Routes dropdown showing correct data
- [ ] No 500 errors in logs
- [ ] No 401/403 errors for valid tokens

#### Hour 2-4: Extended Checks
- [ ] Bus creation working
- [ ] Location creation working
- [ ] Route filtering working
- [ ] Cross-operator data isolation verified
- [ ] Performance metrics normal
- [ ] Database queries optimized

#### Hour 4-24: Ongoing Monitoring
- [ ] Monitor error rates
- [ ] Monitor response times
- [ ] Monitor database performance
- [ ] Check user feedback
- [ ] Verify no data corruption

### Performance Baseline

Before deployment, establish baseline metrics:
```
Metric                          Baseline    Target
────────────────────────────────────────────────────
Available Locations API         <100ms      <150ms
Operator Routes API             <100ms      <150ms
Routes by Location API          <50ms       <100ms
Frontend Dropdown Load          <200ms      <300ms
Database Query Time             <50ms       <100ms
```

## Success Criteria

Deployment is successful if:

✅ **Functionality**
- Locations dropdown shows only operator's locations + system-wide
- Routes dropdown shows only operator's routes
- Route filtering by selected locations works
- Bus creation works with filtered data
- No data visible across operators

✅ **Performance**
- API response times < 150ms
- Database queries < 100ms
- No performance degradation
- No memory leaks

✅ **Security**
- Authentication required for all endpoints
- Operator ID from JWT token
- No unauthorized data access
- No SQL injection vulnerabilities

✅ **Stability**
- No 500 errors
- No unhandled exceptions
- Proper error handling
- Graceful error messages

✅ **User Experience**
- Dropdowns load quickly
- No confusion from seeing other operators' data
- Clear error messages
- Smooth workflow

## Communication Plan

### Before Deployment
- Notify operators of maintenance window
- Provide estimated downtime (if any)
- Explain new features/fixes

### During Deployment
- Monitor logs continuously
- Be ready to rollback
- Keep stakeholders informed

### After Deployment
- Confirm successful deployment
- Provide release notes
- Gather user feedback
- Monitor for issues

## Release Notes Template

```
## Operator Dashboard Fixes - Release v1.X.X

### What's New
- Fixed locations dropdown to show only operator's locations
- Fixed routes dropdown to show only operator's routes
- Improved data isolation between operators
- Enhanced security with proper authentication

### Bug Fixes
- Operators can no longer see other operators' locations
- Operators can no longer see other operators' routes
- Routes are properly filtered by selected locations

### Technical Changes
- Added GetOperatorRoutesAsync() method
- Added GetOperatorAvailableLocationsAsync() method
- Updated /api/operator-dashboard/routes endpoint
- Updated /api/operator-dashboard/available-locations endpoint

### Breaking Changes
- None (backward compatible)

### Migration Guide
- No database changes required
- No configuration changes required
- No user action required

### Known Issues
- None

### Support
- For issues, contact: support@example.com
- For documentation, see: OPERATOR_DASHBOARD_FIXES_SUMMARY.md
```

## Maintenance Tasks

### Weekly
- [ ] Monitor error logs
- [ ] Check performance metrics
- [ ] Verify data integrity

### Monthly
- [ ] Review slow queries
- [ ] Optimize database indexes if needed
- [ ] Update documentation

### Quarterly
- [ ] Performance review
- [ ] Security audit
- [ ] Capacity planning

## Contacts

- **Backend Lead**: [Name]
- **Frontend Lead**: [Name]
- **DevOps**: [Name]
- **Database Admin**: [Name]
- **Product Manager**: [Name]

## Appendix: Useful Commands

### Backend Commands
```bash
# Build
dotnet build

# Run tests
dotnet test

# Publish
dotnet publish -c Release

# Check logs
tail -f /var/log/bus-booking-api/app.log

# Restart service
systemctl restart bus-booking-api
```

### Frontend Commands
```bash
# Install dependencies
npm install

# Build
npm run build

# Run tests
npm test

# Serve locally
npm start

# Check logs
# Browser DevTools → Console
```

### Database Commands
```bash
# Backup
mysqldump -u root -p busBooking > backup.sql

# Restore
mysql -u root -p busBooking < backup.sql

# Check data
mysql -u root -p -e "SELECT COUNT(*) FROM busBooking.locations;"

# Monitor queries
SHOW PROCESSLIST;
```

### Git Commands
```bash
# Create tag
git tag -a v1.X.X -m "Release v1.X.X"

# Push tag
git push origin v1.X.X

# View commits
git log --oneline -10

# Revert commit
git revert <commit-hash>
```
