# Bus Availability Manager - Quick Start

## 🚀 Get It Running in 3 Steps

### Step 1: Restart Backend (REQUIRED)
```bash
cd backend/BusBookingAPI
dotnet clean
dotnet build
dotnet run
```

Wait for: `Now listening on: http://localhost:5266`

### Step 2: Verify API Works
Open browser: `http://localhost:5266/swagger`

Look for **BusAvailability** section with endpoints.

### Step 3: Test Frontend
1. Open: `http://localhost:4200`
2. Login as operator
3. Click **"Bus Availability"** tab
4. Should load without 404 errors ✅

## ✅ Quick Test

### Create Your First Availability Record:
1. Click **"+ Add Availability"** button
2. Select a bus from dropdown
3. Pick a date (today or future)
4. Seats will auto-populate
5. Click **"Create"**
6. Success! ✨

## 🔍 Still Getting 404?

### Check Backend is Running:
```bash
curl http://localhost:5266/api/busavailability
```

**Expected**: `[]` or array of records  
**If fails**: Backend not running - go back to Step 1

### Check Database Columns:
```sql
SELECT column_name FROM information_schema.columns 
WHERE table_name = 'bus_availability';
```

**Missing columns?** Run:
```sql
ALTER TABLE bus_availability 
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);
```

## 📚 Full Documentation

- **Complete Setup**: See `BUS_AVAILABILITY_COMPLETE_SETUP.md`
- **Implementation Details**: See `BUS_AVAILABILITY_MANAGER_IMPLEMENTATION.md`
- **Backend Instructions**: See `BACKEND_RESTART_INSTRUCTIONS.md`

## 🎯 What You Get

✅ Full CRUD for bus availability  
✅ Advanced filters (bus, status, date range)  
✅ Real-time search  
✅ Smart forms with validation  
✅ Auto-population of seat capacity  
✅ Pagination (10 per page)  
✅ Success/error notifications  
✅ Responsive design  

## 💡 Pro Tips

- **Auto-populate seats**: Select a bus and seats fill automatically
- **Date validation**: Can't select past dates
- **Seat validation**: Available seats can't exceed total
- **Quick filter**: Use search box for instant results
- **Clear filters**: One click to reset all filters

## 🆘 Need Help?

1. Check backend terminal for errors
2. Check browser console for frontend errors
3. Verify database has `bus_availability` table
4. Check logs in `backend/BusBookingAPI/logs/`
5. Review full documentation files

---

**That's it! You're ready to manage bus availability like a pro! 🚌✨**
