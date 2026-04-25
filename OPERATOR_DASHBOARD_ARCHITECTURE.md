# Operator Dashboard Architecture - Before & After

## BEFORE (Issues)

```
┌─────────────────────────────────────────────────────────────────┐
│                    OPERATOR DASHBOARD                           │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Bus Creation Form                                        │  │
│  │                                                          │  │
│  │ Source Location: [Dropdown] ← getAllLocations()         │  │
│  │                              ❌ Returns ALL locations    │  │
│  │                                                          │  │
│  │ Destination Location: [Dropdown] ← getAllLocations()    │  │
│  │                                    ❌ Returns ALL        │  │
│  │                                                          │  │
│  │ Route: [Dropdown] ← getAllRoutes()                       │  │
│  │                    ❌ Returns ALL routes                 │  │
│  │                                                          │  │
│  │ [Save Bus]                                               │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  PROBLEM: Operator A can see Operator B's locations & routes!   │
└─────────────────────────────────────────────────────────────────┘

Database:
┌─────────────────────────────────────────────────────────────────┐
│ locations table                                                  │
├─────────────────────────────────────────────────────────────────┤
│ id │ city      │ street_address │ operator_id │ created_at      │
├─────────────────────────────────────────────────────────────────┤
│ 1  │ Mumbai    │ Main St        │ 1           │ 2024-04-20      │
│ 2  │ Delhi     │ Park Ave       │ 2           │ 2024-04-21      │
│ 3  │ Bangalore │ Tech Park      │ 1           │ 2024-04-22      │
│ 4  │ Chennai   │ Beach Rd       │ NULL        │ 2024-04-23      │
└─────────────────────────────────────────────────────────────────┘

When Operator 1 loads locations:
❌ Gets: [1, 2, 3, 4] - ALL locations including Operator 2's location!
✅ Should get: [1, 3, 4] - Only own locations + system-wide
```

## AFTER (Fixed)

```
┌─────────────────────────────────────────────────────────────────┐
│                    OPERATOR DASHBOARD                           │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Bus Creation Form                                        │  │
│  │                                                          │  │
│  │ Source Location: [Dropdown] ← getAvailableLocations()   │  │
│  │                              ✅ Filtered by operator    │  │
│  │                                                          │  │
│  │ Destination Location: [Dropdown] ← getAvailableLocations()
│  │                                    ✅ Filtered by operator
│  │                                                          │  │
│  │ Route: [Dropdown] ← getOperatorRoutes()                 │  │
│  │                    ✅ Filtered by operator              │  │
│  │                                                          │  │
│  │ [Save Bus]                                               │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ✅ FIXED: Operator A only sees their own locations & routes    │
└─────────────────────────────────────────────────────────────────┘

Database Query (Operator 1):
┌─────────────────────────────────────────────────────────────────┐
│ SELECT * FROM locations                                          │
│ WHERE operator_id = 1 OR operator_id IS NULL                    │
├─────────────────────────────────────────────────────────────────┤
│ id │ city      │ street_address │ operator_id │ created_at      │
├─────────────────────────────────────────────────────────────────┤
│ 1  │ Mumbai    │ Main St        │ 1           │ 2024-04-20      │
│ 3  │ Bangalore │ Tech Park      │ 1           │ 2024-04-22      │
│ 4  │ Chennai   │ Beach Rd       │ NULL        │ 2024-04-23      │
└─────────────────────────────────────────────────────────────────┘

✅ Gets: [1, 3, 4] - Only own locations + system-wide
```

## API Endpoint Changes

### Locations Endpoint

**BEFORE:**
```
GET /api/operator-dashboard/all-locations
├─ No authentication required
├─ Returns: ALL locations in system
└─ Service: OperatorService.GetAllLocationsAsync()
```

**AFTER:**
```
GET /api/operator-dashboard/available-locations
├─ Requires: JWT authentication
├─ Extracts: operatorId from JWT token
├─ Returns: Locations where (operator_id = operatorId OR operator_id IS NULL)
└─ Service: OperatorService.GetOperatorAvailableLocationsAsync(operatorId)
```

### Routes Endpoint

**BEFORE:**
```
GET /api/operator-dashboard/routes
├─ No authentication required
├─ Returns: ALL routes in system
└─ Service: OperatorService.GetAllRoutesAsync()
```

**AFTER:**
```
GET /api/operator-dashboard/routes
├─ Requires: JWT authentication
├─ Extracts: operatorId from JWT token
├─ Returns: Routes where both source and destination locations 
│           belong to operator OR are system-wide
└─ Service: OperatorService.GetOperatorRoutesAsync(operatorId)
```

## Data Flow Diagram

### Scenario: Operator A creates a bus

```
┌─────────────────────────────────────────────────────────────────┐
│ FRONTEND: Operator Dashboard Component                          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ngOnInit() called
                              │
                ┌─────────────┼─────────────┐
                ▼             ▼             ▼
          loadBuses()  loadLocations()  loadRoutes()
                │             │             │
                ▼             ▼             ▼
        getBuses()    getAvailableLocations()  getOperatorRoutes()
                │             │                      │
                └─────────────┼──────────────────────┘
                              ▼
        ┌─────────────────────────────────────────────────────────┐
        │ FRONTEND: OperatorDashboardService                      │
        └─────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                ▼             ▼             ▼
        GET /buses    GET /available-    GET /routes
                      locations
                │             │             │
                └─────────────┼─────────────┘
                              ▼
        ┌─────────────────────────────────────────────────────────┐
        │ BACKEND: OperatorDashboardController                    │
        └─────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                ▼             ▼             ▼
        GetMyBuses()  GetAvailableLocations()  GetOperatorRoutes()
                │             │                      │
                ▼             ▼                      ▼
        Extract operatorId from JWT token
                │             │                      │
                └─────────────┼──────────────────────┘
                              ▼
        ┌─────────────────────────────────────────────────────────┐
        │ BACKEND: OperatorService                                │
        └─────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                ▼             ▼             ▼
        GetOperator    GetOperator        GetOperator
        BusesAsync()   AvailableLocations RoutesAsync()
                       Async()
                │             │             │
                └─────────────┼─────────────┘
                              ▼
        ┌─────────────────────────────────────────────────────────┐
        │ DATABASE: BusBooking                                    │
        └─────────────────────────────────────────────────────────┘
                              │
                ┌─────────────┼─────────────┐
                ▼             ▼             ▼
        SELECT *      SELECT *          SELECT *
        FROM buses    FROM locations    FROM routes
        WHERE         WHERE             WHERE
        operator_id   operator_id=@op   source/dest
        =@operatorId  OR operator_id    locations
                      IS NULL           belong to @op
                │             │             │
                └─────────────┼─────────────┘
                              ▼
        ┌─────────────────────────────────────────────────────────┐
        │ RESPONSE: Filtered Data                                 │
        │ - Only Operator A's buses                               │
        │ - Only Operator A's locations + system-wide             │
        │ - Only routes connecting Operator A's locations         │
        └─────────────────────────────────────────────────────────┘
```

## Security Layers

```
┌─────────────────────────────────────────────────────────────────┐
│ LAYER 1: Frontend Authentication                                │
│ - JWT token stored in localStorage                              │
│ - Included in Authorization header                              │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ LAYER 2: Backend Authorization                                  │
│ - [Authorize] attribute on controller                           │
│ - Validates JWT token                                           │
│ - Extracts operatorId from token claims                         │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ LAYER 3: Data Filtering                                         │
│ - Service methods filter by operatorId                          │
│ - Database queries include WHERE clauses                        │
│ - Only authorized data is returned                              │
└─────────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│ LAYER 4: Access Control                                         │
│ - Endpoints validate ownership before modifications             │
│ - Returns 403 Forbidden if operator doesn't own resource        │
│ - Prevents unauthorized updates/deletes                         │
└─────────────────────────────────────────────────────────────────┘
```

## Route Selection Flow

```
User selects Source Location (e.g., Mumbai)
                │
                ▼
onSourceLocationChange() triggered
                │
                ▼
Check if both source and destination are selected
                │
                ▼
Call getRoutesByLocations(sourceId, destId)
                │
                ▼
GET /api/operator-dashboard/routes/{sourceId}/{destId}
                │
                ▼
Database: SELECT * FROM routes
          WHERE source_location_id = @sourceId
          AND destination_location_id = @destId
                │
                ▼
Returns: Routes connecting Mumbai to selected destination
                │
                ▼
Auto-select first route (if available)
                │
                ▼
✅ Source and destination districts now match the route!
```

## Summary of Changes

| Component | Before | After | Benefit |
|-----------|--------|-------|---------|
| Locations Dropdown | getAllLocations() | getAvailableLocations() | Only operator's locations visible |
| Routes Dropdown | getAllRoutes() | getOperatorRoutes() | Only operator's routes visible |
| Authentication | None | JWT required | Secure operator context |
| Data Isolation | None | Filtered by operatorId | Prevents cross-operator data access |
| Database Queries | No filtering | WHERE operator_id = @op | Efficient, secure queries |
| Security | Low | High | Multi-layer authorization |
