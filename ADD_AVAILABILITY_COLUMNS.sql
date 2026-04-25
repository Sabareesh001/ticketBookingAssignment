-- SQL Migration Script to Add Missing Columns to bus_availability Table
-- Run this script against the busBooking database

-- Add the missing columns to bus_availability table
ALTER TABLE bus_availability
ADD COLUMN IF NOT EXISTS pickup_time TIME,
ADD COLUMN IF NOT EXISTS drop_time TIME,
ADD COLUMN IF NOT EXISTS journey_duration_hours NUMERIC(5,2);

-- Verify the columns were added
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'bus_availability' 
ORDER BY ordinal_position;
