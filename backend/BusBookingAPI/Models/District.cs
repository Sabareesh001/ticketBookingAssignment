namespace BusBookingAPI.Models
{
    public class District
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int StateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public State State { get; set; }

        // Navigation properties
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
