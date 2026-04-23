namespace BusBookingAPI.DTOs
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCountryDto
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }

    public class UpdateCountryDto
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}
