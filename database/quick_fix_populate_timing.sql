-- =====================================================
-- Quick Fix: Populate Missing Timing Data
-- Run this if bus_availability records are missing timing information
-- =====================================================

-- Option 1: Update all records with timing from buses table
UPDATE bus_availability ba 
SET 
    pickup_time = b.pickup_time,
    drop_time = b.drop_time,
    journey_duration_hours = b.journey_duration_hours,
    updated_at = CURRENT_TIMESTAMP
FROM buses b 
WHERE ba.bus_id = b.id
AND (
    ba.pickup_time IS NULL 
    OR ba.drop_time IS NULL 
    OR ba.journey_duration_hours IS NULL
    OR ba.pickup_time = '00:00:00'::TIME
    OR ba.drop_time = '00:00:00'::TIME
    OR ba.journey_duration_hours = 0
);

-- Verify the update
SELECT 
    'After Update - Records Status' as status,
    COUNT(*) as total_records,
    COUNT(CASE WHEN pickup_time IS NOT NULL AND pickup_time != '00:00:00'::TIME THEN 1 END) as with_pickup,
    COUNT(CASE WHEN drop_time IS NOT NULL AND drop_time != '00:00:00'::TIME THEN 1 END) as with_drop,
    COUNT(CASE WHEN journey_duration_hours IS NOT NULL AND journey_duration_hours > 0 THEN 1 END) as with_duration
FROM bus_availability;

-- Show sample updated records
SELECT 
    ba.id,
    ba.bus_id,
    ba.available_date,
    ba.pickup_time,
    ba.drop_time,
    ba.journey_duration_hours,
    b.registration_number
FROM bus_availability ba
JOIN buses b ON ba.bus_id = b.id
ORDER BY ba.bus_id, ba.available_date
LIMIT 10;
