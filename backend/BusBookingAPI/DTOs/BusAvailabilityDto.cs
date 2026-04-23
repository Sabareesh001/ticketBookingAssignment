namespace BusBookingAPI.DTOs
{
    public class BusAvailabilityDto
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public DateTime AvailableDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AvailableDatesRequest
    {
        public int BusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AvailableDatesResponse
    {
        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();
        public Dictionary<DateTime, int> DateAvailability { get; set; } = new Dictionary<DateTime, int>();
    }

    public class BusScheduleDto
    {
        public int BusId { get; set; }
        public string OperatingDays { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int AdvanceBookingDays { get; set; }
    }
}