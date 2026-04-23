-- Tamil Nadu Districts Seeder
-- This script inserts all 38 districts of Tamil Nadu
-- It will ignore any districts that already exist (UNIQUE constraint on district_name, state_id)

-- First, ensure Tamil Nadu state exists
INSERT INTO states (state_name, country)
SELECT 'Tamil Nadu', 'India'
WHERE NOT EXISTS (SELECT 1 FROM states WHERE state_name = 'Tamil Nadu');

-- Get the state_id for Tamil Nadu (will use it for all districts)
-- Now insert all Tamil Nadu districts
INSERT INTO districts (district_name, state_id, created_at)
SELECT district_name, (SELECT id FROM states WHERE state_name = 'Tamil Nadu'), CURRENT_TIMESTAMP
FROM (
    VALUES
        ('Ariyalur'),
        ('Chengalpattu'),
        ('Chennai'),
        ('Coimbatore'),
        ('Cuddalore'),
        ('Dharmapuri'),
        ('Dindigul'),
        ('Erode'),
        ('Kallakurichi'),
        ('Kancheepuram'),
        ('Kanyakumari'),
        ('Karur'),
        ('Krishnagiri'),
        ('Madurai'),
        ('Nagapattinam'),
        ('Namakkal'),
        ('Nilgiris'),
        ('Perambalur'),
        ('Ramanathapuram'),
        ('Ranipet'),
        ('Salem'),
        ('Sivaganga'),
        ('Tenkasi'),
        ('Thiruppur'),
        ('Thiruvannamalai'),
        ('Tirupathur'),
        ('Tirunelveli'),
        ('Tuticorin'),
        ('Vellore'),
        ('Villupuram'),
        ('Virudhunagar')
) AS districts(district_name)
WHERE NOT EXISTS (
    SELECT 1 FROM districts d 
    WHERE d.district_name = districts.district_name 
    AND d.state_id = (SELECT id FROM states WHERE state_name = 'Tamil Nadu')
)
ON CONFLICT (district_name, state_id) DO NOTHING;

-- Display inserted/existing districts count
SELECT COUNT(*) as total_tamil_nadu_districts FROM districts 
WHERE state_id = (SELECT id FROM states WHERE state_name = 'Tamil Nadu');
