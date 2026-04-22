namespace BusBookingAPI.Models
{
    public class BusOperator
    {
        public int Id { get; set; }
        public string OperatorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Bus> Buses { get; set; } = new List<Bus>();
    }
}
