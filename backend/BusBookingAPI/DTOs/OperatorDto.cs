namespace BusBookingAPI.DTOs
{
    public class OperatorDto
    {
        public int Id { get; set; }
        public string OperatorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateOperatorDto
    {
        public string OperatorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
    }

    public class UpdateOperatorDto
    {
        public string OperatorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
