namespace BusBookingAPI.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public int DistrictId { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? OperatorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public District District { get; set; }
        public State State { get; set; }
        public Country Country { get; set; }
        public BusOperator Operator { get; set; }

        // Navigation properties
        public ICollection<Route> SourceRoutes { get; set; } = new List<Route>();
        public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
        public ICollection<Bus> SourceBuses { get; set; } = new List<Bus>();
        public ICollection<Bus> DestinationBuses { get; set; } = new List<Bus>();
    }
}
