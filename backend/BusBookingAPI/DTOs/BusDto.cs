namespace BusBookingAPI.DTOs
{
    public class BusDto
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Timing Information
        public string OperatingDays { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
        public int AdvanceBookingDays { get; set; }
        
        // Schedule information
        public List<BusScheduleDto> Schedules { get; set; } = new List<BusScheduleDto>();
    }

    public class CreateBusDto
    {
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
        public decimal Price { get; set; }
        public string OperatingDays { get; set; } = "1,2,3,4,5,6,7";
        public TimeSpan PickupTime { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan DropTime { get; set; } = new TimeSpan(18, 0, 0);
        public decimal JourneyDurationHours { get; set; } = 10.00m;
    }

    public class UpdateBusDto
    {
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string OperatingDays { get; set; }
        public TimeSpan PickupTime { get; set; }
        public TimeSpan DropTime { get; set; }
        public decimal JourneyDurationHours { get; set; }
        public int AdvanceBookingDays { get; set; }
    }
}
