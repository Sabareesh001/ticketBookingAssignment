namespace BusBookingAPI.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int? SeatingCapacity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public BusOperator Operator { get; set; }
        public Route Route { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
