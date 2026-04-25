namespace BusBookingAPI.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class RouteDetailDto
    {
        public int Id { get; set; }
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public string SourceDistrictName { get; set; }
        public string DestinationDistrictName { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateRouteDto
    {
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
    }

    public class UpdateRouteDto
    {
        public int SourceLocationId { get; set; }
        public int DestinationLocationId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? EstimatedDurationHours { get; set; }
    }
}
