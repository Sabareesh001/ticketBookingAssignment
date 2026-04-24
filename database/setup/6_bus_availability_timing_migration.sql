-- =====================================================
-- Bus Availability Timing Migration Script
-- This script adds date-specific timing columns to bus_availability table
-- =====================================================

-- Step 1: Check if columns already exist
DO $$ 
BEGIN
    -- Add pickup_time column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'bus_availability' AND column_name = 'pickup_time'
    ) THEN
        ALTER TABLE bus_availability 
        ADD COLUMN pickup_time TIME DEFAULT '08:00:00';
        RAISE NOTICE 'Added pickup_time column';
    ELSE
        RAISE NOTICE 'pickup_time column already exists';
    END IF;

    -- Add drop_time column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'bus_availability' AND column_name = 'drop_time'
    ) THEN
        ALTER TABLE bus_availability 
        ADD COLUMN drop_time TIME DEFAULT '18:00:00';
        RAISE NOTICE 'Added drop_time column';
    ELSE
        RAISE NOTICE 'drop_time column already exists';
    END IF;

    -- Add journey_duration_hours column if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'bus_availability' AND column_name = 'journey_duration_hours'
    ) THEN
        ALTER TABLE bus_availability 
        ADD COLUMN journey_duration_hours DECIMAL(5,2) DEFAULT 10.00;
        RAISE NOTICE 'Added journey_duration_hours column';
    ELSE
        RAISE NOTICE 'journey_duration_hours column already exists';
    END IF;
END $$;

-- Step 2: Create index for better performance (if not exists)
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE indexname = 'idx_bus_availability_timing'
    ) THEN
        CREATE INDEX idx_bus_availability_timing 
        ON bus_availability(bus_id, available_date, pickup_time);
        RAISE NOTICE 'Created index idx_bus_availability_timing';
    ELSE
        RAISE NOTICE 'Index idx_bus_availability_timing already exists';
    END IF;
END $$;

-- Step 3: Update existing bus_availability records with default timings from buses table
-- Only update records where timing is not set (NULL or default values)
UPDATE bus_availability ba 
SET 
    pickup_time = COALESCE(ba.pickup_time, b.pickup_time, '08:00:00'::TIME),
    drop_time = COALESCE(ba.drop_time, b.drop_time, '18:00:00'::TIME),
    journey_duration_hours = COALESCE(ba.journey_duration_hours, b.journey_duration_hours, 10.00)
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

-- Step 4: Verify the migration
DO $$ 
DECLARE
    total_records INTEGER;
    records_with_timing INTEGER;
    records_without_timing INTEGER;
BEGIN
    -- Count total records
    SELECT COUNT(*) INTO total_records FROM bus_availability;
    
    -- Count records with timing
    SELECT COUNT(*) INTO records_with_timing 
    FROM bus_availability 
    WHERE pickup_time IS NOT NULL 
    AND drop_time IS NOT NULL 
    AND journey_duration_hours IS NOT NULL
    AND pickup_time != '00:00:00'::TIME
    AND drop_time != '00:00:00'::TIME;
    
    -- Count records without timing
    records_without_timing := total_records - records_with_timing;
    
    RAISE NOTICE '=== Migration Summary ===';
    RAISE NOTICE 'Total bus_availability records: %', total_records;
    RAISE NOTICE 'Records with timing data: %', records_with_timing;
    RAISE NOTICE 'Records without timing data: %', records_without_timing;
    
    IF records_without_timing > 0 THEN
        RAISE WARNING 'Some records still do not have timing data. Please check the buses table for default timing values.';
    ELSE
        RAISE NOTICE 'All records have timing data. Migration successful!';
    END IF;
END $$;

-- Step 5: Display sample data to verify
SELECT 
    ba.id,
    ba.bus_id,
    ba.available_date,
    ba.pickup_time,
    ba.drop_time,
    ba.journey_duration_hours,
    ba.available_seats,
    b.registration_number,
    b.pickup_time as bus_default_pickup,
    b.drop_time as bus_default_drop
FROM bus_availability ba
JOIN buses b ON ba.bus_id = b.id
ORDER BY ba.bus_id, ba.available_date
LIMIT 10;

-- Step 6: Create a view for easy querying of availability with timing
CREATE OR REPLACE VIEW v_bus_availability_with_timing AS
SELECT 
    ba.id,
    ba.bus_id,
    ba.available_date,
    ba.total_seats,
    ba.available_seats,
    ba.is_active,
    ba.pickup_time,
    ba.drop_time,
    ba.journey_duration_hours,
    ba.schedule_id,
    ba.created_at,
    ba.updated_at,
    b.registration_number,
    b.operator_id,
    bo.operator_name,
    b.source_location_id,
    b.destination_location_id,
    sl.city as source_city,
    dl.city as destination_city,
    b.price
FROM bus_availability ba
JOIN buses b ON ba.bus_id = b.id
JOIN bus_operators bo ON b.operator_id = bo.id
JOIN locations sl ON b.source_location_id = sl.id
JOIN locations dl ON b.destination_location_id = dl.id
WHERE ba.is_active = true;

-- Grant permissions on the view
GRANT SELECT ON v_bus_availability_with_timing TO PUBLIC;

RAISE NOTICE '=== Migration Complete ===';
RAISE NOTICE 'You can now query the view: SELECT * FROM v_bus_availability_with_timing;';
