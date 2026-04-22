# Bus Booking API - Detailed Documentation

## Available Buses Endpoint

This endpoint allows users to search for available buses based on source and destination districts.

### Endpoint Details

**URL**: `/api/user/available-buses`

**Methods**:

- POST (Request body JSON)
- GET (Query parameters)

---

## POST Method

### Request

```
POST /api/user/available-buses
Content-Type: application/json

{
  "sourceDistrict": "Mumbai",
  "destinationDistrict": "Pune"
}
```

### Parameters

| Parameter           | Type   | Required | Description                      |
| ------------------- | ------ | -------- | -------------------------------- |
| sourceDistrict      | string | Yes      | Name of the source district      |
| destinationDistrict | string | Yes      | Name of the destination district |

### Response - Success (200 OK)

```json
{
  "success": true,
  "message": "Found 3 available buses.",
  "buses": [
    {
      "id": 1,
      "registrationNumber": "MH-01-AB-1234",
      "operatorId": 5,
      "operatorName": "Express Travels",
      "routeId": 10,
      "seatingCapacity": 50,
      "isActive": true,
      "sourceCity": "Mumbai",
      "destinationCity": "Pune",
      "distanceKm": 150.5,
      "estimatedDurationHours": 3.5
    },
    {
      "id": 2,
      "registrationNumber": "MH-01-AB-1235",
      "operatorId": 5,
      "operatorName": "Express Travels",
      "routeId": 10,
      "seatingCapacity": 45,
      "isActive": true,
      "sourceCity": "Mumbai",
      "destinationCity": "Pune",
      "distanceKm": 150.5,
      "estimatedDurationHours": 3.5
    }
  ]
}
```

### Response - Error (400 Bad Request)

```json
{
  "success": false,
  "message": "Source and destination districts are required.",
  "buses": []
}
```

When no buses are found:

```json
{
  "success": true,
  "message": "No available buses found for the given route.",
  "buses": []
}
```

---

## GET Method

### Request

```
GET /api/user/available-buses?sourceDistrict=Mumbai&destinationDistrict=Pune
```

### Parameters

| Parameter           | Type   | Location | Required | Description                      |
| ------------------- | ------ | -------- | -------- | -------------------------------- |
| sourceDistrict      | string | Query    | Yes      | Name of the source district      |
| destinationDistrict | string | Query    | Yes      | Name of the destination district |

### Example cURL Request

```bash
curl -X GET "https://localhost:5001/api/user/available-buses?sourceDistrict=Mumbai&destinationDistrict=Pune" \
  -H "Content-Type: application/json"
```

### Example JavaScript/Fetch

```javascript
// POST method
fetch("/api/user/available-buses", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
  },
  body: JSON.stringify({
    sourceDistrict: "Mumbai",
    destinationDistrict: "Pune",
  }),
})
  .then((response) => response.json())
  .then((data) => console.log(data));

// GET method
fetch(
  "/api/user/available-buses?sourceDistrict=Mumbai&destinationDistrict=Pune",
)
  .then((response) => response.json())
  .then((data) => console.log(data));
```

### Example Python Request

```python
import requests

# POST method
response = requests.post('https://localhost:5001/api/user/available-buses',
    json={
        'sourceDistrict': 'Mumbai',
        'destinationDistrict': 'Pune'
    }
)
print(response.json())

# GET method
response = requests.get(
    'https://localhost:5001/api/user/available-buses',
    params={
        'sourceDistrict': 'Mumbai',
        'destinationDistrict': 'Pune'
    }
)
print(response.json())
```

---

## Response Fields

### Bus Object

| Field                  | Type    | Description                         |
| ---------------------- | ------- | ----------------------------------- |
| id                     | integer | Unique bus identifier               |
| registrationNumber     | string  | Vehicle registration number         |
| operatorId             | integer | ID of the bus operator              |
| operatorName           | string  | Name of the bus operator            |
| routeId                | integer | Route ID for this bus               |
| seatingCapacity        | integer | Total number of seats               |
| isActive               | boolean | Whether the bus is currently active |
| sourceCity             | string  | City of the source location         |
| destinationCity        | string  | City of the destination location    |
| distanceKm             | decimal | Distance in kilometers              |
| estimatedDurationHours | decimal | Estimated travel time in hours      |

---

## Error Codes

| Code | Message                                             | Reason                              |
| ---- | --------------------------------------------------- | ----------------------------------- |
| 400  | Source and destination districts are required.      | Missing required parameters         |
| 400  | An error occurred while retrieving available buses. | Server-side error during processing |

---

## Search Logic

The endpoint:

1. Accepts source and destination district names (case-insensitive)
2. Finds all routes where:
   - Source location's district matches the provided source district
   - Destination location's district matches the provided destination district
3. Returns only buses that are marked as `IsActive = true`
4. Includes full operator and route information

---

## Database Query Flow

1. **District Matching**: Matches district names from `districts` table
2. **Location Lookup**: Finds locations associated with those districts
3. **Route Finding**: Identifies routes connecting source to destination locations
4. **Bus Filtering**: Gets active buses on those routes
5. **Data Enrichment**: Includes operator and route details in response

---

## Notes

- District names are searched case-insensitively
- Only buses with `is_active = true` are returned
- The response includes comprehensive bus information for booking purposes
- The search is efficient with proper indexes on district names and bus status
