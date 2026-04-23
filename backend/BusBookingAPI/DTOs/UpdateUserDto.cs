namespace BusBookingAPI.DTOs
{
    public class UpdateUserDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
