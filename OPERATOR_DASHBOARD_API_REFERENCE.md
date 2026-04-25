# Operator Dashboard API Reference

## Base URL
```
http://localhost:5266/api/operator-dashboard
```

## Authentication
All endpoints require Bearer token in Authorization header:
```
Authorization: Bearer <jwt_token>
```

The JWT token must contain the `operatorId` claim, which is automatically extracted by the backend.

---

## Endpoints

### 1. Get All Operator Locations

**Request**
```http
GET /locations
Authorization: Bearer <jwt_token>
```

**Response (200 OK)**
```json
[
  {
    "id": 1,
    "streetAddress": "123 Main Street",
    "city": "New York",
    "postalCode": "10001",
    "countryId": 1,
    "stateId": 1,
    "districtId": 1,
    "latitude": 40.7128,
    "longitude": -74.0060,
    "operatorId": 5,
    "createdAt": "2026-04-24T10:30:00Z",
    "updatedAt": "2026-04-24T10:30:00Z"
  },
  {
    "id": 2,
    "streetAddress": "456 Park Avenue",
    "city": "Boston",
    "postalCode": "02101",
    "countryId": 1,
    "stateId": 2,
    "districtId": 5,
    "latitude": 42.3601,
    "longitude": -71.0589,
    "operatorId": 5,
    "createdAt": "2026-04-23T15:45:00Z",
    "updatedAt": "2026-04-23T15:45:00Z"
  }
]
```

**Error Responses**
```json
// 401 Unauthorized
{
  "message": "Invalid operator token"
}

// 500 Server Error
{
  "message": "An error occurred while retrieving locations"
}
```

---

### 2. Get Specific Location

**Request**
```http
GET /locations/{id}
Authorization: Bearer <jwt_token>
```

**Path Parameters**
- `id` (number, required): Location ID

**Response (200 OK)**
```json
{
  "id": 1,
  "streetAddress": "123 Main Street",
  "city": "New York",
  "postalCode": "10001",
  "countryId": 1,
  "stateId": 1,
  "districtId": 1,
  "latitude": 40.7128,
  "longitude": -74.0060,
  "operatorId": 5,
  "createdAt": "2026-04-24T10:30:00Z",
  "updatedAt": "2026-04-24T10:30:00Z"
}
```

**Error Responses**
```json
// 401 Unauthorized
{
  "message": "Invalid operator token"
}

// 404 Not Found
{
  "message": "Location with ID 999 not found or does not belong to this operator"
}

// 500 Server Error
{
  "message": "An error occurred while retrieving the location"
}
```

---

### 3. Create Location

**Request**
```http
POST /locations
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "streetAddress": "789 Oak Street",
  "city": "Chicago",
  "postalCode": "60601",
  "countryId": 1,
  "stateId": 3,
  "districtId": 10,
  "latitude": 41.8781,
  "longitude": -87.6298
}
```

**Request Body**
```typescript
{
  streetAddress: string (required, min 5 chars)
  city: string (required, min 2 chars)
  postalCode: string (required, 5-10 digits)
  countryId: number (required)
  stateId: number (required)
  districtId: number (required)
  latitude?: number (optional)
  longitude?: number (optional)
}
```

**Response (201 Created)**
```json
{
  "id": 3,
  "streetAddress": "789 Oak Street",
  "city": "Chicago",
  "postalCode": "60601",
  "countryId": 1,
  "stateId": 3,
  "districtId": 10,
  "latitude": 41.8781,
  "longitude": -87.6298,
  "operatorId": 5,
  "createdAt": "2026-04-24T11:00:00Z",
  "updatedAt": "2026-04-24T11:00:00Z"
}
```

**Error Responses**
```json
// 400 Bad Request - Invalid data
{
  "message": "District with ID 999 not found"
}

// 400 Bad Request - Validation error
{
  "message": "Street address is required (min 5 characters)"
}

// 401 Unauthorized
{
  "message": "Invalid operator token"
}

// 500 Server Error
{
  "message": "An error occurred while creating the location"
}
```

---

### 4. Update Location

**Request**
```http
PUT /locations/{id}
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "streetAddress": "789 Oak Street Updated",
  "city": "Chicago",
  "postalCode": "60601",
  "countryId": 1,
  "stateId": 3,
  "districtId": 10,
  "latitude": 41.8781,
  "longitude": -87.6298
}
```

**Path Parameters**
- `id` (number, required): Location ID

**Request Body**
```typescript
{
  streetAddress: string (required, min 5 chars)
  city: string (required, min 2 chars)
  postalCode: string (required, 5-10 digits)
  countryId: number (required)
  stateId: number (required)
  districtId: number (required)
  latitude?: number (optional)
  longitude?: number (optional)
}
```

**Response (200 OK)**
```json
{
  "id": 3,
  "streetAddress": "789 Oak Street Updated",
  "city": "Chicago",
  "postalCode": "60601",
  "countryId": 1,
  "stateId": 3,
  "districtId": 10,
  "latitude": 41.8781,
  "longitude": -87.6298,
  "operatorId": 5,
  "createdAt": "2026-04-24T11:00:00Z",
  "updatedAt": "2026-04-24T11:15:00Z"
}
```

**Error Responses**
```json
// 400 Bad Request - Invalid data
{
  "message": "State with ID 999 not found"
}

// 401 Unauthorized
{
  "message": "Invalid operator token"
}

// 404 Not Found
{
  "message": "Location with ID 999 not found or does not belong to this operator"
}

// 500 Server Error
{
  "message": "An error occurred while updating the location"
}
```

---

### 5. Delete Location

**Request**
```http
DELETE /locations/{id}
Authorization: Bearer <jwt_token>
```

**Path Parameters**
- `id` (number, required): Location ID

**Response (204 No Content)**
```
(empty body)
```

**Error Responses**
```json
// 401 Unauthorized
{
  "message": "Invalid operator token"
}

// 404 Not Found
{
  "message": "Location with ID 999 not found or does not belong to this operator"
}

// 500 Server Error
{
  "message": "An error occurred while deleting the location"
}
```

---

## HTTP Status Codes

| Code | Meaning | Scenario |
|------|---------|----------|
| 200 | OK | Successful GET, PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Invalid input, validation error |
| 401 | Unauthorized | Missing/invalid token, invalid operator |
| 404 | Not Found | Location doesn't exist or doesn't belong to operator |
| 500 | Server Error | Unexpected server error |

---

## Example Workflows

### Complete Create Workflow

1. **Get Countries** (from LocationService)
```http
GET /country
```

2. **Get States** (after country selected)
```http
GET /state/country/{countryId}
```

3. **Get Districts** (after state selected)
```http
GET /district/state/{stateId}
```

4. **Create Location**
```http
POST /operator-dashboard/locations
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "streetAddress": "123 Main St",
  "city": "New York",
  "postalCode": "10001",
  "countryId": 1,
  "stateId": 1,
  "districtId": 1,
  "latitude": 40.7128,
  "longitude": -74.0060
}
```

### Complete Update Workflow

1. **Get Location Details**
```http
GET /operator-dashboard/locations/{id}
Authorization: Bearer <jwt_token>
```

2. **Get States** (if country changed)
```http
GET /state/country/{countryId}
```

3. **Get Districts** (if state changed)
```http
GET /district/state/{stateId}
```

4. **Update Location**
```http
PUT /operator-dashboard/locations/{id}
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "streetAddress": "123 Main St Updated",
  "city": "New York",
  "postalCode": "10001",
  "countryId": 1,
  "stateId": 1,
  "districtId": 1,
  "latitude": 40.7128,
  "longitude": -74.0060
}
```

---

## Validation Rules

### Street Address
- Required
- Minimum 5 characters
- Maximum 255 characters

### City
- Required
- Minimum 2 characters
- Maximum 100 characters

### Postal Code
- Required
- Must be 5-10 digits
- Pattern: `^\d{5,10}$`

### Latitude
- Optional
- Valid decimal number
- Range: -90 to 90
- Pattern: `^-?\d+(\.\d+)?$`

### Longitude
- Optional
- Valid decimal number
- Range: -180 to 180
- Pattern: `^-?\d+(\.\d+)?$`

### Foreign Keys
- countryId: Must exist in Countries table
- stateId: Must exist in States table and belong to selected country
- districtId: Must exist in Districts table and belong to selected state

---

## Security Notes

1. **Operator Isolation**: Backend verifies operatorId from token matches location's operatorId
2. **Token Validation**: All requests require valid JWT token
3. **Ownership Verification**: Operators cannot access/modify other operators' locations
4. **Input Validation**: All inputs validated on backend
5. **Error Messages**: Generic error messages for security (no data leakage)

---

## Rate Limiting

Currently no rate limiting implemented. Consider adding:
- 100 requests per minute per operator
- 10 requests per second per IP

---

## Pagination

Currently implemented on frontend:
- 10 items per page
- Manual pagination (Previous/Next)

Consider backend pagination for large datasets:
```http
GET /locations?page=1&pageSize=10
```

---

## Sorting

Currently not implemented. Consider adding:
```http
GET /locations?sortBy=createdAt&sortOrder=desc
```

---

## Filtering

Currently not implemented. Consider adding:
```http
GET /locations?city=NewYork&state=1
```

---

## Caching

Consider implementing:
- ETag headers for GET requests
- Cache-Control headers
- 304 Not Modified responses

---

## Testing with cURL

```bash
# Get all locations
curl -X GET http://localhost:5266/api/operator-dashboard/locations \
  -H "Authorization: Bearer <jwt_token>"

# Create location
curl -X POST http://localhost:5266/api/operator-dashboard/locations \
  -H "Authorization: Bearer <jwt_token>" \
  -H "Content-Type: application/json" \
  -d '{
    "streetAddress": "123 Main St",
    "city": "New York",
    "postalCode": "10001",
    "countryId": 1,
    "stateId": 1,
    "districtId": 1
  }'

# Update location
curl -X PUT http://localhost:5266/api/operator-dashboard/locations/1 \
  -H "Authorization: Bearer <jwt_token>" \
  -H "Content-Type: application/json" \
  -d '{
    "streetAddress": "123 Main St Updated",
    "city": "New York",
    "postalCode": "10001",
    "countryId": 1,
    "stateId": 1,
    "districtId": 1
  }'

# Delete location
curl -X DELETE http://localhost:5266/api/operator-dashboard/locations/1 \
  -H "Authorization: Bearer <jwt_token>"
```

---

## Testing with Postman

1. Create new collection: "Operator Dashboard"
2. Add environment variable: `token` = JWT token
3. Create requests:
   - GET /locations
   - POST /locations
   - GET /locations/1
   - PUT /locations/1
   - DELETE /locations/1
4. Use `{{token}}` in Authorization header
5. Use `{{base_url}}` for base URL

---

## Troubleshooting

### 401 Unauthorized
- Check token is valid
- Check token hasn't expired
- Check Authorization header format: `Bearer <token>`

### 404 Not Found
- Check location ID exists
- Check location belongs to current operator
- Check location hasn't been deleted

### 400 Bad Request
- Check all required fields are provided
- Check field formats (postal code, coordinates)
- Check foreign key IDs exist

### 500 Server Error
- Check backend logs
- Check database connectivity
- Check database migrations ran
