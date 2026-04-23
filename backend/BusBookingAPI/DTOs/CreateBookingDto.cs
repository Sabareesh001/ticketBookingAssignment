namespace BusBookingAPI.DTOs
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime TravelDate { get; set; }
        public string SeatNumbers { get; set; }
        public decimal TotalFare { get; set; }
    }
}
