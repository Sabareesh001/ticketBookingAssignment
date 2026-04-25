namespace BusBookingAPI.Models
{
    public class BusSchedule
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public string ScheduleName { get; set; } = "Default Schedule";
        public bool IsActive { get; set; } = true;
        public string OperatingDays { get; set; } = "1,2,3,4,5,6,7"; // 1=Monday, 7=Sunday
        public DateTime EffectiveFrom { get; set; } = DateTime.Today;
        public DateTime EffectiveTo { get; set; } = DateTime.Today.AddYears(1);
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public Bus Bus { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<BusAvailability> Availabilities { get; set; } = new List<BusAvailability>();
    }
}