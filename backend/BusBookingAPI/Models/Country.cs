namespace BusBookingAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; } // ISO 3166-1 alpha-2 code (e.g., "IN", "US")
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<State> States { get; set; } = new List<State>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
