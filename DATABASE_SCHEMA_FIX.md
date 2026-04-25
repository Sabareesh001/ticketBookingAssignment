# Database Schema Fix for Bus Availability Manager

## Problem
The backend is trying to query columns (`pickup_time`, `drop_time`, `journey_duration_hours`) that don't exist in the `bus_availability` table in the database.

Error:
```
Npgsql.PostgresException: 42703: column b.DropTime does not exist
```

## Root Cause
The `BusAvailability` model has these properties, but:
1. The database table doesn't have these columns
2. The DbContext configuration was missing the column mappings

## Solution

### Step 1: Update DbContext (Already Done)
The `BusBookingDbContext.cs` has been updated to include the column mappings:
```csharp
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.PickupTime).HasColumnName("pickup_time");
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.DropTime).HasColumnName("drop_time");
modelBuilder.Entity<BusAvailability>()
    .Property(ba => ba.JourneyDurationHours).HasColumnName("journey_duration_hours");
```

### Step 2: Add Columns to Database

#### Option A: Using pgAdmin (GUI)

1. Open pgAdmin
2. Navigate to: Databases → busBooking → Schemas → public → Tables → bus_availability
3. Right-click on `bus_availability` and select "Columns"
4. Add three new columns:
   - **Column Name:** `pickup_time` | **Data Type:** `time` | **Not Null:** No
   - **Column Name:** `drop_time` | **Data Type:** `time` | **Not Null:** No
   - **Column Name:** `journey_duration_hours` | **Data Type:** `numeric(5,2)` | **Not Null:** No
5. Click "Save"

#### Option B: Using SQL Query (Recommended)

1. Open pgAdmin Query Tool or any PostgreSQL client
2. Connect to the `busBooking` database
3. Run the following SQL:

```sql
ALTER TABLE bus_availability
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);
```

4. Verify the columns were added:

```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
ORDER BY ordinal_position;
```

You should see output like:
```
id                      | integer
bus_id                  | integer
available_date          | date
total_seats             | integer
available_seats         | integer
is_active               | boolean
schedule_id             | integer
pickup_time             | time without time zone
drop_time               | time without time zone
journey_duration_hours  | numeric
created_at              | timestamp with time zone
updated_at              | timestamp with time zone
```

#### Option C: Using Command Line (psql)

```bash
psql -U postgres -d busBooking -c "
ALTER TABLE bus_availability
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);
"
```

### Step 3: Rebuild and Restart Backend

```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

### Step 4: Verify the Fix

1. Wait for backend to start
2. Go to frontend
3. Log in as operator
4. Click "Bus Availability" tab
5. Should load without 500 error

## Verification Checklist

After applying the fix:

- [ ] Database columns added successfully
- [ ] Backend rebuilt and running
- [ ] No 500 error when accessing availability endpoint
- [ ] Can view availability records (or empty list)
- [ ] Can create new availability record
- [ ] Can edit availability record
- [ ] Can delete availability record

## If Still Getting Error

### Check 1: Verify Columns Exist
```sql
SELECT * FROM bus_availability LIMIT 1;
```

If you get an error about missing columns, the ALTER TABLE didn't work.

### Check 2: Check Column Names
```sql
\d bus_availability
```

Look for `pickup_time`, `drop_time`, `journey_duration_hours` in the output.

### Check 3: Verify DbContext Changes
Make sure `BusBookingDbContext.cs` has the three new property mappings.

### Check 4: Clear Entity Framework Cache
```bash
cd backend/BusBookingAPI
rm -rf bin obj
dotnet clean
dotnet build
```

## SQL Script File

A SQL script file `ADD_AVAILABILITY_COLUMNS.sql` has been created in the project root. You can run it directly:

```bash
psql -U postgres -d busBooking -f ADD_AVAILABILITY_COLUMNS.sql
```

## Column Details

| Column Name | Data Type | Nullable | Purpose |
|---|---|---|---|
| `pickup_time` | TIME | Yes | Time when bus picks up passengers |
| `drop_time` | TIME | Yes | Time when bus drops off passengers |
| `journey_duration_hours` | NUMERIC(5,2) | Yes | Duration of journey in hours (e.g., 8.5) |

## Data Types Explanation

- **TIME**: Stores time of day (HH:MM:SS format)
- **NUMERIC(5,2)**: Decimal number with 5 total digits, 2 after decimal point (e.g., 123.45)

## Rollback (If Needed)

If you need to remove these columns:

```sql
ALTER TABLE bus_availability
DROP COLUMN IF EXISTS pickup_time,
DROP COLUMN IF EXISTS drop_time,
DROP COLUMN IF EXISTS journey_duration_hours;
```

## Next Steps

After the database schema is fixed:

1. Test creating availability records with timing information
2. Verify all CRUD operations work
3. Test with different operators
4. Verify authorization works correctly

## Support

If you continue to have issues:

1. Check the backend logs for specific error messages
2. Verify the database connection is working
3. Ensure all three columns were added to the table
4. Verify the DbContext has the column mappings
5. Try a full clean rebuild of the backend
