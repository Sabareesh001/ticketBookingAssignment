    -- Bus Operators Table
    CREATE TABLE IF NOT EXISTS bus_operators (
        id SERIAL PRIMARY KEY,
        operator_name VARCHAR(150) NOT NULL UNIQUE,
        email VARCHAR(100) NOT NULL UNIQUE,
        phone_number VARCHAR(20) NOT NULL,
        license_number VARCHAR(50) NOT NULL UNIQUE,
        address VARCHAR(255),
        is_active BOOLEAN DEFAULT TRUE,
        password_hash VARCHAR(255) NOT NULL,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

    -- Create indexes for better query performance
    CREATE INDEX idx_bus_operators_email ON bus_operators(email);
    CREATE INDEX idx_bus_operators_phone ON bus_operators(phone_number);
    CREATE INDEX idx_bus_operators_license ON bus_operators(license_number);
    CREATE INDEX idx_bus_operators_active ON bus_operators(is_active);

    -- Bus Table
    CREATE TABLE IF NOT EXISTS buses (
        id SERIAL PRIMARY KEY,
        registration_number VARCHAR(50) NOT NULL UNIQUE,
        operator_id INT NOT NULL,
        route_id INT NOT NULL,
        seating_capacity INT,
        is_active BOOLEAN DEFAULT TRUE,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (operator_id) REFERENCES bus_operators(id) ON DELETE RESTRICT ON UPDATE CASCADE,
        FOREIGN KEY (route_id) REFERENCES routes(id) ON DELETE RESTRICT ON UPDATE CASCADE
    );

    -- Create indexes for better query performance
    CREATE INDEX idx_buses_registration ON buses(registration_number);
    CREATE INDEX idx_buses_operator ON buses(operator_id);
    CREATE INDEX idx_buses_route ON buses(route_id);
    CREATE INDEX idx_buses_active ON buses(is_active);
