namespace BusBookingAPI.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public int DistrictId { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? OperatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateLocationDto
    {
        public string StreetAddress { get; set; }
        public int DistrictId { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? OperatorId { get; set; }
    }

    public class UpdateLocationDto
    {
        public string StreetAddress { get; set; }
        public int DistrictId { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? OperatorId { get; set; }
    }
}
