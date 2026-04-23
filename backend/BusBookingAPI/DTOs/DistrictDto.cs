namespace BusBookingAPI.DTOs
{
    public class DistrictDto
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int StateId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDistrictDto
    {
        public string DistrictName { get; set; }
        public int StateId { get; set; }
    }

    public class UpdateDistrictDto
    {
        public string DistrictName { get; set; }
        public int StateId { get; set; }
    }
}
