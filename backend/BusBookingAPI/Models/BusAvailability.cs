namespace BusBookingAPI.Models
{
    public class BusAvailability
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public DateTime AvailableDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ScheduleId { get; set; }
        
        // Date-specific timing information
        public TimeSpan PickupTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan DropTime { get; set; } = new TimeSpan(18, 0, 0);
        public decimal JourneyDurationHours { get; set; } = 10.00m;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Bus Bus { get; set; }
        public BusSchedule Schedule { get; set; }
    }
}