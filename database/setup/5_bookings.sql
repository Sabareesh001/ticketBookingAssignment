-- Bus Bookings Table
CREATE TABLE IF NOT EXISTS bookings (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    bus_id INT NOT NULL,
    booking_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    travel_date DATE NOT NULL,
    seat_numbers VARCHAR(255) NOT NULL,
    total_fare DECIMAL(10, 2) NOT NULL,
    booking_status VARCHAR(50) DEFAULT 'confirmed',
    payment_status VARCHAR(50) DEFAULT 'pending',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (bus_id) REFERENCES buses(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CHECK (travel_date >= CURRENT_DATE),
    CHECK (booking_status IN ('confirmed', 'cancelled', 'pending')),
    CHECK (payment_status IN ('pending', 'completed', 'failed'))
);

-- Create indexes for better query performance
CREATE INDEX idx_bookings_user ON bookings(user_id);
CREATE INDEX idx_bookings_bus ON bookings(bus_id);
CREATE INDEX idx_bookings_travel_date ON bookings(travel_date);
CREATE INDEX idx_bookings_status ON bookings(booking_status);
CREATE INDEX idx_bookings_payment_status ON bookings(payment_status);
