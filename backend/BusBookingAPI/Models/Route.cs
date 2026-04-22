namespace BusBookingAPI.Models
{
    public class Route
    {
        public int Id { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Location SourceLocation { get; set; }
        public Location DestinationLocation { get; set; }

        // Navigation properties
        public ICollection<Bus> Buses { get; set; } = new List<Bus>();
    }
}
