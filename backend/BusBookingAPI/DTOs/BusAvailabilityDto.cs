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
        public int? ScheduleId { get; set; }
        
        // Date-specific timing information
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
        
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
        public List<AvailableDateInfo> AvailableDates { get; set; } = new List<AvailableDateInfo>();
        public Dictionary<DateTime, AvailableDateInfo> DateAvailability { get; set; } = new Dictionary<DateTime, AvailableDateInfo>();
    }

    public class AvailableDateInfo
    {
        public DateTime Date { get; set; }
        public int AvailableSeats { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
        public string FormattedPickupTime => FormatTime(PickupTime);
        public string FormattedDropTime => FormatTime(DropTime);
        
        private string FormatTime(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var ampm = hours >= 12 ? "PM" : "AM";
            var displayHours = hours % 12;
            if (displayHours == 0) displayHours = 12;
            return $"{displayHours}:{minutes:D2} {ampm}";
        }
    }

    public class BusScheduleDto
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public string ScheduleName { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public bool IsActive { get; set; }
        public string OperatingDays { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
    }

    public class CreateBusScheduleDto
    {
        public int BusId { get; set; }
        public string ScheduleName { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public string OperatingDays { get; set; } = "1,2,3,4,5,6,7";
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }

    public class UpdateBusScheduleDto
    {
        public string ScheduleName { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public bool IsActive { get; set; }
        public string OperatingDays { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
    }

    public class UpdateBusAvailabilityTimingDto
    {
        public int BusId { get; set; }
        public DateTime AvailableDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
    }

    public class BulkUpdateAvailabilityTimingDto
    {
        public int BusId { get; set; }
        public List<DateTime> Dates { get; set; } = new List<DateTime>();
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
    }
}