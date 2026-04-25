# Bus Operator Dashboard - Deployment Checklist

## Pre-Deployment Verification

### Backend (.NET)

- [ ] **Code Compilation**
  - [ ] No compilation errors
  - [ ] No warnings
  - [ ] All NuGet packages restored
  - [ ] Target framework: .NET 10

- [ ] **Database**
  - [ ] Migrations applied
  - [ ] Tables created: BusOperators, Locations, Countries, States, Districts
  - [ ] Foreign key relationships established
  - [ ] Indexes created for performance
  - [ ] Sample data inserted (optional)

- [ ] **Configuration**
  - [ ] JWT secret configured (min 32 characters)
  - [ ] JWT issuer set correctly
  - [ ] JWT audience set correctly
  - [ ] JWT expiration set (60 minutes)
  - [ ] Database connection string correct
  - [ ] CORS policy configured
  - [ ] Logging configured

- [ ] **Services Registration**
  - [ ] IOperatorDashboardService registered
  - [ ] IOperatorAuthService registered
  - [ ] ILocationService registered
  - [ ] All other services registered

- [ ] **Controllers**
  - [ ] OperatorDashboardController created
  - [ ] All endpoints implemented
  - [ ] Authorization attributes applied
  - [ ] Error handling implemented
  - [ ] Logging implemented

- [ ] **Security**
  - [ ] JWT authentication configured
  - [ ] Authorization middleware enabled
  - [ ] CORS properly configured
  - [ ] HTTPS enforced (production)
  - [ ] SQL injection prevention (EF Core)
  - [ ] Input validation implemented

- [ ] **Testing**
  - [ ] Unit tests pass (if applicable)
  - [ ] Integration tests pass (if applicable)
  - [ ] API endpoints tested with Postman/cURL
  - [ ] Error scenarios tested
  - [ ] Edge cases tested

### Frontend (Angular)

- [ ] **Code Compilation**
  - [ ] No TypeScript errors
  - [ ] No linting errors
  - [ ] No build warnings
  - [ ] Angular version compatible

- [ ] **Components**
  - [ ] OperatorDashboardComponent created
  - [ ] Template HTML valid
  - [ ] Styles applied correctly
  - [ ] Responsive design verified

- [ ] **Services**
  - [ ] OperatorDashboardService created
  - [ ] LocationService created
  - [ ] API URLs correct
  - [ ] Error handling implemented
  - [ ] Observables properly managed

- [ ] **Guards**
  - [ ] OperatorAuthGuard created
  - [ ] Guard logic correct
  - [ ] Redirects working

- [ ] **Routing**
  - [ ] /operator-dashboard route added
  - [ ] Guard applied to route
  - [ ] Redirects after login working
  - [ ] Redirects after signup working

- [ ] **Forms**
  - [ ] Form validation working
  - [ ] Error messages displaying
  - [ ] Cascading dropdowns working
  - [ ] Form submission working

- [ ] **UI/UX**
  - [ ] Toast notifications working
  - [ ] Loading states displaying
  - [ ] Empty states displaying
  - [ ] Pagination working
  - [ ] Modal opening/closing
  - [ ] Responsive design verified

- [ ] **Testing**
  - [ ] Component tests pass (if applicable)
  - [ ] Service tests pass (if applicable)
  - [ ] Manual testing completed
  - [ ] Cross-browser testing done
  - [ ] Mobile testing done

## Deployment Steps

### Backend Deployment

1. [ ] **Build**
   ```bash
   dotnet build --configuration Release
   ```

2. [ ] **Publish**
   ```bash
   dotnet publish --configuration Release --output ./publish
   ```

3. [ ] **Database Migration**
   ```bash
   dotnet ef database update
   ```

4. [ ] **Deploy to Server**
   - [ ] Copy published files to server
   - [ ] Set environment variables
   - [ ] Configure IIS/Kestrel
   - [ ] Set up SSL certificate
   - [ ] Configure firewall rules

5. [ ] **Verify Deployment**
   - [ ] API responds to requests
   - [ ] Database connection working
   - [ ] Authentication working
   - [ ] Logging working

### Frontend Deployment

1. [ ] **Build**
   ```bash
   ng build --configuration production
   ```

2. [ ] **Verify Build Output**
   - [ ] dist/ folder created
   - [ ] No errors in build
   - [ ] Bundle size acceptable
   - [ ] Source maps generated (optional)

3. [ ] **Deploy to Server**
   - [ ] Copy dist/ contents to web server
   - [ ] Configure web server (nginx/Apache)
   - [ ] Set up SSL certificate
   - [ ] Configure CORS headers
   - [ ] Set up redirects for SPA

4. [ ] **Verify Deployment**
   - [ ] Application loads
   - [ ] API calls working
   - [ ] Authentication working
   - [ ] Routing working

## Post-Deployment Testing

### Functional Testing

- [ ] **Authentication**
  - [ ] Operator can log in
  - [ ] Operator redirected to dashboard
  - [ ] Invalid credentials rejected
  - [ ] Token expires correctly
  - [ ] Logout works

- [ ] **Locations Manager**
  - [ ] Can view locations
  - [ ] Can create location
  - [ ] Can edit location
  - [ ] Can delete location
  - [ ] Pagination works
  - [ ] Dropdowns cascade correctly

- [ ] **Data Integrity**
  - [ ] Only operator's locations shown
  - [ ] Cannot access other operators' locations
  - [ ] Data persists after refresh
  - [ ] Timestamps correct

- [ ] **Error Handling**
  - [ ] Network errors handled
  - [ ] Validation errors shown
  - [ ] 404 errors handled
  - [ ] 401 errors redirect to login
  - [ ] 500 errors shown gracefully

### Performance Testing

- [ ] **Load Time**
  - [ ] Dashboard loads in < 3 seconds
  - [ ] API responses < 500ms
  - [ ] No memory leaks

- [ ] **Scalability**
  - [ ] 100+ locations load correctly
  - [ ] Pagination handles large datasets
  - [ ] No performance degradation

### Security Testing

- [ ] **Authentication**
  - [ ] Token validation working
  - [ ] Expired tokens rejected
  - [ ] Invalid tokens rejected

- [ ] **Authorization**
  - [ ] Operators can't access other operators' data
  - [ ] Unauthenticated users redirected
  - [ ] API endpoints protected

- [ ] **Input Validation**
  - [ ] SQL injection prevented
  - [ ] XSS prevented
  - [ ] Invalid data rejected

### Browser Compatibility

- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Safari (latest)
- [ ] Edge (latest)
- [ ] Mobile browsers

### Device Testing

- [ ] Desktop (1920x1080)
- [ ] Tablet (768x1024)
- [ ] Mobile (375x667)

## Monitoring & Logging

- [ ] **Backend Logging**
  - [ ] Logs configured
  - [ ] Log level appropriate
  - [ ] Logs stored securely
  - [ ] Log rotation configured

- [ ] **Frontend Logging**
  - [ ] Console errors monitored
  - [ ] Error tracking configured (optional)
  - [ ] Performance monitoring (optional)

- [ ] **Alerts**
  - [ ] API errors alert configured
  - [ ] Database errors alert configured
  - [ ] High error rate alert configured

## Documentation

- [ ] **API Documentation**
  - [ ] Endpoints documented
  - [ ] Request/response examples provided
  - [ ] Error codes documented
  - [ ] Authentication documented

- [ ] **User Documentation**
  - [ ] Quick start guide provided
  - [ ] Screenshots included
  - [ ] Common issues documented
  - [ ] Support contact provided

- [ ] **Developer Documentation**
  - [ ] Architecture documented
  - [ ] Setup instructions provided
  - [ ] Deployment instructions provided
  - [ ] Troubleshooting guide provided

## Rollback Plan

- [ ] **Backup**
  - [ ] Database backed up
  - [ ] Previous version backed up
  - [ ] Configuration backed up

- [ ] **Rollback Procedure**
  - [ ] Steps documented
  - [ ] Tested (if possible)
  - [ ] Team trained

- [ ] **Communication**
  - [ ] Stakeholders notified
  - [ ] Support team briefed
  - [ ] Users informed (if needed)

## Post-Deployment Monitoring (First 24 Hours)

- [ ] Monitor error logs
- [ ] Monitor API response times
- [ ] Monitor database performance
- [ ] Monitor user feedback
- [ ] Check for any issues
- [ ] Be ready to rollback if needed

## Sign-Off

- [ ] **Development Team**
  - [ ] Code review completed
  - [ ] Tests passed
  - [ ] Ready for deployment
  - [ ] Signed by: _________________ Date: _______

- [ ] **QA Team**
  - [ ] Testing completed
  - [ ] No critical issues
  - [ ] Ready for deployment
  - [ ] Signed by: _________________ Date: _______

- [ ] **DevOps Team**
  - [ ] Infrastructure ready
  - [ ] Deployment plan reviewed
  - [ ] Ready for deployment
  - [ ] Signed by: _________________ Date: _______

- [ ] **Product Owner**
  - [ ] Requirements met
  - [ ] Approved for deployment
  - [ ] Signed by: _________________ Date: _______

## Deployment Completed

- [ ] **Date**: _______
- [ ] **Time**: _______
- [ ] **Deployed By**: _______
- [ ] **Version**: _______
- [ ] **Notes**: _______

## Post-Deployment Review

- [ ] **Date**: _______
- [ ] **Issues Found**: _______
- [ ] **Resolution**: _______
- [ ] **Lessons Learned**: _______
- [ ] **Reviewed By**: _______

---

## Quick Reference

### Environment Variables (Backend)
```
DB_USER=postgres
DB_PASSWORD=<password>
DB_NAME=busBooking
DB_HOST=localhost
DB_PORT=5432
JWT_SECRET=<min 32 chars>
JWT_ISSUER=BusBookingAPI
JWT_AUDIENCE=BusBookingClient
JWT_EXPIRATION_MINUTES=60
```

### Build Commands

**Backend**
```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
dotnet ef database update
```

**Frontend**
```bash
ng build --configuration production
```

### Deployment Verification

**Backend**
```bash
curl -X GET http://localhost:5266/api/operator-dashboard/locations \
  -H "Authorization: Bearer <token>"
```

**Frontend**
```bash
# Check if application loads
curl http://localhost:4200
```

### Rollback Commands

**Backend**
```bash
# Restore previous version
# Restore database backup
dotnet ef database update <previous-migration>
```

**Frontend**
```bash
# Restore previous build
# Clear browser cache
```
