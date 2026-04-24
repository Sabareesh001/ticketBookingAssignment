-- =====================================================
-- Diagnostic Script: Check Bus Availability Timing Columns
-- Run this to diagnose the current state of your database
-- =====================================================

-- Check 1: Verify if timing columns exist
SELECT 
    'Column Existence Check' as check_type,
    column_name,
    data_type,
    column_default,
    is_nullable
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
AND column_name IN ('pickup_time', 'drop_time', 'journey_duration_hours')
ORDER BY column_name;

-- Check 2: Count records with and without timing data
SELECT 
    'Timing Data Status' as check_type,
    COUNT(*) as total_records,
    COUNT(CASE WHEN pickup_time IS NOT NULL AND pickup_time != '00:00:00'::TIME THEN 1 END) as records_with_pickup,
    COUNT(CASE WHEN drop_time IS NOT NULL AND drop_time != '00:00:00'::TIME THEN 1 END) as records_with_drop,
    COUNT(CASE WHEN journey_duration_hours IS NOT NULL AND journey_duration_hours > 0 THEN 1 END) as records_with_duration
FROM bus_availability;

-- Check 3: Sample data from bus_availability
SELECT 
    'Sample Bus Availability Data' as check_type,
    id,
    bus_id,
    available_date,
    pickup_time,
    drop_time,
    journey_duration_hours,
    available_seats,
    total_seats
FROM bus_availability
ORDER BY bus_id, available_date
LIMIT 5;

-- Check 4: Compare with buses table default timing
SELECT 
    'Bus Default Timing' as check_type,
    b.id as bus_id,
    b.registration_number,
    b.pickup_time as bus_pickup_time,
    b.drop_time as bus_drop_time,
    b.journey_duration_hours as bus_journey_duration,
    COUNT(ba.id) as availability_records
FROM buses b
LEFT JOIN bus_availability ba ON b.id = ba.bus_id
GROUP BY b.id, b.registration_number, b.pickup_time, b.drop_time, b.journey_duration_hours
ORDER BY b.id
LIMIT 5;

-- Check 5: Find records with missing or zero timing
SELECT 
    'Records with Missing Timing' as check_type,
    ba.id,
    ba.bus_id,
    ba.available_date,
    ba.pickup_time,
    ba.drop_time,
    ba.journey_duration_hours,
    b.registration_number
FROM bus_availability ba
JOIN buses b ON ba.bus_id = b.id
WHERE ba.pickup_time IS NULL 
   OR ba.drop_time IS NULL 
   OR ba.journey_duration_hours IS NULL
   OR ba.pickup_time = '00:00:00'::TIME
   OR ba.drop_time = '00:00:00'::TIME
   OR ba.journey_duration_hours = 0
LIMIT 10;

-- Check 6: Verify indexes
SELECT 
    'Index Check' as check_type,
    indexname,
    indexdef
FROM pg_indexes 
WHERE tablename = 'bus_availability'
AND indexname LIKE '%timing%';

-- Check 7: Test query for specific bus and date (like the API endpoint)
SELECT 
    'API Endpoint Test Query' as check_type,
    ba.id,
    ba.bus_id,
    ba.available_date,
    ba.total_seats,
    ba.available_seats,
    ba.is_active,
    ba.schedule_id,
    ba.pickup_time,
    ba.drop_time,
    ba.journey_duration_hours,
    ba.created_at,
    ba.updated_at
FROM bus_availability ba
WHERE ba.bus_id = 1 
AND ba.available_date::date = '2026-04-24'::date
LIMIT 5;

-- Summary Report
SELECT 
    '=== SUMMARY REPORT ===' as report_section,
    '' as details
UNION ALL
SELECT 
    'Total Buses' as report_section,
    COUNT(*)::text as details
FROM buses
UNION ALL
SELECT 
    'Total Availability Records' as report_section,
    COUNT(*)::text as details
FROM bus_availability
UNION ALL
SELECT 
    'Records with Complete Timing' as report_section,
    COUNT(*)::text as details
FROM bus_availability
WHERE pickup_time IS NOT NULL 
AND drop_time IS NOT NULL 
AND journey_duration_hours IS NOT NULL
AND pickup_time != '00:00:00'::TIME
AND drop_time != '00:00:00'::TIME
AND journey_duration_hours > 0;
