-- Create Countries Table
CREATE TABLE IF NOT EXISTS countries (
    id SERIAL PRIMARY KEY,
    country_name VARCHAR(100) NOT NULL UNIQUE,
    country_code CHAR(2) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better query performance
CREATE INDEX idx_countries_name ON countries(country_name);
CREATE INDEX idx_countries_code ON countries(country_code);
CREATE INDEX idx_states_country_id ON states(country_id);
