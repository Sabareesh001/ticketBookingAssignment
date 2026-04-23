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
        public bool IsActive { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateBusDto
    {
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
    }

    public class UpdateBusDto
    {
        public string RegistrationNumber { get; set; }
        public int OperatorId { get; set; }
        public int RouteId { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public int? SeatingCapacity { get; set; }
        public bool IsActive { get; set; }
    }
}
