namespace BusBookingAPI.DTOs
{
    public class UpdateBookingDto
    {
        public DateTime TravelDate { get; set; }
        public string SeatNumbers { get; set; }
        public decimal TotalFare { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TravelStatus { get; set; }
    }
}
