# Operator Dashboard Architecture

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     FRONTEND (Angular)                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  operator-dashboard.component.ts                         │  │
│  │  ├─ loadRoutes()                                         │  │
│  │  ├─ loadAvailableLocations()                            │  │
│  │  ├─ onSourceLocationChange()                            │  │
│  │  ├─ onDestinationLocationChange()                       │  │
│  │  ├─ saveBus()                                           │  │
│  │  └─ deleteLocation()                                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  operator-dashboard.service.ts                          │  │
│  │  ├─ getAllRoutes()                                       │  │
│  │  ├─ getRoutesByLocations()                              │  │
│  │  ├─ getAllLocations()                                   │  │
│  │  └─ getLocationsByDistrict()                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  operator-dashboard.component.html                      │  │
│  │  ├─ Location Dropdowns (City, Address)                  │  │
│  │  ├─ Route Dropdown (Source → Destination)               │  │
│  │  ├─ Bus Form                                            │  │
│  │  └─ Bus Cards (with location names)                     │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                           ↓ HTTP
┌─────────────────────────────────────────────────────────────────┐
│                    BACKEND (.NET 10)                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  OperatorDashboardController                            │  │
│  │  ├─ GET /routes                                         │  │
│  │  ├─ GET /routes/{sourceId}/{destId}                     │  │
│  │  ├─ GET /all-locations                                  │  │
│  │  ├─ GET /locations-by-district/{districtId}             │  │
│  │  ├─ POST /buses                                         │  │
│  │  ├─ PUT /buses/{id}                                     │  │
│  │  └─ DELETE /buses/{id}                                  │  │
│  └──────────────────────────────────────────────────────────┘  │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Services Layer                                         │  │
│  │  ├─ OperatorService                                     │  │
│  │  │  ├─ GetAllRoutesAsync()                              │  │
│  │  │  ├─ GetRoutesByLocationsAsync()                      │  │
│  │  │  ├─ GetAllLocationsAsync()                           │  │
│  │  │  └─ GetLocationsByDistrictAsync()                    │  │
│  │  ├─ RouteService                                        │  │
│  │  │  ├─ GetRouteByIdAsync()                              │  │
│  │  │  ├─ GetAllRoutesAsync()                              │  │
│  │  │  └─ GetRoutesByLocationsAsync()                      │  │
│  │  └─ LocationService                                     │  │
│  │     ├─ GetLocationByIdAsync()                           │  │
│  │     ├─ GetAllLocationsAsync()                           │  │
│  │     └─ GetLocationsByDistrictAsync()                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  DTOs (Data Transfer Objects)                           │  │
│  │  ├─ RouteDto (with location names)                      │  │
│  │  ├─ LocationDto (with displayName)                      │  │
│  │  ├─ BusDto                                              │  │
│  │  └─ CreateBusDto                                        │  │
│  └──────────────────────────────────────────────────────────┘  │
│                           ↓                                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Entity Framework Core                                  │  │
│  │  ├─ BusBookingDbContext                                 │  │
│  │  └─ Relationships & Mappings                            │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                           ↓ SQL
┌─────────────────────────────────────────────────────────────────┐
│                    DATABASE (SQL Server)                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐         │
│  │  locations   │  │    routes    │  │    buses     │         │
│  ├──────────────┤  ├──────────────┤  ├──────────────┤         │
│  │ id           │  │ id           │  │ id           │         │
│  │ city         │  │ source_loc_id│  │ registration │         │
│  │ street_addr  │  │ dest_loc_id  │  │ route_id     │         │
│  │ district_id  │  │ distance_km  │  │ source_loc_id│         │
│  │ state_id     │  │ duration_hrs │  │ dest_loc_id  │         │
│  │ country_id   │  └──────────────┘  │ capacity     │         │
│  │ postal_code  │                     │ price        │         │
│  │ latitude     │                     │ active       │         │
│  │ longitude    │                     └──────────────┘         │
│  └──────────────┘                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

### Adding a Bus

```
User Action: Select Source Location
        ↓
Component: onSourceLocationChange()
        ↓
Service: getRoutesByLocations(sourceId, destId)
        ↓
API: GET /api/operator-dashboard/routes/{sourceId}/{destId}
        ↓
Backend: OperatorService.GetRoutesByLocationsAsync()
        ↓
Database: Query routes with location relationships
        ↓
Response: RouteDto[] with location names
        ↓
Component: Update routes dropdown
        ↓
Auto-select: First route selected
        ↓
User sees: "Source City → Destination City"
```

## Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                  Operator Dashboard                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  My Buses Tab                                        │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ Add New Bus Form                               │  │  │
│  │  │ ┌──────────────────────────────────────────┐   │  │  │
│  │  │ │ Registration Number: [_____________]     │   │  │  │
│  │  │ │ Source Location: [Dropdown ▼]            │   │  │  │
│  │  │ │   → Triggers: onSourceLocationChange()   │   │  │  │
│  │  │ │ Destination Location: [Dropdown ▼]       │   │  │  │
│  │  │ │   → Triggers: onDestinationLocationChange│   │  │  │
│  │  │ │ Route: [Dropdown ▼] (Auto-filtered)      │   │  │  │
│  │  │ │ Seating Capacity: [_____________]        │   │  │  │
│  │  │ │ Price: [_____________]                   │   │  │  │
│  │  │ │ [Save Bus Button]                        │   │  │  │
│  │  │ └──────────────────────────────────────────┘   │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │                                                      │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ Bus List                                       │  │  │
│  │  │ ┌──────────────────────────────────────────┐   │  │  │
│  │  │ │ Bus Card                                 │   │  │  │
│  │  │ │ Registration: KA-01-AB-1234              │   │  │  │
│  │  │ │ Route: Bangalore → Mysore                │   │  │  │
│  │  │ │ Distance: 150 km                         │   │  │  │
│  │  │ │ Duration: 3 hours                        │   │  │  │
│  │  │ │ Capacity: 40 | Price: ₹500               │   │  │  │
│  │  │ │ [Edit] [Delete]                          │   │  │  │
│  │  │ └──────────────────────────────────────────┘   │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  My Locations Tab                                    │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ Add New Location Form                          │  │  │
│  │  │ ┌──────────────────────────────────────────┐   │  │  │
│  │  │ │ Street Address: [_____________]          │   │  │  │
│  │  │ │ City: [_____________]                    │   │  │  │
│  │  │ │ District: [_____________]                │   │  │  │
│  │  │ │ State: [_____________]                   │   │  │  │
│  │  │ │ Country: [_____________]                 │   │  │  │
│  │  │ │ Postal Code: [_____________]             │   │  │  │
│  │  │ │ [Save Location Button]                   │   │  │  │
│  │  │ └──────────────────────────────────────────┘   │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  │                                                      │  │
│  │  ┌────────────────────────────────────────────────┐  │  │
│  │  │ Location List                                  │  │  │
│  │  │ ┌──────────────────────────────────────────┐   │  │  │
│  │  │ │ Location Card                            │   │  │  │
│  │  │ │ City: Bangalore                          │   │  │  │
│  │  │ │ Address: Main Street                     │   │  │  │
│  │  │ │ District: 1 | State: 1 | Country: 1      │   │  │  │
│  │  │ │ [Edit] [Delete]                          │   │  │  │
│  │  │ └──────────────────────────────────────────┘   │  │  │
│  │  └────────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## API Response Flow

```
Frontend Request
    ↓
GET /api/operator-dashboard/routes/1/2
    ↓
Backend Controller
    ↓
OperatorService.GetRoutesByLocationsAsync(1, 2)
    ↓
RouteService.GetRoutesByLocationsAsync(1, 2)
    ↓
Database Query:
    SELECT r.*, l1.city as source_city, l1.street_address as source_addr,
           l2.city as dest_city, l2.street_address as dest_addr
    FROM routes r
    JOIN locations l1 ON r.source_location_id = l1.id
    JOIN locations l2 ON r.destination_location_id = l2.id
    WHERE r.source_location_id = 1 AND r.destination_location_id = 2
    ↓
MapToDto() - Combine location data
    ↓
Response: RouteDto[] with:
    - id: 1
    - sourceLocationName: "Bangalore, Main Street"
    - destinationLocationName: "Mysore, Highway Road"
    - distanceKm: 150
    - estimatedDurationHours: 3
    ↓
Frontend receives and displays in dropdown
```

## State Management

```
Component State:
├─ buses: BusDto[]
├─ locations: LocationDto[]
├─ routes: RouteWithNames[]
├─ availableLocations: LocationWithName[]
├─ busForm: FormGroup
├─ locationForm: FormGroup
├─ activeTab: 'buses' | 'locations'
├─ showBusForm: boolean
├─ showLocationForm: boolean
├─ loading: boolean
├─ error: string
└─ successMessage: string

Form State:
├─ registrationNumber: string
├─ sourceLocationId: number
├─ destinationLocationId: number
├─ routeId: number
├─ seatingCapacity: number
├─ price: number
└─ isActive: boolean
```

## Error Handling Flow

```
User Action
    ↓
Try API Call
    ├─ Success (200)
    │   ├─ Update component state
    │   ├─ Show success message
    │   └─ Refresh data
    │
    └─ Error (4xx/5xx)
        ├─ Catch error
        ├─ Extract error message
        ├─ Display error message
        └─ Keep form open for retry
```

## Performance Optimization

```
Initial Load:
├─ Load all routes (cached)
├─ Load all locations (cached)
└─ Display UI

User Selects Location:
├─ Check if route exists in cache
├─ If not, fetch from API
├─ Filter routes in memory
└─ Update dropdown

User Saves Bus:
├─ Validate form
├─ Send POST request
├─ Refresh bus list
└─ Show success message
```

This architecture ensures:
- ✅ Separation of concerns
- ✅ Reusable services
- ✅ Efficient data loading
- ✅ Proper error handling
- ✅ Scalable design
