-- Create BusBooking Database
CREATE DATABASE IF NOT EXISTS "busBooking";

-- States Table
CREATE TABLE IF NOT EXISTS states (
    id SERIAL PRIMARY KEY,
    state_name VARCHAR(100) NOT NULL UNIQUE,
    country VARCHAR(100) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Districts Table
CREATE TABLE IF NOT EXISTS districts (
    id SERIAL PRIMARY KEY,
    district_name VARCHAR(100) NOT NULL,
    state_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (state_id) REFERENCES states(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE(district_name, state_id)
);

-- Locations Table
CREATE TABLE IF NOT EXISTS locations (
    id SERIAL PRIMARY KEY,
    street_address VARCHAR(255) NOT NULL,
    district_id INT NOT NULL,
    city VARCHAR(100) NOT NULL,
    state_id INT NOT NULL,
    country_id INT NOT NULL,
    postal_code VARCHAR(20),
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    operator_id INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (state_id) REFERENCES states(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (district_id) REFERENCES districts(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (country_id) REFERENCES countries(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (operator_id) REFERENCES bus_operators(id) ON DELETE SET NULL ON UPDATE CASCADE
);

-- Routes Table
CREATE TABLE IF NOT EXISTS routes (
    id SERIAL PRIMARY KEY,
    source_location_id INT NOT NULL,
    destination_location_id INT NOT NULL,
    distance_km DECIMAL(8, 2),
    estimated_duration_hours DECIMAL(5, 2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (source_location_id) REFERENCES locations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (destination_location_id) REFERENCES locations(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- Create indexes for better query performance
CREATE INDEX idx_states_name ON states(state_name);
CREATE INDEX idx_districts_name ON districts(district_name);
CREATE INDEX idx_districts_state_id ON districts(state_id);
CREATE INDEX idx_locations_city ON locations(city);
CREATE INDEX idx_locations_district_id ON locations(district_id);
CREATE INDEX idx_locations_state_id ON locations(state_id);
CREATE INDEX idx_locations_country_id ON locations(country_id);
CREATE INDEX idx_locations_operator_id ON locations(operator_id);
CREATE INDEX idx_routes_source ON routes(source_location_id);
CREATE INDEX idx_routes_destination ON routes(destination_location_id);
