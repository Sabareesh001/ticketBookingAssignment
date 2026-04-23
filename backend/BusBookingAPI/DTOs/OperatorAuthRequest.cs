namespace BusBookingAPI.DTOs
{
    public class OperatorLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class OperatorSignupRequest
    {
        public string OperatorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }

    public class OperatorAuthResponse
    {
        public string Token { get; set; }
        public OperatorDto Operator { get; set; }
        public string Message { get; set; }
    }
}
