namespace BusBookingAPI.Models
{
    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<District> Districts { get; set; } = new List<District>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
