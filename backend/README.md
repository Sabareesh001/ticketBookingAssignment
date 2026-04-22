# Bus Booking API

ASP.NET Core Web API for bus booking system with PostgreSQL database.

## Features

- User management
- Bus and bus operator management
- Route management with locations and districts
- Available bus search by source and destination districts
- Booking management

## Prerequisites

- .NET 10.0 or later
- PostgreSQL database server
- Visual Studio Code or Visual Studio 2022 (optional)

## Setup Instructions

### 1. Database Setup

Before running the API, ensure PostgreSQL is running and set up the database:

```bash
# Using the provided SQL scripts (from ./database/setup)
psql -U postgres -f database/setup/route_and_locations.sql
psql -U postgres -f database/setup/bus_and_operator.sql
psql -U postgres -f database/setup/admin_and_user.sql
psql -U postgres -f database/setup/bookings.sql
```

Or execute the seeding scripts from `./database/seeding/` to populate test data.

### 2. Configure Database Connection

Edit `appsettings.json` in the `BusBookingAPI` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=busBooking;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

Update the connection string with your PostgreSQL credentials.

### 3. Restore Dependencies

```bash
cd backend/BusBookingAPI
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

The API will start on `https://localhost:5001` or `http://localhost:5000`.

## API Endpoints

### User Controller

#### Get Available Buses

**POST** `/api/user/available-buses`

Request body:

```json
{
  "sourceDistrict": "District Name",
  "destinationDistrict": "District Name"
}
```

Response:

```json
{
  "success": true,
  "message": "Found X available buses.",
  "buses": [
    {
      "id": 1,
      "registrationNumber": "MH-01-AB-1234",
      "operatorId": 1,
      "operatorName": "Operator Name",
      "routeId": 1,
      "seatingCapacity": 50,
      "isActive": true,
      "sourceCity": "Source City",
      "destinationCity": "Destination City",
      "distanceKm": 100.5,
      "estimatedDurationHours": 2.5
    }
  ]
}
```

**GET** `/api/user/available-buses`

Query parameters:

- `sourceDistrict`: Source district name
- `destinationDistrict`: Destination district name

Example: `GET /api/user/available-buses?sourceDistrict=Mumbai&destinationDistrict=Pune`

## Project Structure

```
BusBookingAPI/
├── Controllers/          # API controllers
│   └── UserController.cs
├── Models/              # Entity models
│   ├── User.cs
│   ├── Bus.cs
│   ├── Route.cs
│   ├── Location.cs
│   ├── District.cs
│   ├── State.cs
│   ├── BusOperator.cs
│   └── Booking.cs
├── Data/                # Database context
│   └── BusBookingDbContext.cs
├── DTOs/                # Data transfer objects
│   ├── BusDto.cs
│   ├── AvailableBusesRequest.cs
│   └── AvailableBusesResponse.cs
├── Services/            # Business logic
│   └── BusService.cs
├── Program.cs           # Application entry point
├── appsettings.json     # Configuration
└── appsettings.Development.json
```

## Database Schema

The application uses the following tables:

- **users**: User account information
- **states**: State/province information
- **districts**: Districts within states
- **locations**: Physical locations for bus stops
- **routes**: Bus routes with source and destination locations
- **bus_operators**: Bus company operators
- **buses**: Individual buses with their details
- **bookings**: Passenger bookings

## Technologies Used

- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL
- Npgsql (PostgreSQL data provider)

## Future Enhancements

- Authentication and authorization
- User registration and login
- Booking creation and management
- Payment integration
- Schedule management for buses
- Seat availability tracking
- Admin dashboard
- Email notifications

## Troubleshooting

### Connection String Issues

Ensure PostgreSQL is running and the connection string in `appsettings.json` is correct.

### Database Not Found

Create the database using: `CREATE DATABASE "busBooking";`

### Port Already in Use

Change the port in `launchSettings.json` under Properties folder.

## Support

For issues or questions, please refer to the database schema files in the `./database` folder.
