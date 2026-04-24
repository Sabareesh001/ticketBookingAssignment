-- Migration Script for Seat Blocking Feature
-- This script adds seat reservation functionality to the existing bookings table
-- Run this on your existing database to add the new columns

-- Add new columns for seat reservation
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS is_reserved BOOLEAN DEFAULT FALSE,
ADD COLUMN IF NOT EXISTS reserved_until TIMESTAMP NULL;

-- Update the booking_status constraint to include 'reserved'
ALTER TABLE bookings 
DROP CONSTRAINT IF EXISTS bookings_booking_status_check;

ALTER TABLE bookings 
ADD CONSTRAINT bookings_booking_status_check 
CHECK (booking_status IN ('confirmed', 'cancelled', 'pending', 'reserved'));

-- Create index for efficient reservation queries
CREATE INDEX IF NOT EXISTS idx_bookings_reserved ON bookings(is_reserved, reserved_until);

-- Verify the changes
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'bookings' 
AND column_name IN ('is_reserved', 'reserved_until')
ORDER BY ordinal_position;

-- Show updated constraint
SELECT conname, pg_get_constraintdef(oid) 
FROM pg_constraint 
WHERE conname = 'bookings_booking_status_check';

COMMENT ON COLUMN bookings.is_reserved IS 'Flag indicating if this is a temporary seat reservation (true) or confirmed booking (false)';
COMMENT ON COLUMN bookings.reserved_until IS 'Timestamp when the reservation expires (NULL for confirmed bookings)';
