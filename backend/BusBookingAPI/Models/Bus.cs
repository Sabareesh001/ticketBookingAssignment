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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public BusOperator Operator { get; set; }
        public Route Route { get; set; }
        public Location SourceLocation { get; set; }
        public Location DestinationLocation { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
