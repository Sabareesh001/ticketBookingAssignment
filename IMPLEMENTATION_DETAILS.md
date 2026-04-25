# Implementation Details & Testing Guide

## Code Changes Summary

### 1. Backend Service Layer (OperatorService.cs)

#### New Method: GetOperatorRoutesAsync
```csharp
public async Task<List<RouteDto>> GetOperatorRoutesAsync(int operatorId)
{
    _logger.LogInformation($"Fetching routes for operator with ID {operatorId}");

    var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == operatorId);
    if (!operatorExists)
    {
        throw new KeyNotFoundException($"Operator with ID {operatorId} not found");
    }

    // Get all routes where source or destination location belongs to the operator
    var routes = await _context.Routes
        .Where(r => 
            (r.SourceLocation.OperatorId == operatorId || r.SourceLocation.OperatorId == null) &&
            (r.DestinationLocation.OperatorId == operatorId || r.DestinationLocation.OperatorId == null)
        )
        .Include(r => r.SourceLocation)
        .Include(r => r.DestinationLocation)
        .ToListAsync();

    return routes.Select(MapRouteToDto).ToList();
}
```

**Key Points:**
- Validates operator exists before querying
- Filters routes where BOTH source AND destination locations are either:
  - Owned by the operator, OR
  - System-wide (OperatorId = NULL)
- Includes related location data for display names
- Logs all operations for debugging

#### New Method: GetOperatorAvailableLocationsAsync
```csharp
public async Task<List<LocationDto>> GetOperatorAvailableLocationsAsync(int operatorId)
{
    _logger.LogInformation($"Fetching available locations for operator with ID {operatorId}");

    var operatorExists = await _context.BusOperators.AnyAsync(o => o.Id == operatorId);
    if (!operatorExists)
    {
        throw new KeyNotFoundException($"Operator with ID {operatorId} not found");
    }

    // Get locations that belong to the operator or are system-wide (OperatorId is null)
    var locations = await _context.Locations
        .Where(l => l.OperatorId == operatorId || l.OperatorId == null)
        .ToListAsync();

    return locations.Select(MapLocationToDto).ToList();
}
```

**Key Points:**
- Validates operator exists before querying
- Returns locations where:
  - OperatorId matches current operator, OR
  - OperatorId is NULL (system-wide)
- Allows operators to use system-wide locations
- Prevents access to other operators' locations

### 2. Backend Controller Layer (OperatorDashboardController.cs)

#### Updated Endpoint: GetOperatorRoutes
```csharp
[HttpGet("routes")]
public async Task<ActionResult<List<RouteDto>>> GetOperatorRoutes()
{
    try
    {
        var operatorId = GetOperatorIdFromToken();
        _logger.LogInformation($"Fetching routes for operator {operatorId}");
        var routes = await _operatorService.GetOperatorRoutesAsync(operatorId);
        return Ok(routes);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error fetching routes: {ex.Message}");
        return StatusCode(500, new { message = "An error occurred while fetching routes" });
    }
}
```

**Key Points:**
- Requires [Authorize] attribute (inherited from class)
- Extracts operatorId from JWT token
- Calls new filtered service method
- Proper error handling with appropriate HTTP status codes

#### Updated Endpoint: GetAvailableLocations
```csharp
[HttpGet("available-locations")]
public async Task<ActionResult<List<LocationDto>>> GetAvailableLocations()
{
    try
    {
        var operatorId = GetOperatorIdFromToken();
        _logger.LogInformation($"Fetching available locations for operator {operatorId}");
        var locations = await _operatorService.GetOperatorAvailableLocationsAsync(operatorId);
        return Ok(locations);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error fetching locations: {ex.Message}");
        return StatusCode(500, new { message = "An error occurred while fetching locations" });
    }
}
```

**Key Points:**
- New endpoint path: `/available-locations` (instead of `/all-locations`)
- Requires authentication
- Filters by operator context
- Comprehensive error handling

### 3. Frontend Service Layer (operator-dashboard.service.ts)

#### New Method: getAvailableLocations
```typescript
getAvailableLocations(): Observable<LocationWithName[]> {
    const url = `${this.apiUrl}/available-locations`;
    return this.http.get<LocationWithName[]>(url).pipe(
        catchError(error => this.handleError(error))
    );
}
```

#### Updated Method: getOperatorRoutes
```typescript
getOperatorRoutes(): Observable<RouteWithNames[]> {
    const url = `${this.apiUrl}/routes`;
    return this.http.get<RouteWithNames[]>(url).pipe(
        catchError(error => this.handleError(error))
    );
}
```

**Key Points:**
- Both methods use the same endpoint path as before
- Backend now handles filtering based on JWT token
- Error handling with user-friendly messages
- Proper RxJS operators for error management

### 4. Frontend Component Layer (operator-dashboard.component.ts)

#### Updated Method: loadRoutes
```typescript
loadRoutes(): void {
    this.operatorDashboardService.getOperatorRoutes().subscribe({
        next: (data) => {
            this.routes = data;
            this.cdr.detectChanges();
        },
        error: (error) => {
            console.error('Failed to load routes:', error);
        }
    });
}
```

#### Updated Method: loadAvailableLocations
```typescript
loadAvailableLocations(): void {
    this.operatorDashboardService.getAvailableLocations().subscribe({
        next: (data) => {
            this.availableLocations = data;
            this.cdr.detectChanges();
        },
        error: (error) => {
            console.error('Failed to load available locations:', error);
        }
    });
}
```

**Key Points:**
- Calls new service methods
- Properly handles async operations with RxJS
- Updates component state and triggers change detection
- Error logging for debugging

## Testing Guide

### Prerequisites
- Backend running on `http://localhost:5266`
- Frontend running on `http://localhost:4200`
- Database populated with test data
- Two test operators created

### Test Data Setup

```sql
-- Create test operators
INSERT INTO bus_operators (operator_name, email, phone_number, license_number, address, password_hash, is_active, created_at, updated_at)
VALUES 
('Operator A', 'operatora@test.com', '9876543210', 'LIC001', 'Address A', 'hash1', true, NOW(), NOW()),
('Operator B', 'operatorb@test.com', '9876543211', 'LIC002', 'Address B', 'hash2', true, NOW(), NOW());

-- Create locations for Operator A
INSERT INTO locations (street_address, district_id, city, state_id, country_id, postal_code, operator_id, created_at, updated_at)
VALUES 
('Main Street', 1, 'Mumbai', 1, 1, '400001', 1, NOW(), NOW()),
('Tech Park', 2, 'Bangalore', 2, 1, '560001', 1, NOW(), NOW());

-- Create locations for Operator B
INSERT INTO locations (street_address, district_id, city, state_id, country_id, postal_code, operator_id, created_at, updated_at)
VALUES 
('Park Avenue', 3, 'Delhi', 3, 1, '110001', 2, NOW(), NOW()),
('Beach Road', 4, 'Chennai', 4, 1, '600001', 2, NOW(), NOW());

-- Create system-wide location
INSERT INTO locations (street_address, district_id, city, state_id, country_id, postal_code, operator_id, created_at, updated_at)
VALUES 
('Central Hub', 5, 'Pune', 5, 1, '411001', NULL, NOW(), NOW());

-- Create routes
INSERT INTO routes (source_location_id, destination_location_id, distance_km, estimated_duration_hours, created_at, updated_at)
VALUES 
(1, 2, 1000, 20, NOW(), NOW()),  -- Mumbai to Bangalore (Operator A)
(3, 4, 2000, 30, NOW(), NOW()),  -- Delhi to Chennai (Operator B)
(1, 5, 500, 10, NOW(), NOW());   -- Mumbai to Pune (Operator A + System)
```

### Test Case 1: Locations Dropdown Filtering

**Objective:** Verify that operators only see their own locations + system-wide locations

**Steps:**
1. Login as Operator A (operatora@test.com)
2. Navigate to "My Buses" tab
3. Click "Add New Bus"
4. Click on "Source Location" dropdown

**Expected Result:**
- Dropdown shows:
  - ✅ Mumbai (Operator A's location)
  - ✅ Bangalore (Operator A's location)
  - ✅ Pune (System-wide location)
- Dropdown does NOT show:
  - ❌ Delhi (Operator B's location)
  - ❌ Chennai (Operator B's location)

**Verification:**
```bash
# Check API response
curl -H "Authorization: Bearer <OPERATOR_A_TOKEN>" \
  http://localhost:5266/api/operator-dashboard/available-locations

# Should return only locations with operator_id = 1 or NULL
```

### Test Case 2: Routes Dropdown Filtering

**Objective:** Verify that operators only see routes connecting their locations

**Steps:**
1. Login as Operator A
2. Navigate to "My Buses" tab
3. Click "Add New Bus"
4. Check the "Route" dropdown

**Expected Result:**
- Dropdown shows:
  - ✅ Mumbai → Bangalore (Operator A's route)
  - ✅ Mumbai → Pune (Operator A + System route)
- Dropdown does NOT show:
  - ❌ Delhi → Chennai (Operator B's route)

**Verification:**
```bash
# Check API response
curl -H "Authorization: Bearer <OPERATOR_A_TOKEN>" \
  http://localhost:5266/api/operator-dashboard/routes

# Should return only routes where both locations belong to operator or are system-wide
```

### Test Case 3: Route Filtering by Selected Locations

**Objective:** Verify that routes are filtered when locations are selected

**Steps:**
1. Login as Operator A
2. Navigate to "My Buses" tab
3. Click "Add New Bus"
4. Select "Mumbai" as Source Location
5. Select "Bangalore" as Destination Location
6. Observe the Route dropdown

**Expected Result:**
- Route dropdown updates to show only:
  - ✅ Mumbai → Bangalore
- Route is auto-selected
- Source and destination districts match the route

**Verification:**
```bash
# Check API response
curl -H "Authorization: Bearer <OPERATOR_A_TOKEN>" \
  http://localhost:5266/api/operator-dashboard/routes/1/2

# Should return routes where source_location_id = 1 AND destination_location_id = 2
```

### Test Case 4: Cross-Operator Data Isolation

**Objective:** Verify that Operator B cannot see Operator A's data

**Steps:**
1. Login as Operator B (operatorb@test.com)
2. Navigate to "My Buses" tab
3. Click "Add New Bus"
4. Check the "Source Location" dropdown

**Expected Result:**
- Dropdown shows:
  - ✅ Delhi (Operator B's location)
  - ✅ Chennai (Operator B's location)
  - ✅ Pune (System-wide location)
- Dropdown does NOT show:
  - ❌ Mumbai (Operator A's location)
  - ❌ Bangalore (Operator A's location)

**Verification:**
```bash
# Check API response
curl -H "Authorization: Bearer <OPERATOR_B_TOKEN>" \
  http://localhost:5266/api/operator-dashboard/available-locations

# Should return only locations with operator_id = 2 or NULL
```

### Test Case 5: Authentication Required

**Objective:** Verify that endpoints require authentication

**Steps:**
1. Make request without Authorization header

**Expected Result:**
```
HTTP 401 Unauthorized
{
  "message": "Invalid operator token"
}
```

**Verification:**
```bash
# Should fail without token
curl http://localhost:5266/api/operator-dashboard/available-locations

# Should return 401 Unauthorized
```

### Test Case 6: Bus Creation with Filtered Data

**Objective:** Verify that bus creation works correctly with filtered locations and routes

**Steps:**
1. Login as Operator A
2. Navigate to "My Buses" tab
3. Click "Add New Bus"
4. Fill in form:
   - Registration Number: TEST001
   - Source Location: Mumbai
   - Destination Location: Bangalore
   - Route: Mumbai → Bangalore (auto-selected)
   - Seating Capacity: 50
   - Price: 1000
5. Click "Save Bus"

**Expected Result:**
- ✅ Bus created successfully
- ✅ Bus appears in "My Buses" list
- ✅ Bus shows correct route information
- ✅ Bus is associated with Operator A

**Verification:**
```bash
# Check database
SELECT * FROM buses WHERE operator_id = 1 AND registration_number = 'TEST001';

# Should return the newly created bus
```

## Performance Considerations

### Database Indexes
The following indexes are already in place for optimal performance:
```sql
CREATE INDEX idx_locations_operator_id ON locations(operator_id);
CREATE INDEX idx_routes_source ON routes(source_location_id);
CREATE INDEX idx_routes_destination ON routes(destination_location_id);
```

### Query Optimization
- Queries use indexed columns for filtering
- Related data is eagerly loaded with `.Include()`
- No N+1 query problems

### Caching Opportunities (Future)
- Cache operator's locations for 5 minutes
- Cache operator's routes for 5 minutes
- Invalidate cache on location/route creation

## Troubleshooting

### Issue: "Invalid operator token"
**Cause:** JWT token is missing or invalid
**Solution:** 
- Ensure user is logged in
- Check token expiration
- Verify token is included in Authorization header

### Issue: Empty dropdowns
**Cause:** No locations/routes exist for operator
**Solution:**
- Create locations first
- Create routes connecting those locations
- Verify operator_id is set correctly

### Issue: Seeing other operator's data
**Cause:** Old endpoint is being called
**Solution:**
- Clear browser cache
- Verify frontend is calling new endpoints
- Check network tab in browser DevTools

### Issue: 500 Internal Server Error
**Cause:** Database query error
**Solution:**
- Check backend logs
- Verify database connection
- Ensure all required tables exist
- Check for NULL reference exceptions

## Rollback Plan

If issues occur, revert to previous implementation:

1. **Backend:**
   - Revert OperatorService.cs to use `GetAllRoutesAsync()` and `GetAllLocationsAsync()`
   - Revert OperatorDashboardController.cs endpoints

2. **Frontend:**
   - Revert operator-dashboard.service.ts to use `getAllLocations()` and `getAllRoutes()`
   - Revert operator-dashboard.component.ts method calls

3. **Database:**
   - No changes needed (schema is backward compatible)

## Monitoring & Logging

### Key Logs to Monitor
```
[INFO] Fetching available locations for operator with ID {operatorId}
[INFO] Fetching routes for operator with ID {operatorId}
[ERROR] Error fetching locations: {error message}
[ERROR] Error fetching routes: {error message}
```

### Metrics to Track
- Response time for location/route endpoints
- Number of locations returned per operator
- Number of routes returned per operator
- Error rate for these endpoints

## Deployment Checklist

- [ ] Backend code reviewed and tested
- [ ] Frontend code reviewed and tested
- [ ] Database schema verified (no changes needed)
- [ ] All test cases passed
- [ ] Performance testing completed
- [ ] Security review completed
- [ ] Documentation updated
- [ ] Rollback plan documented
- [ ] Monitoring configured
- [ ] Deployment scheduled during low-traffic period
