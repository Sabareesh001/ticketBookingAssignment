namespace BusBookingAPI.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public int DistrictId { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public District District { get; set; }
        public State State { get; set; }

        // Navigation properties
        public ICollection<Route> SourceRoutes { get; set; } = new List<Route>();
        public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
    }
}
