namespace BusBookingAPI.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
        public decimal Price { get; set; } = 500.00m;
        public bool IsActive { get; set; } = true;
        public string OperatingDays { get; set; } = "1,2,3,4,5,6,7"; // 1=Monday, 7=Sunday
        public TimeSpan PickupTime { get; set; } = new TimeSpan(8, 0, 0); // 08:00
        public TimeSpan DropTime { get; set; } = new TimeSpan(18, 0, 0); // 18:00
        public decimal JourneyDurationHours { get; set; } = 10.00m;
        public TimeSpan DepartureTime { get; set; } = new TimeSpan(8, 0, 0); // 08:00 (alias for PickupTime)
        public TimeSpan ArrivalTime { get; set; } = new TimeSpan(18, 0, 0); // 18:00 (alias for DropTime)
        public int AdvanceBookingDays { get; set; } = 90;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public BusOperator Operator { get; set; }
        public Route Route { get; set; }
        public Location SourceLocation { get; set; }
        public Location DestinationLocation { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<BusAvailability> Availabilities { get; set; } = new List<BusAvailability>();
        public ICollection<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
    }
}
