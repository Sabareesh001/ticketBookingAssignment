namespace BusBookingAPI.DTOs
{
    public class StateDto
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStateDto
    {
        public string StateName { get; set; }
        public int CountryId { get; set; }
    }

    public class UpdateStateDto
    {
        public string StateName { get; set; }
        public int CountryId { get; set; }
    }
}
